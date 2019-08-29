using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatefulControl
    {
        public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatefulControlStates), typeof(StatefulControl), propertyChanged: HandleStatePropertyChanged);

        private const string EmptyStateContainerName = "EmptyStateContainer";
        private const string ErrorStateContainerName = "ErrorStateContainer";
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

        public StatefulControlStates State
        {
            get => (StatefulControlStates)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
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
                    containerName = ErrorStateContainerName;
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

            statefulContainer = Children?.FirstOrDefault()?.FindByName<VisualElement>(containerName);
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