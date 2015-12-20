using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public class BaseStateManager<TState, TTransition> :
        NotifyBase, IStateManager<TState, TTransition>
        where TState : struct
        where TTransition : struct
    {
        public event EventHandler<StateEventArgs<TState>> StateChanged;

        public TState CurrentState { get; private set; }

        public IDictionary<TState, IStateDefinition<TState>> States { get; set; } = new Dictionary<TState, IStateDefinition<TState>>();

        public IDictionary<TTransition, ITransitionDefinition<TState>> Transitions { get; set; } = new Dictionary<TTransition, ITransitionDefinition<TState>>();

        public void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof (TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineState((TState) enumVal);
            }
        }

        public virtual IStateDefinition<TState> DefineState(TState state)
        {
            var stateDefinition = new BaseStateDefinition<TState> { State = state };
            return DefineState(stateDefinition);
        }

        public virtual IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition)
        {
            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.State] = stateDefinition;
            return stateDefinition;
        }

        public virtual ITransitionDefinition<TState> DefineTransition(TTransition transition)
        {
            var transitionDefinition = new BaseTransitionDefinition<TState>();
            Transitions[transition] = transitionDefinition;
            $"Defining transition of type {transition.GetType().Name}".Log();
            return transitionDefinition;
        }

        public async Task<bool> ChangeTo(TState newState, bool useTransitions = true)
        {
            $"Changing to state {newState} ({useTransitions})".Log();
            var current = CurrentState;
            if (current.Equals(newState))
            {
                "Transitioning to same state - doing nothing".Log();
                return true;
            }


            if (!current.Equals(default(TState)))
            {
                $"Current state is {current}".Log();
                var currentStateDef = States[current];
                var cancel = new CancelEventArgs();
                if (currentStateDef.AboutToChangeFrom != null)
                {
                    "Invoking 'AboutToChangeFrom' on current state definition".Log();
                    await currentStateDef.AboutToChangeFrom(cancel);
                    "'AboutToChangeFrom' completed".Log();
                }
                if (cancel.Cancel)
                {
                    "Cancelling state transition invoking 'AboutToChangeFrom'".Log();
                    return false;
                }
            }


            try
            {
                "Invoking internal ChangeToState to perform state change".Log();
                var proceed = await ChangeToState(current, newState);
                if (!proceed)
                {
                    "Unable to complete ChangeToState so exiting the ChangeTo, returning false".Log();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }

            $"About to updated CurrentState (currently: {CurrentState})".Log();
            CurrentState = newState;
            $"CurrentState updated (now: {CurrentState})".Log();


            await NotifyStateChanged(newState,useTransitions);



            await ChangedToState(newState);



            "ChangeTo completed".Log();
            return true;
        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(TState newState, bool useTransitions)
#pragma warning restore 1998
        {
            try
            {
                if (StateChanged != null)
                {
                    "Invoking StateChanged event".Log();
                    StateChanged.Invoke(this, new StateEventArgs<TState>(CurrentState, useTransitions));
                    "StateChanged event completed".Log();
                }
                else
                {
                    "Nothing listening to StateChanged".Log();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as 
                // the state change has still occurred
            }

        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task<bool> ChangeToState(TState oldState, TState newState)
#pragma warning restore 1998
        {
            if (oldState.Equals(default(TState))) return true;
            var currentStateDef = States[oldState];
            if (currentStateDef.ChangingFrom != null)
            {
                "Invoking 'ChangingFrom'".Log();
                await currentStateDef.ChangingFrom();
            }


            return true;
        }


#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangedToState(TState newState)
#pragma warning restore 1998
        {

            var newStateDef = States.SafeValue<TState, IStateDefinition<TState>, IStateDefinition<TState>>(newState);
            if (newStateDef.ChangedTo != null)
            {
                $"State definition found, of type {newStateDef.GetType().Name}, invoking ChangedTo method".Log();
                await newStateDef.ChangedTo();
                "ChangedTo completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

        }


        protected virtual ITransitionDefinition<TState> CreateDefaultTransition()
        {
            return new BaseTransitionDefinition<TState>();
        }

        public async Task<bool> Transition(TState newState, bool useTransition = true)
        {
            var transition = CreateDefaultTransition();
            transition.EndState = newState;
            return await InternalTransition(transition, useTransition);
        }

        public async Task<bool> Transition(TTransition transitionDef, bool useTransition = true)
        {
            var transition = Transitions[transitionDef];
            return await InternalTransition(transition, useTransition);
        }

        private async Task<bool> InternalTransition(ITransitionDefinition<TState> transition, bool useTransition)
        {
            $"Checking initial state {CurrentState} and transition start state {transition.StartState}".Log();
            if (!transition.StartState.Equals(CurrentState) && !transition.StartState.Equals(default(TState)))
            {
                "Current state doesn't match start state of transition".Log();
                return false;
            }
            var cancel = new CancelEventArgs();
            "Invoking LeavingState".Log();
            await LeavingState(transition, CurrentState, cancel);
            "LeavingState completed".Log();

            if (cancel.Cancel)
            {
                "Transition cancelled by LeavingState".Log();
                return false;
            }

            "Invoking ArrivingState".Log();
            await ArrivingState(transition);
            "ArrivingState completed, now invoking ChangeTo".Log();
            
            if (!await ChangeTo(transition.EndState, useTransition))
            {
                "ChangeTo not completed, transition aborted".Log();
                return false;
            }

            "Invokign ArrivedState".Log();
            await ArrivedState(transition, CurrentState);
            "ArrivedState completed".Log();
            return true;
        }

        protected async virtual Task ArrivedState(ITransitionDefinition<TState> transition, TState currentState)
        {
            if (transition.ArrivedState != null)
            {
                await transition.ArrivedState(currentState);
            }
        }

        protected async virtual Task LeavingState(ITransitionDefinition<TState> transition, TState currentState, CancelEventArgs cancel)
        {
            if (transition.LeavingState != null)
            {
                await transition.LeavingState(currentState, cancel);
            }
        }

        protected async virtual Task ArrivingState(ITransitionDefinition<TState> transition)
        {
            if (transition.ArrivingState != null)
            {
                await transition.ArrivingState(transition.EndState);
            }
        }
    }
}