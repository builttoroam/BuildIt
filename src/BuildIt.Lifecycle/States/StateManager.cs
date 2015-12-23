using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle.States
{
    public class StateManager : IStateManager, ICanRegisterDependencies
    {
        public IDictionary<Type, IStateGroup> StateGroups { get; } =
            new Dictionary<Type, IStateGroup>();

        public async Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct
        {
            IStateGroup group = StateGroups.SafeValue(state.GetType());
            return await group?.ChangeTo(state,animate);
        }

        public void RegisterDependencies(IContainer container)
        {
            foreach (var stateGroup in StateGroups)
            {
                (stateGroup.Value as ICanRegisterDependencies)?.RegisterDependencies(container);
            }
        }

        public IUIExecutionContext UIContext { get; set; } 
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;

            foreach (var stateGroup in StateGroups)
            {
                (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
            }
        }
    }
}