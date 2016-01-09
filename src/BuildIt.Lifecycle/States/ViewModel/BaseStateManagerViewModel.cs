using System;
using Autofac;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseStateManagerViewModel : BaseViewModel, IHasStates
    {
        public IStateManager StateManager { get; } = new StateManager();

        public override void RegisterForUIAccess(IUIExecutionContext context)
        {
            base.RegisterForUIAccess(context);

            foreach (var stateGroup in StateManager.StateGroups)
            {
                (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
            }
        }

        public override void RegisterDependencies(IContainer container)
        {
            base.RegisterDependencies(container);
            foreach (var stateGroup in StateManager.StateGroups)
            {
                (stateGroup.Value as ICanRegisterDependencies)?.RegisterDependencies(container);
            }
        }

    }


    public class BaseStateManagerViewModelWithCompletion<TCompletion> : BaseStateManagerViewModel, IViewModelCompletion<TCompletion>
        where TCompletion : struct
    {
        public event EventHandler<CompletionEventArgs<TCompletion>> Complete;

        protected void OnComplete(TCompletion completion)
        {
            Complete?.Invoke(this, new CompletionEventArgs<TCompletion> { Completion = completion });
        }
        protected void OnCompleteWithData<TData>(TCompletion completion,TData data)
        {
            Complete?.Invoke(this, new CompletionWithDataEventArgs<TCompletion,TData>{ Completion = completion,Data=data });
        }
    }
}