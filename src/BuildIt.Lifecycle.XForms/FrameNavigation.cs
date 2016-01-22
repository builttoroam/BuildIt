using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
   

    public class FrameNavigation<TState>:IHasCurrentViewModel 
        where TState : struct
    {
        public INotifyPropertyChanged CurrentViewModel => (StateNotifier as IHasCurrentViewModel)?.CurrentViewModel;


        public INotifyStateChanged<TState> StateNotifier { get; }

        private NavigationPage NavigationRoot { get; }

        public FrameNavigation(NavigationPage navigationRoot,
            INotifyStateChanged<TState> stateNotifier)
            //,string registerAs = null)
        {
            //var stateManager = hasStateManager.StateManager;
            //if (string.IsNullOrWhiteSpace( registerAs ))
            //{
            //    registerAs = hasStateManager.GetType().Name;
            //}
            //Application.Current.Resources[registerAs] = this;
            NavigationRoot = navigationRoot;

            //RootFrame.Navigated += RootFrame_Navigated;
            //RootFrame.Navigating += RootFrame_Navigating;
            NavigationRoot.Pushed += NavigationRoot_Pushed;

            //RootFrame.Tag = registerAs;
            StateNotifier = stateNotifier;
            StateNotifier.StateChanged += StateManager_StateChanged;
        }


        //private void RootFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        //{
        //}

        private void NavigationRoot_Pushed(object sender, NavigationEventArgs e)
        //private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            //var dc = e.Parameter as INotifyPropertyChanged;
            //$"Found data context? '{dc != null}'".Log();
            //if (dc == null) return;


            var pg = e.Page;
            if (pg == null) return;

            var dc = pg.BindingContext as INotifyPropertyChanged;
            $"Found data context? '{dc != null}'".Log();
            if (dc == null) return;

            var pgHasNotifier = pg as IHasStates;
            if (pgHasNotifier == null) return;



            var sm = (dc as IHasStates)?.StateManager;

           // pgHasNotifier.StateManager.Bind(sm);


            var groups = sm.StateGroups;
            var inotifier = typeof(INotifyStateChanged<>);
            var vsct = typeof(VisualStateChanger<>);
            foreach (var stateGroup in groups)
            {
                var groupNotifier = inotifier.MakeGenericType(stateGroup.Key);
                if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                {
                    var vsc = Activator.CreateInstance(vsct.MakeGenericType(stateGroup.Key), pgHasNotifier, stateGroup.Value);
                }
            }


            //var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
            //"Iterating through declared properties".Log();
            //foreach (var p in pps)
            //{
            //    var pt = p.PropertyType.GetTypeInfo();
            //    var interfaces = pt.ImplementedInterfaces;
            //    if (pt.IsInterface)
            //    {
            //        interfaces = new[] { pt.AsType() }.Union(interfaces);
            //    }
            //    "Completed interface search".Log();
            //    var ism = typeof(IStateManager<,>);
            //    //var vsct = typeof(VisualStateChanger<,>);
            //    foreach (var inf in interfaces)
            //    {
            //        $"Inspecting interface {inf.Name}".Log();
            //        if (inf.IsConstructedGenericType &&
            //            inf.GetGenericTypeDefinition() == ism)
            //        {
            //            "Interface matched, creating instance".Log();
            //            var parm = inf.GenericTypeArguments;
            //            var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), pg, p.GetValue(dc));
            //            "Instance created".Log();
            //        }
            //    }
            //}


        }

        private async void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var tp = NavigationHelper.TypeForState(e.State);

            if (e.IsNewState)
            {
                var page = Activator.CreateInstance(tp) as Page;
                page.BindingContext = CurrentViewModel;
                await NavigationRoot.Navigation.PushAsync(page);
                
            }
            else
            {
                var previous = NavigationRoot.Navigation.NavigationStack.FirstOrDefault();
                while (previous != null && previous.GetType() != tp)
                {
                    await NavigationRoot.Navigation.PopAsync();
                    previous = NavigationRoot.Navigation.NavigationStack.FirstOrDefault();
                }
                if (previous != null)
                {
                    await NavigationRoot.Navigation.PopAsync();
                }
            }

            if (NavigationRoot.Navigation.NavigationStack.Any())
            {
                NavigationPage.SetHasNavigationBar(NavigationRoot, true);
            }
            else
            {
                NavigationPage.SetHasNavigationBar(NavigationRoot, false);
            }
        }


    }


    //public interface ICodeBehindViewModel<TViewModel>
    //{
    //    Wrapper<TViewModel> Data { get; }
    //}

    //public class Wrapper<T> : NotifyBase
    //{
    //    private T viewModel;

    //    public T ViewModel
    //    {
    //        get { return viewModel; }
    //        private set
    //        {
    //            viewModel = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public Wrapper(FrameworkElement element)
    //    {
    //        element.DataContextChanged += Element_DataContextChanged;
    //    }

    //    private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    //    {
    //        if (args.NewValue is T)
    //        {
    //            ViewModel = (T)args.NewValue;
    //        }
    //        else
    //        {
    //            ViewModel = default(T);
    //        }
    //    }
    //}

}