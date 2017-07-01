using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;

namespace BuildIt.States.Interfaces
{
    public interface IStateGroup:IRequiresUIAccess,
        INotifyStateChanged,
        INotifyStateChanging
    {
        event EventHandler GoToPreviousStateIsBlockedChanged;

        string GroupName { get; }

        Task<bool> ChangeTo(string newState, bool useTransitions = true);

        Task<bool> ChangeToWithData<TData>(string newState, TData data, bool useTransitions = true);

        Task<bool> ChangeBackTo(string newState, bool useTransitions = true);

        Task<bool> ChangeToPrevious(bool useTransitions = true);

        IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections=true);

        bool TrackHistory { get; set; }

        bool HasHistory { get; }

        bool GoToPreviousStateIsBlocked { get; }

        IDependencyContainer DependencyContainer { get; set; }


        IStateDefinition DefineState(IStateDefinition stateDefinition);
        IStateDefinition DefineState(string state);

        IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
            where TStateData : INotifyPropertyChanged;


        string CurrentStateName { get; }

        void WatchTrigger(IStateTrigger trigger);

        IDictionary<string, IStateDefinition> States { get; }


    }


    public interface IStateBinder
    {
        void Unbind();
    }

    
}