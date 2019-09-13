using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatefulControl
    {
        public static readonly BindableProperty EmptyStateTemplateProperty = BindableProperty.Create(nameof(EmptyStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandleEmptyStateTemplateChanged);
        public static readonly BindableProperty LoadingErrorStateTemplateProperty = BindableProperty.Create(nameof(LoadingErrorStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandleLoadingErrorStateTemplateChanged);
        public static readonly BindableProperty LoadingStateTemplateProperty = BindableProperty.Create(nameof(LoadingStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandleLoadingStateTemplateChanged);
        public static readonly BindableProperty PullToRefreshCommandProperty = BindableProperty.Create(nameof(PullToRefreshCommand), typeof(ICommand), typeof(StatefulControl));
        public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatefulControlStates), typeof(StatefulControl), propertyChanged: HandleStatePropertyChanged);

        private const uint FadeInAnimationTimeInMilliseconds = 250;
        private const uint FadeOutAnimationTimeInMilliseconds = FadeInAnimationTimeInMilliseconds / 2;
        private const double FullyOpaque = 1;
        private const double FullyTransparent = 0;

        private StatefulControlStates? currentState;

        private PullToRefreshControl template;

        public StatefulControl()
        {
            InitializeComponent();
        }

        public DataTemplate EmptyStateTemplate
        {
            get => (DataTemplate)GetValue(EmptyStateTemplateProperty);
            set => SetValue(EmptyStateTemplateProperty, value);
        }

        public DataTemplate LoadingErrorStateTemplate
        {
            get => (DataTemplate)GetValue(LoadingErrorStateTemplateProperty);
            set => SetValue(LoadingErrorStateTemplateProperty, value);
        }

        public DataTemplate LoadingStateTemplate
        {
            get => (DataTemplate)GetValue(LoadingStateTemplateProperty);
            set => SetValue(LoadingStateTemplateProperty, value);
        }

        public ICommand PullToRefreshCommand
        {
            get => (ICommand)GetValue(PullToRefreshCommandProperty);
            set => SetValue(PullToRefreshCommandProperty, value);
        }

        public StatefulControlStates State
        {
            get => (StatefulControlStates)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private PullToRefreshControl Template => template ?? (template = Children?.FirstOrDefault() as PullToRefreshControl);

        internal void HandlePullToRefreshDragGesture(float offsetTop)
        {
            Template.ContentPresenterContainer.TranslationY = offsetTop;
        }

        internal async Task StartPullToRefresh()
        {
            State = StatefulControlStates.PullToRefresh;
            await Template.ContentPresenterContainer.TranslateTo(0, Template.PullToRefreshInnerContainer.Height, easing: Easing.SpringIn);
        }

        private static void CreateAndAddStateContainerTemplate(BindableObject bindable, object oldValue, object newValue, StatefulControlStates state)
        {
            var statefulControl = bindable as StatefulControl;
            var layout = statefulControl?.Template.RetrieveStatefulContainer(state) as Layout;
            if (layout == null || !(newValue is DataTemplate newDataTemplate))
            {
                return;
            }

            var layoutChildren = layout.Children as ICollection<Element>;
            if (layoutChildren == null)
            {
                return;
            }

            if (oldValue is DataTemplate || layout.Children.Any())
            {
                layoutChildren.Clear();
            }

            // TODO MK Might think about deferring creation of this view somehow, as the control might not enter a particular state at all in the entire life of the app.
            var newDataTemplateView = newDataTemplate.CreateContent() as View;
            if (newDataTemplateView == null)
            {
                return;
            }

            layoutChildren.Add(newDataTemplateView);
        }

        private static void HandleEmptyStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, StatefulControlStates.Empty);
        }

        private static void HandleLoadingErrorStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, StatefulControlStates.LoadingError);
        }

        private static void HandleLoadingStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, StatefulControlStates.Loading);
        }

        private static async void HandleStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var statefulControl = bindable as StatefulControl;
            if (statefulControl == null)
            {
                return;
            }

            var newValueString = newValue?.ToString();
            var oldValueString = oldValue?.ToString();
            if (string.IsNullOrWhiteSpace(newValueString) || !Enum.TryParse(newValueString, true, out StatefulControlStates newState) ||
                string.IsNullOrWhiteSpace(oldValueString) || !Enum.TryParse(oldValueString, true, out StatefulControlStates oldState))
            {
                return;
            }

            try
            {
                await statefulControl.UpdateState(newState);
                if (newState == StatefulControlStates.Loaded && oldState == StatefulControlStates.PullToRefresh)
                {
                    await statefulControl.StopPullToRefresh();
                }

                if (newState == StatefulControlStates.PullToRefresh)
                {
                    statefulControl.PullToRefreshCommand?.Execute(EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task CancelAndFinishPreviousStateUpdate()
        {
            if (!currentState.HasValue)
            {
                return;
            }

            var stateContainer = Template.RetrieveStatefulContainer(currentState.Value);
            if (stateContainer == null)
            {
                return;
            }

            ViewExtensions.CancelAnimations(stateContainer);

            await stateContainer.FadeTo(FullyTransparent, FadeOutAnimationTimeInMilliseconds);
            stateContainer.IsVisible = false;
        }

        private async Task StopPullToRefresh()
        {
            await Template.ContentPresenterContainer.TranslateTo(0, 0);
        }

        private async Task UpdateState(StatefulControlStates newState)
        {
            if (currentState == newState)
            {
                return;
            }

            if (newState != StatefulControlStates.PullToRefresh)
            {
                await CancelAndFinishPreviousStateUpdate();
            }

            currentState = newState;

            var statefulContainer = Template.RetrieveStatefulContainer(newState);
            if (statefulContainer == null)
            {
                return;
            }

            statefulContainer.IsVisible = true;
            await statefulContainer.FadeTo(FullyOpaque, FadeInAnimationTimeInMilliseconds);
        }
    }
}