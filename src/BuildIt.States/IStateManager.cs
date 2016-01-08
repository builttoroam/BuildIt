using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IStateManager//:IRegisterForUIAccess
    {
        IDictionary<Type, IStateGroup> StateGroups { get; }

        Task<bool> GoToState<TState>(TState state, bool animate = true) where TState : struct;
        Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct;

        Task<bool> GoBackToPreviousState(bool animate = true);

        IStateBinder Bind(IStateManager managerToBindTo);
    }
}