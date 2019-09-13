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
        public static readonly BindableProperty LoadingStateTemplateProperty = BindableProperty.Create(nameof(LoadingStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandleLoadingStateTemplateChanged);
        public static readonly BindableProperty LoadingErrorStateTemplateProperty = BindableProperty.Create(nameof(LoadingErrorStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandleLoadingErrorStateTemplateChanged);
        public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatefulControlStates), typeof(StatefulControl), propertyChanged: HandleStatePropertyChanged);
        public static readonly BindableProperty PullToRefreshCommandProperty = BindableProperty.Create(nameof(PullToRefreshCommand), typeof(ICommand), typeof(StatefulControl));

        private const string EmptyStateContainerName = "EmptyStateContainer";
        private const string LoadingErrorStateContainerName = "LoadingErrorStateContainer";
        private const uint FadeInAnimationTimeInMilliseconds = 250;
        private const uint FadeOutAnimationTimeInMilliseconds = FadeInAnimationTimeInMilliseconds / 2;
        private const double FullyOpaque = 1;
        private const double FullyTransparent = 0;
        private const string LoadingStateContainerName = "LoadingStateContainer";

        private readonly IDictionary<string, VisualElement> statefulContainersByName = new Dictionary<string, VisualElement>();

        private StatefulControlStates? currentState;

        public StatefulControl()
        {
            InitializeComponent();
        }

        public DataTemplate EmptyStateTemplate
        {
            get => (DataTemplate)GetValue(EmptyStateTemplateProperty);
            set => SetValue(EmptyStateTemplateProperty, value);
        }

        public DataTemplate LoadingStateTemplate
        {
            get => (DataTemplate)GetValue(LoadingStateTemplateProperty);
            set => SetValue(LoadingStateTemplateProperty, value);
        }

        public DataTemplate LoadingErrorStateTemplate
        {
            get => (DataTemplate)GetValue(LoadingErrorStateTemplateProperty);
            set => SetValue(LoadingErrorStateTemplateProperty, value);
        }

        public StatefulControlStates State
        {
            get => (StatefulControlStates)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public ICommand PullToRefreshCommand
        {
            get => (ICommand)GetValue(PullToRefreshCommandProperty);
            set => SetValue(PullToRefreshCommandProperty, value);
        }

        private static void HandleEmptyStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, EmptyStateContainerName);
        }

        private static void HandleLoadingStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, LoadingStateContainerName);
        }

        private static void HandleLoadingErrorStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, LoadingErrorStateContainerName);
        }

        private static void CreateAndAddStateContainerTemplate(BindableObject bindable, object oldValue, object newValue, string stateContainerName)
        {
            var statefulControl = bindable as StatefulControl;
            var container = statefulControl?.RetrieveStatefulContainer(stateContainerName) as Grid;
            if (container == null || !(newValue is DataTemplate newDataTemplate))
            {
                return;
            }

            if (oldValue is DataTemplate || container.Children.Any())
            {
                container.Children.Clear();
            }

            // TODO MK Might think about deferring creation of this view somehow, as the control might not enter a particular state at all in the entire life of the app.
            var newDataTemplateView = newDataTemplate.CreateContent() as View;
            if (newDataTemplateView == null)
            {
                return;
            }

            container.Children.Add(newDataTemplateView);
        }

        private static async void HandleStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var statefulControl = bindable as StatefulControl;
            if (statefulControl == null)
            {
                return;
            }

            var newValueString = newValue?.ToString();
            if (string.IsNullOrWhiteSpace(newValueString) || !Enum.TryParse(newValueString, true, out StatefulControlStates newState))
            {
                return;
            }

            try
            {
                await statefulControl.UpdateState(newState);
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

            var stateContainer = RetrieveStatefulControlTemplateStateContainer(currentState.Value);
            if (stateContainer == null)
            {
                return;
            }

            ViewExtensions.CancelAnimations(stateContainer);

            await stateContainer.FadeTo(FullyTransparent, FadeOutAnimationTimeInMilliseconds);
            stateContainer.IsVisible = false;
        }

        private string RetrieveStatefulContainerName(StatefulControlStates state)
        {
            var containerName = string.Empty;
            switch (state)
            {
                case StatefulControlStates.Loading:
                    containerName = LoadingStateContainerName;
                    break;

                case StatefulControlStates.Empty:
                    containerName = EmptyStateContainerName;
                    break;

                case StatefulControlStates.LoadingError:
                    containerName = LoadingErrorStateContainerName;
                    break;
            }

            return containerName;
        }

        private VisualElement RetrieveStatefulControlTemplateStateContainer(StatefulControlStates state)
        {
            var containerName = RetrieveStatefulContainerName(state);
            if (string.IsNullOrWhiteSpace(containerName))
            {
                return null;
            }

            if (statefulContainersByName.TryGetValue(containerName, out var statefulContainer))
            {
                return statefulContainer;
            }

            return RetrieveStatefulContainer(containerName);
        }

        private VisualElement RetrieveStatefulContainer(string containerName)
        {
            var statefulContainer = Children?.FirstOrDefault()?.FindByName<VisualElement>(containerName);
            if (statefulContainer != null)
            {
                statefulContainersByName[containerName] = statefulContainer;
            }

            return statefulContainer;
        }

        private async Task UpdateState(StatefulControlStates newState)
        {
            if (currentState == newState)
            {
                return;
            }

            await CancelAndFinishPreviousStateUpdate();

            currentState = newState;

            var statefulContainer = RetrieveStatefulControlTemplateStateContainer(newState);
            if (statefulContainer == null)
            {
                return;
            }

            statefulContainer.IsVisible = true;
            await statefulContainer.FadeTo(FullyOpaque, FadeInAnimationTimeInMilliseconds);
        }
    }
}