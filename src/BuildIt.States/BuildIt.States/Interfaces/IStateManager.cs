using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface IStateManager//:IRegisterForUIAccess
    {
        event EventHandler GoToPreviousStateIsBlockedChanged;

        IReadOnlyDictionary<Type, IStateGroup> StateGroups { get; }

        IEnumStateGroup<TState> EnumStateGroup<TState>() where TState : struct;

        void AddStateGroup<TState>(IEnumStateGroup<TState> group)
            where TState : struct;

        void AddStateGroup(Type state, IStateGroup group);

        TState CurrentState<TState>()
            where TState : struct;

        Task<bool> GoToState<TState>(TState state, bool animate = true) where TState : struct;
        Task<bool> GoToStateWithData<TState,TData>(TState state, TData data, bool animate = true) where TState : struct;
        Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct;

        Task<bool> GoBackToPreviousState(bool animate = true);

        bool PreviousStateExists { get; }

        bool GoToPreviousStateIsBlocked { get; }

        IStateBinder Bind(IStateManager managerToBindTo, bool bothDirections=true);
    }
}