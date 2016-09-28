using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IViewModelStateGroup<TState> : IStateGroup<TState>, IHasCurrentViewModel
        where TState : struct
    {
        IViewModelStateDefinition<TState, TViewModel> DefineViewModelState<TViewModel>(TState state)
            where TViewModel : INotifyPropertyChanged;

        IViewModelStateDefinition<TState, TViewModel> DefineViewModelState<TViewModel>(
            IViewModelStateDefinition<TState, TViewModel> stateDefinition)
            where TViewModel : INotifyPropertyChanged;
    }


    public class ViewModelStateGroup<TState> :
        StateGroup<TState>, 
        ICanRegisterDependencies,
        IViewModelStateGroup<TState>,
        IRegisterForUIAccess
        where TState : struct
        
    {
        public ViewModelStateGroup()
        {
            TrackHistory = true;
        }


        private const string ErrorDontUseDefineState =
            "Use DefineViewModelState instead of DefineState for ViewModelStateManager";
        public override IStateDefinition<TState> DefineState(TState state)
        {
            throw new Exception(ErrorDontUseDefineState);
        }

        public override IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition)
        {
            if (stateDefinition.GetType().GetGenericTypeDefinition() != typeof(ViewModelStateDefinition<,>)) throw new Exception(ErrorDontUseDefineState);
            return base.DefineState(stateDefinition);
        }

        public IViewModelStateDefinition<TState,TViewModel> DefineViewModelState<TViewModel>(TState state) where TViewModel : INotifyPropertyChanged//, new()
        {
            var stateDefinition = new ViewModelStateDefinition<TState, TViewModel> { State = state };
            return DefineViewModelState(stateDefinition);
        }

        public IViewModelStateDefinition<TState,TViewModel> DefineViewModelState<TViewModel>(IViewModelStateDefinition<TState, TViewModel> stateDefinition) where TViewModel : INotifyPropertyChanged//, new()
        {
            $"Defining state for {typeof(TState).Name} with ViewModel type {typeof(TViewModel)}".Log();
            base.DefineState(stateDefinition);
            return stateDefinition;
        }

        #region Migrated to StateGroup

        private IDictionary<Type, INotifyPropertyChanged> ViewModels { get; } =
            new Dictionary<Type, INotifyPropertyChanged>();

        public INotifyPropertyChanged CurrentViewModel { get; set; }

        public INotifyPropertyChanged Existing(Type viewModelType)
        {
            if (viewModelType == null) return null;

            INotifyPropertyChanged existing;
            ViewModels.TryGetValue(viewModelType, out existing);
            return existing;
        }

        public override bool GoToPreviousStateIsBlocked
        {
            get
            {
                var isBlocked = base.GoToPreviousStateIsBlocked;
                if (isBlocked) return true;

                // ReSharper disable once SuspiciousTypeConversion.Global 
                var isBlockable = CurrentViewModel as IIsAbleToBeBlocked;
                return isBlockable?.IsBlocked ?? false;
            }
        }

        protected override async Task<bool> ChangeToState(TState oldState, TState newState, bool isNewState)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global - NOT HELPFUL
            var aboutVM = CurrentViewModel as IAboutToLeaveViewModelState;
            var cancel = new CancelEventArgs();
            if (aboutVM != null)
            {
                "Invoking AboutToLeave".Log();
                await aboutVM.AboutToLeave(cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by AboutToLeave".Log();
                    return false;
                }
            }

            "Retrieving current state definition".Log();
            var currentVMStates = !oldState.Equals(default(TState)) ? States[oldState] as IGenerateViewModel : null;
            if (currentVMStates != null)
            {
                "Invoking AboutToChangeFrom for existing state definition".Log();
                await currentVMStates.InvokeAboutToChangeFromViewModel(CurrentViewModel, cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by existing state definition".Log();
                    return false;
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var leaving = CurrentViewModel as ILeavingViewModelState;
            if (leaving != null)
            {
                "Invoking Leaving on current view model".Log();
                await leaving.Leaving();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var isBlockable = CurrentViewModel as IIsAbleToBeBlocked;
            if (isBlockable != null)
            {
                "Detaching event handlers for isblocked on current view model".Log();
                isBlockable.IsBlockedChanged -= IsBlockable_IsBlockedChanged;
            }


            if (currentVMStates != null)
            {
                "Invoking ChangingFrom on current state definition".Log();
                await currentVMStates.InvokeChangingFromViewModel(CurrentViewModel);
            }

            "Invoking ChangeToState to invoke state change".Log();
            var ok = await base.ChangeToState(oldState, newState, isNewState);
            if (!ok)
            {
                "ChangeToState aborted".Log();
                return false;
            }
            INotifyPropertyChanged vm = null;
            if (!newState.Equals(default(TState)))
            {
                var current = States[newState] as IGenerateViewModel;
                var genType = current?.ViewModelType;

                "Retrieving existing ViewModel for new state".Log();
                vm = Existing(genType);
            }

            if (vm == null)
            {
                var newGen = States[newState] as IGenerateViewModel;
                if (newGen == null) return false;
                "Generating ViewModel for new state".Log();
                vm = newGen.Generate();

                "Registering dependencies".Log();
                (vm as ICanRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                await newGen.InvokeInitialiseViewModel(vm);

                //if (vm.InitialiseViewModel != null)
                //{
                //    "Initialising ViewModel".Log();
                //    await InitialiseViewModel(vm);
                //}
            }
            var requireUI = vm as IRegisterForUIAccess;
            requireUI?.RegisterForUIAccess(UIContext);

            ViewModels[vm.GetType()] = vm;
            CurrentViewModel = vm;
            isBlockable = CurrentViewModel as IIsAbleToBeBlocked;
            if (isBlockable != null)
            {
                isBlockable.IsBlockedChanged += IsBlockable_IsBlockedChanged;
            }


            return true;
        }

        private void IsBlockable_IsBlockedChanged(object sender, EventArgs e)
        {
            OnGoToPreviousStateIsBlockedChanged();
        }


        protected override async Task NotifyStateChanged(TState newState, bool useTransitions, bool isNewState)
        {
            await UIContext.RunAsync(async () =>
            {
                await base.NotifyStateChanged(newState, useTransitions, isNewState);
            });
        }
        protected override async Task ChangedToState(TState newState, string dataAsJson)
        {
            await base.ChangedToState(newState,dataAsJson);

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var arrived = CurrentViewModel as IArrivingViewModelState;
            if (arrived != null)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrived.Arriving();
            }

            var currentVMStates = States[CurrentState] as IGenerateViewModel;
            if (currentVMStates != null)
            {
                "Invoking ChangedTo on new state definition".Log();
                await currentVMStates.InvokeChangedToViewModel(CurrentViewModel);
                await currentVMStates.InvokeChangedToWithDataViewModel(CurrentViewModel, dataAsJson);
            }

        }


        protected override async Task ArrivedState(ITransitionDefinition<TState> transition, TState currentState)
        {
            await base.ArrivedState(transition, currentState);

            var trans = transition as ViewModelTransitionDefinition<TState>;
            if (trans?.ArrivedStateViewModel != null)
            {
                await trans.ArrivedStateViewModel(currentState, CurrentViewModel);
            }

        }

        protected override async Task LeavingState(ITransitionDefinition<TState> transition, TState currentState, CancelEventArgs cancel)
        {
            await base.LeavingState(transition, currentState, cancel);

            if (cancel.Cancel) return;

            var trans = transition as ViewModelTransitionDefinition<TState>;
            if (trans?.LeavingStateViewModel != null)
            {
                await trans.LeavingStateViewModel(currentState, CurrentViewModel, cancel);
            }

        }
        #endregion

        protected IContainer DependencyContainer { get; private set; }
        public void RegisterDependencies(IContainer container)
        {
            DependencyContainer = container;
            var cb = new ContainerBuilder();
            foreach (var state in States.Values.OfType<IGenerateViewModel>())
            {
                cb.RegisterType(state.ViewModelType);
            }
            cb.Update(container);
        }

        public IUIExecutionContext UIContext { get; private set; }
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;
        }
    }
}