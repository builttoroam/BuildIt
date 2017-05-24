using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms.Core
{
    public class VisualStateManager
    {
        public static void GoToState(Element element, string stateName)
        {
            var groups = GetVisualStateGroups(element);
            var state = (from g in groups
                         from s in g
                         where s.Name == stateName
                         select new { Group = g, State = s }).FirstOrDefault();

            if (state == null)
            {
                var cvv = element as ContentView;
                foreach (var child in cvv.Children)
                {
                    var gps = GetVisualStateGroups(child);
                    state = (from g in gps
                             from s in g
                             where s.Name == stateName
                             select new { Group = g, State = s }).FirstOrDefault();
                    if (state != null) break;
                }
            }

            if (state == null || state.Group?.CurrentState == state.State) return;
            foreach (var setter in state.State.Setters)
            {
                var target = setter.Target.Split('.');
                var name = target.FirstOrDefault();
                var prop = target.Skip(1).FirstOrDefault();
                var setterTarget = element.FindByName<Element>(name);
                if (setterTarget == null)
                {
                    var cv = element as ContentView;
                    foreach (var child in cv.Children)
                    {
                        setterTarget = child.FindByName<Element>(name);
                        if (setterTarget != null)
                        {
                            break;
                        }
                    }
                }

                var targetProp = element.GetType().GetProperty(prop);
                var targetType = targetProp.PropertyType;
                var val = (object)setter.Value;
                if (targetType != typeof(string))
                {
                    var converterType = targetType.GetTypeInfo()
                        .GetCustomAttribute<TypeConverterAttribute>(true)
                        ?.ConverterTypeName;
                    var converter = Activator.CreateInstance(Type.GetType(converterType)) as TypeConverter;
                    if (!converter?.CanConvertFrom(typeof(string)) ?? false) return;
                    val = converter.ConvertFromInvariantString((string)val);
                }

                targetProp?.SetValue(setterTarget, val);
            }

            state.Group.CurrentState = state.State;
        }
        /*
         * internal static readonly BindablePropertyKey TriggersPropertyKey = 
         * BindableProperty.CreateReadOnly(
         *      "Triggers", 
         *      typeof(IList<TriggerBase>), 
         *      typeof(VisualElement), 
         *      null, 
         *      BindingMode.OneWayToSource, 
         *      null, null, 
         *      null, null, 
         *      new BindableProperty.CreateDefaultValueDelegate(VisualElement.<>c.<>9.<.cctor>b__214_5));

         * 
         * public IList<TriggerBase> Triggers
{
	get
	{
		return (IList<TriggerBase>)base.GetValue(VisualElement.TriggersProperty);
	}
}
         * 
         * 
         */


        public static readonly BindableProperty VisualStateGroupsProperty =
            BindableProperty.CreateAttached("VisualStateGroups", typeof(IList<VisualStateGroup>),
                typeof(VisualStateManager), null, BindingMode.OneWayToSource, null, StateGroupsChanged, null, null, CreateDefaultValue);

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<VisualStateGroup>();
        }

        private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        public static IList<VisualStateGroup> GetVisualStateGroups(BindableObject view)
        {
            return (IList<VisualStateGroup>)view.GetValue(VisualStateGroupsProperty);
        }

        public static void SetVisualStateGroups(BindableObject view, IList<VisualStateGroup> value)
        {
            view.SetValue(VisualStateGroupsProperty, value);
        }


        //public static readonly BindableProperty VisualStateGroupsProperty =
        //    BindableProperty.CreateAttached("VisualStateGroups", typeof(Array), 
        //        typeof(VisualStateManager),null,BindingMode.OneWay,null,StateGroupsChanged);

        //private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        //{
        //}

        //public static Array GetVisualStateGroups(BindableObject view)
        //{
        //    return (Array)view.GetValue(VisualStateGroupsProperty);
        //}

        //public static void SetVisualStateGroups(BindableObject view, Array value)
        //{
        //    view.SetValue(VisualStateGroupsProperty, value);
        //}
    }

    public class VisualStateGroup : List<VisualState>
    {

        public string Name { get; set; }

        public VisualState CurrentState { get; set; }



        public VisualStateGroup()
        {

        }

    }

    public class VisualState : BindableObject
    {
        public string Name { get; set; }
        public static readonly BindableProperty SettersProperty =
             BindableProperty.CreateAttached("Setters", typeof(IList<Setter>),
                 typeof(VisualStateManager), null, BindingMode.OneWayToSource, null, SettersChanged, null, null, CreateDefaultValue);

        private static void SettersChanged(BindableObject bindable, object oldvalue, object newvalue)
        {

        }

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<Setter>();
        }

        public IList<Setter> Setters
        {
            get
            {
                return (IList<Setter>)base.GetValue(VisualState.SettersProperty);
            }
        }
    }

    public class Setter
    {
        public string Value { get; set; }

        public string Target { get; set; }
    }


    /*
     *  <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="States">
                <VisualState x:Name="Test">
                    <VisualState.Setters>
                        <Setter Value="Red"
                                Target="BG.(Grid.Background)"/>
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>
     * 
     */
}
