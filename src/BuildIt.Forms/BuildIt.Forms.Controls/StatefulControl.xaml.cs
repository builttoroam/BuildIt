using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildIt.Forms.Controls.Extensions;
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
        public static readonly BindableProperty PullToRefreshStateErrorTemplateProperty = BindableProperty.Create(nameof(PullToRefreshStateErrorTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandlePullToRefreshErrorStateTemplateChanged);
        public static readonly BindableProperty PullToRefreshStateTemplateProperty = BindableProperty.Create(nameof(PullToRefreshStateTemplate), typeof(DataTemplate), typeof(StatefulControl), propertyChanged: HandlePullToRefreshStateTemplateChanged);
        public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State), typeof(StatefulControlStates), typeof(StatefulControl), propertyChanged: HandleStatePropertyChanged);

        private const uint FadeInAnimationTimeInMilliseconds = 250;
        private const uint FadeOutAnimationTimeInMilliseconds = FadeInAnimationTimeInMilliseconds / 2;
        private const double FullyOpaque = 1;
        private const double FullyTransparent = 0;
        private const int MaxStatesHistoryEntries = 20;

        private readonly LinkedList<StatefulControlStates> statesHistory = new LinkedList<StatefulControlStates>();

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

        public DataTemplate PullToRefreshStateErrorTemplate
        {
            get => (DataTemplate)GetValue(PullToRefreshStateErrorTemplateProperty);
            set => SetValue(PullToRefreshStateTemplateProperty, value);
        }

        public DataTemplate PullToRefreshStateTemplate
        {
            get => (DataTemplate)GetValue(PullToRefreshStateTemplateProperty);
            set => SetValue(PullToRefreshStateTemplateProperty, value);
        }

        public StatefulControlStates State
        {
            get => (StatefulControlStates)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private PullToRefreshControl Template => template ?? (template = Children?.FirstOrDefault() as PullToRefreshControl);

        public bool CanPullToRefresh()
        {
            return State != StatefulControlStates.PullToRefresh &&
                   State != StatefulControlStates.Loading;
        }

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

        private static void HandlePullToRefreshErrorStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, StatefulControlStates.PullToRefreshError);
        }

        private static void HandlePullToRefreshStateTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CreateAndAddStateContainerTemplate(bindable, oldValue, newValue, StatefulControlStates.PullToRefresh);
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
                await statefulControl.UpdateState(statefulControl, oldState, newState);
                if (newState != StatefulControlStates.PullToRefreshError &&
                    oldState == StatefulControlStates.PullToRefresh)
                {
                    var statesHistoryCollection = statefulControl.statesHistory.ToArray();
                    // MK Skipping one, as the latest state is the new state, that occured after pull to refresh state
                    var lastNonPullToRefreshRelatedState = statesHistoryCollection.Skip(1)
                                                                                  .FirstOrDefault(s => !s.IsPullToRefreshRelated());
                    if (lastNonPullToRefreshRelatedState != StatefulControlStates.Default)
                    {
                        await statefulControl.CancelAndFinishPreviousStateUpdate(lastNonPullToRefreshRelatedState);
                    }

                    await statefulControl.StopPullToRefresh();
                }

                // MK TODO There should be also a check to make sure that we don't remove the last non pull to refresh related state, so we can cancel it when needed.
                //         If there's more than MaxStatesHistoryEntries pull to refresh related states in the list then we won't have the before state to cancel and finish
                if (statefulControl.statesHistory.Count > MaxStatesHistoryEntries)
                {
                    statefulControl.statesHistory.RemoveLast();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task CancelAndFinishPreviousStateUpdate(StatefulControlStates stateToCancelAndFinish)
        {
            var stateContainer = Template.RetrieveStatefulContainer(stateToCancelAndFinish);
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

        private async Task UpdateState(StatefulControl statefulControl, StatefulControlStates oldState, StatefulControlStates newState)
        {
            switch (newState)
            {
                case StatefulControlStates.Default:
                case StatefulControlStates.Loading:
                case StatefulControlStates.Empty:
                case StatefulControlStates.LoadingError:
                case StatefulControlStates.Loaded:
                    if (oldState.IsPullToRefreshRelated())
                    {
                        await StopPullToRefresh();
                    }

                    await CancelAndFinishPreviousStateUpdate(oldState);

                    break;

                case StatefulControlStates.PullToRefreshError:
                    var pullToRefreshErrorParentAnimation = new Animation();
                    var pullToRefreshErrorContentPresenterContainerAdjustTranslationAnimation = new Animation((y) => Template.ContentPresenterContainer.TranslationY = y, Template.ContentPresenterContainer.TranslationY, Template.PullToRefreshErrorStateContainer.Height);
                    var pullToRefreshErrorContainerFadeInAnimation = new Animation((x) => Template.PullToRefreshErrorStateContainer.Opacity = x, Template.PullToRefreshErrorStateContainer.Opacity, FullyOpaque);
                    var pullToRefreshContainerFadeAnimation = new Animation((x) => Template.PullToRefreshContainer.Opacity = x, Template.PullToRefreshContainer.Opacity, FullyTransparent, Easing.Linear, () => Template.PullToRefreshContainer.IsVisible = false);

                    pullToRefreshErrorParentAnimation.Add(0, 0.5, pullToRefreshContainerFadeAnimation);
                    pullToRefreshErrorParentAnimation.Add(0, 0.5, pullToRefreshErrorContainerFadeInAnimation);
                    pullToRefreshErrorParentAnimation.Add(0, 1, pullToRefreshErrorContentPresenterContainerAdjustTranslationAnimation);

                    pullToRefreshErrorParentAnimation.Commit(Template.PullToRefreshOuterContainer, nameof(pullToRefreshErrorParentAnimation), rate: 1, length: 150);

                    break;

                case StatefulControlStates.PullToRefresh:
                    var pullToRefreshParentAnimation = new Animation();
                    var pullToRefreshContentPresenterContainerAdjustTranslationAnimation = new Animation((y) => Template.ContentPresenterContainer.TranslationY = y, Template.ContentPresenterContainer.TranslationY, Template.PullToRefreshContainer.Height);
                    var pullToRefreshErrorContainerFadeOutAnimation = new Animation((x) => Template.PullToRefreshErrorStateContainer.Opacity = x, Template.PullToRefreshErrorStateContainer.Opacity, FullyTransparent, Easing.Linear, () => Template.PullToRefreshErrorStateContainer.IsVisible = false);

                    pullToRefreshParentAnimation.Add(0, 1, pullToRefreshErrorContainerFadeOutAnimation);
                    pullToRefreshParentAnimation.Add(0, 1, pullToRefreshContentPresenterContainerAdjustTranslationAnimation);

                    pullToRefreshParentAnimation.Commit(Template.PullToRefreshOuterContainer, nameof(pullToRefreshErrorParentAnimation), rate: 1, length: 150);

                    statefulControl.PullToRefreshCommand?.Execute(EventArgs.Empty);
                    break;
            }

            statesHistory.AddFirst(newState);

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