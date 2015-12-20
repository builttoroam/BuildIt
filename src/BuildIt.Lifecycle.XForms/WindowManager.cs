using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BuildIt.Lifecycle.States;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        //private IDictionary<string, CoreWindow> Windows { get; }=new Dictionary<string, CoreWindow>(); 

            public NavigationPage NavigationRoot { get; }


        public WindowManager(NavigationPage navRoot, IHasRegionManager root)
        {
            NavigationRoot=navRoot;
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated += RegionManager_RegionCreated;
            RegionManager.RegionIsClosing += RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(object sender, ParameterEventArgs<IApplicationRegion> e)
        {
            //var view = Windows.SafeDictionaryValue<string, CoreWindow, CoreWindow>(e.Parameter1.RegionId);
            //view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(object sender, ParameterEventArgs<IApplicationRegion> e)
#pragma warning restore 1998
        {

            var newViewId = 0;
            e.Parameter1.UIContext.RunContext = new FormsUIContext();
            await e.Parameter1.UIContext.RunAsync(async () =>
            {
                //var navRoot = new NavigationPage();

                var region = e.Parameter1;

                var interfaces = region.GetType().GetTypeInfo().ImplementedInterfaces;//.GetInterfaces();
                foreach (var it in interfaces)
                {
                    if (it.IsConstructedGenericType && 
                    it.GetGenericTypeDefinition() == typeof(IHasViewModelStateManager<,>))
                    {
                        var args = it.GenericTypeArguments;
                        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
                        var fn = Activator.CreateInstance(fnt, NavigationRoot, region);//, string.Empty);
                    }
                }


                //NavigationRoot.Navigation.PushAsync()
                //Application.MainPage = navRoot;
                //Window.Current.Content = frame;

                region.CloseRegion += Region_CloseRegion;

                //Window.Current.Activate();

                //newViewId = ApplicationView.GetForCurrentView().Id;

                //Windows[region.RegionId] = Window.Current.CoreWindow;

                Task.Run(async () => await region.Startup(sender as IRegionManager));


            });

            //if (!isPrimary)
            //{
            //    var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            //    Debug.WriteLine(viewShown);
            //}
        }

        private void Region_CloseRegion(object sender, EventArgs e)
        {
            //Windows[(sender as IApplicationRegion).RegionId].Close();
        }
    }

    public interface IHasVisualStateManager
    {
        IVisualStateManager VisualStateManager { get; }


    }

    public interface IVisualStateManager
    {
        IDictionary<Type, IVisualStateGroup> VisualStateGroups { get; }
        void GoToState<TVisualState>(TVisualState state, bool animate = true) where TVisualState : struct;
    }

    public class VisualStateManager : IVisualStateManager
    {
        public IDictionary<Type, IVisualStateGroup> VisualStateGroups { get; } =
            new Dictionary<Type, IVisualStateGroup>();

        public void GoToState<TVisualState>(TVisualState state, bool animate = true) where TVisualState : struct
        {
            IVisualStateGroup group = VisualStateGroups.SafeValue(state.GetType());
            if (group == null) return;
            group.TransitionTo(state);
        }
    }

    public interface IVisualStateGroup
    {
        //IVisualState VisualState<TVisualState>(TVisualState state) where TVisualState : struct;
        void TransitionTo<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct;

    }

    public class VisualStateGroup<TVisualState> : IVisualStateGroup
        where TVisualState : struct
    {
        public IDictionary<TVisualState, IVisualState> VisualStates { get; } =
            new Dictionary<TVisualState, IVisualState>();

        private IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }=new Dictionary<Tuple<object, string>, IDefaultValue>();

        private IVisualState VisualState<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct
        {
            if (typeof(TFindVisualState) != typeof(TVisualState)) return null;
            var searchState = (TVisualState)(object)state;
            return VisualStates.SafeValue(searchState);
        }

        public void TransitionTo<TFindVisualState>(TFindVisualState state) where TFindVisualState : struct
        {
            var visualState = VisualState(state);
            if (visualState == null) return;
            visualState.TransitionTo(DefaultValues);
        }
    }

    public interface IVisualState
    {
        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }

    public class VisualState<TVisualState> : IVisualState
    {
        public VisualState(TVisualState state)
        {
            State = state;
        }
        public TVisualState State { get; set; }

        public IList<IVisualStateValue> Values { get; } = new List<IVisualStateValue>();
        public void TransitionTo(IDictionary<Tuple<object,string>, IDefaultValue> defaultValues )
        {
            var defaults = new Dictionary<Tuple<object, string>, IDefaultValue>(defaultValues);

            foreach (var visualStateValue in Values)
            {
                visualStateValue.TransitionTo(defaultValues);
                defaults.Remove(visualStateValue.Key);
            }

            foreach (var defValue in defaults)
            {
                defValue.Value.RevertToDefault();
            }
        }
    }

    public interface IDefaultValue
    {
        void RevertToDefault();
    }

    public class DefaultValue<TElement, TPropertyValue>:IDefaultValue
        where TElement:View
    {
        public VisualStateValue<TElement, TPropertyValue> VisualStateValue { get; set; }

        public TPropertyValue Value { get; set; }


        public void RevertToDefault()
        {
            VisualStateValue.Setter(VisualStateValue.Element, Value);
        }
    }

    public interface IVisualStateValue
    {
        Tuple<object, string> Key { get; }
        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }

    public class VisualStateValue<TElement, TPropertyValue> : IVisualStateValue
        where TElement : View
    {
        public Tuple<object,string> Key { get; set; }

        public TElement Element { get; set; }

        public Func<TElement, TPropertyValue> Getter { get; set; }

        public Action<TElement, TPropertyValue> Setter { get; set; }

        public TPropertyValue StateValue { get; set; }

        private IDefaultValue Default => new DefaultValue<TElement, TPropertyValue> {VisualStateValue = this, Value = Getter(Element)};

        public void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            if (!defaultValues.ContainsKey(Key))
            {
                defaultValues[Key] = Default;
            }
            Setter(Element, StateValue);
        }
    }

    public static class StateHelpers
    {
        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> Group<TVisualState>(
            this IVisualStateManager vsm) where TVisualState : struct
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsm.VisualStateGroups.Add(typeof(TVisualState),grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsm, grp);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> Group<TVisualState>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> vsmGroup) where TVisualState : struct
        {
            return vsmGroup.Item1.Group<TVisualState>();
        }

        public static Tuple<IVisualStateManager,
VisualStateGroup<TVisualState>> Group<TVisualState, TExistingVisualState>(
this Tuple<IVisualStateManager, VisualStateGroup<TExistingVisualState>, VisualState<TExistingVisualState>> vsmGroup, TVisualState state)
where TExistingVisualState : struct
where TVisualState : struct
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsmGroup.Item1.VisualStateGroups.Add(typeof(TVisualState), grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsmGroup.Item1, grp);
        }


        public static Tuple<IVisualStateManager,
VisualStateGroup<TVisualState>> Group<TVisualState, TExistingVisualState, TExistingElement, TExistingPropertyValue>(
this Tuple<IVisualStateManager, VisualStateGroup<TExistingVisualState>, VisualState<TExistingVisualState>,
 VisualStateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TVisualState state)
    where TExistingVisualState : struct
    where TVisualState : struct
    where TExistingElement : View
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsmGroup.Item1.VisualStateGroups.Add(typeof(TVisualState), grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsmGroup.Item1, grp);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> DefineState<TVisualState>(
             this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> vsmGroup, TVisualState state) where TVisualState : struct
        {
            var vs=new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state,vs);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>>(vsmGroup.Item1,vsmGroup.Item2,vs);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> DefineState<TVisualState>(
             this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> vsmGroup, TVisualState state) where TVisualState : struct
        {
            var vs = new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state, vs);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IVisualStateManager,
VisualStateGroup<TVisualState>,
VisualState<TVisualState>> DefineState<TVisualState, TExistingVisualState ,TExistingElement, TExistingPropertyValue>(
this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TExistingVisualState>,
 VisualStateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TVisualState state)
    where TExistingVisualState : struct
    where TVisualState : struct
    where TExistingElement : View
        {
            var vs = new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state,vs);
            return new Tuple<IVisualStateManager,
                VisualStateGroup<TVisualState>,
                VisualState<TVisualState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }


        public static Tuple<IVisualStateManager, 
            VisualStateGroup<TVisualState>, 
            VisualState<TVisualState>, 
            TElement> Target<TVisualState, TElement>(
             this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> vsmGroup,TElement element) where TVisualState : struct where TElement : View
        {
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3,element);
        }

        public static Tuple<IVisualStateManager,
    VisualStateGroup<TVisualState>,
    VisualState<TVisualState>,
    TElement> Target<TVisualState, TElement, TExistingElement,TExistingPropertyValue>(
     this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, 
         VisualStateValue<TExistingElement,TExistingPropertyValue>> vsmGroup, TElement element) 
            where TVisualState : struct 
            where TElement : View
            where TExistingElement : View
        {
            return new Tuple<IVisualStateManager, 
                VisualStateGroup<TVisualState>, 
                VisualState<TVisualState>, 
                TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }


        public static Tuple<IVisualStateManager, 
            VisualStateGroup<TVisualState>, 
            VisualState<TVisualState>, 
            VisualStateValue<TElement, TPropertyValue>> Change<TVisualState, TElement, TPropertyValue>(
             this Tuple<IVisualStateManager, 
                 VisualStateGroup<TVisualState>, 
                 VisualState<TVisualState>, 
                 TElement> vsmGroup, 
             Expression<Func<TElement, TPropertyValue>> getter, 
             Action<TElement, TPropertyValue> setter) where TVisualState : struct where TElement : View
        {
            var vsv = new VisualStateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Item4, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Item4,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, VisualStateValue<TElement, TPropertyValue>>(
                vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, 
            VisualStateValue<TElement, TPropertyValue>> ToValue<TVisualState, TElement, TPropertyValue>(
             this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>,
                 VisualState<TVisualState>, VisualStateValue<TElement, TPropertyValue>> vsmGroup, 
             TPropertyValue value) where TVisualState : struct where TElement : View
        {
            vsmGroup.Item4.StateValue = value;
            vsmGroup.Item3.Values.Add(vsmGroup.Item4);
            return vsmGroup;
        }

        public static VisualStateValue<TElement, TPropertyValue> ChangeProperty<TElement, TPropertyValue>(
            this TElement element, Expression<Func<TElement, TPropertyValue>> getter, Action<TElement, TPropertyValue> setter)
            where TElement : View
        {
            return new VisualStateValue<TElement, TPropertyValue>
            {
                Key=new Tuple<object,string>(element,(getter.Body as MemberExpression)?.Member.Name),
                Element = element,
                Getter = getter.Compile(),
                Setter = setter
            };
        }

        public static VisualStateValue<TElement, TPropertyValue> ToValue<TElement, TPropertyValue>(
            this VisualStateValue<TElement, TPropertyValue> state, TPropertyValue newValue)
            where TElement : View
        {
            state.StateValue = newValue;
            return state;
        }
    }

}