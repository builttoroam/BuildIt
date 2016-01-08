using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public class StateGroup<TState, TTransition> :  IStateGroupManager<TState,TTransition>
        where TState : struct
        where TTransition : struct
    {

        public event EventHandler<StateEventArgs<TState>> StateChanged;

        public TState CurrentState { get; private set; }

        public IDictionary<TState, IStateDefinition<TState>> States { get; } =
            new Dictionary<TState, IStateDefinition<TState>>();

        private IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }
            = new Dictionary<Tuple<object, string>, IDefaultValue>();

        public bool TrackHistory { get; set; } = false;

        public bool HasHistory
        {
            get
            {
                if(TrackHistory==false)throw new Exception("History tracking not enabled");
                return History.Count > 0;
            }
        }

        private Stack<TState> History { get; } = new Stack<TState>(); 

        private IStateDefinition<TState> State<TFindState>(TFindState state)
            where TFindState : struct
        {
            if (typeof(TFindState) != typeof(TState)) return null;
            var searchState = (TState)(object)state;
            return States.SafeValue(searchState);
        }

        //public void TransitionTo<TFindState>(TFindState state) where TFindState : struct
        //{
        //    var visualState = State(state);
        //    visualState?.TransitionTo(DefaultValues);
        //}

        public void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineState((TState)enumVal);
            }
        }

        public virtual IStateDefinition<TState> DefineState(TState state)
        {
            var stateDefinition = new StateDefinition<TState> { State = state };
            return DefineState(stateDefinition);
        }

        public virtual IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition)
        {
            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.State] = stateDefinition;
            return stateDefinition;
        }

      


        public async Task<bool> ChangeTo<TFindState>(TFindState findState, bool useTransitions = true)
            where TFindState : struct
        {
            if (typeof (TFindState) != typeof (TState))
            {
                $"Attempting to change to the wrong state type {typeof (TFindState)}".Log();
                return false;
            }

            var newState = (TState) (object) findState;

            return await ChangeTo(newState, useTransitions);
        }

        public async Task<bool> ChangeBackTo<TFindState>(TFindState findState, bool useTransitions = true)
            where TFindState : struct
        {
            if(TrackHistory==false)throw new Exception("History tracking not enabled");

            if (typeof(TFindState) != typeof(TState))
            {
                $"Attempting to change to the wrong state type {typeof(TFindState)}".Log();
                return false;
            }

            var newState = (TState)(object)findState;

            return await ChangeBackTo(newState, useTransitions);
        }

        public async Task<bool> ChangeToPrevious(bool useTransitions = true)
        {
            if (TrackHistory == false) throw new Exception("History tracking not enabled");

            if (History.Count == 0) return false;

            var previous = History.Peek();

            return await ChangeBackTo(previous, useTransitions);
        }

        public IStateBinder Bind(IStateGroup groupToBindTo)
        {
            var sg = groupToBindTo as INotifyStateChanged<TState>;
            if (sg == null) return null;

            return new StateGroupBinder<TState>(this,sg);
        }

        private class StateGroupBinder<TStateBind> : IStateBinder
            where TStateBind : struct
        {
            IStateGroup StateGroup { get; }
            INotifyStateChanged<TStateBind> Source { get; }
            public StateGroupBinder(IStateGroup sg, INotifyStateChanged<TStateBind> source)
            {
                StateGroup = sg;
                Source = source;

                Source.StateChanged += Source_StateChanged;
            }

            private void Source_StateChanged(object sender, StateEventArgs<TStateBind> e)
            {
                if (e.IsNewState)
                {
                    StateGroup.ChangeTo(e.State, e.UseTransitions);
                }
                else
                {
                    StateGroup.ChangeBackTo(e.State, e.UseTransitions);
                }
            }

            public void Unbind()
            {
                Source.StateChanged -= Source_StateChanged;
            }
        }

        public async Task<bool> ChangeTo(TState newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, true, useTransitions);
        }

        public async Task<bool> ChangeBackTo(TState newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, false, useTransitions);
        }

        private async Task<bool> ChangeTo(TState newState, bool isNewState, bool useTransitions)
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
                var proceed = await ChangeToState(current, newState, isNewState);
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

            if (isNewState)
            {
                var oldState = CurrentState;
                CurrentState = newState;
                if (TrackHistory && !oldState.Equals(default(TState)))
                {
                    History.Push(oldState);
                }
            }
            else
            {
                while (History.Count > 0)
                {
                    var historyState = History.Pop();
                    if (!historyState.Equals(newState)) continue;
                    CurrentState = newState;
                    break;
                }
                if (!CurrentState.Equals(newState))
                {
                    $"Unable to go back to {newState} as it doesn't exist in the state History".Log();
                    return false;
                }
            }
            $"CurrentState updated (now: {CurrentState})".Log();

            States[CurrentState].TransitionTo(DefaultValues);

            await NotifyStateChanged(newState, useTransitions, isNewState);
            
            await ChangedToState(newState);



            "ChangeTo completed".Log();
            return true;
        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(TState newState, bool useTransitions, bool isNewState)
#pragma warning restore 1998
        {
            try
            {
                if (StateChanged != null)
                {
                    "Invoking StateChanged event".Log();
                    StateChanged.Invoke(this, new StateEventArgs<TState>(CurrentState, useTransitions, isNewState));
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
        protected virtual async Task<bool> ChangeToState(TState oldState, TState newState, bool isNewState)
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


        public IDictionary<TTransition, ITransitionDefinition<TState>> Transitions { get; set; } =
           new Dictionary<TTransition, ITransitionDefinition<TState>>();


        public virtual ITransitionDefinition<TState> DefineTransition(TTransition transition)
        {
            var transitionDefinition = new TransitionDefinition<TState>();
            Transitions[transition] = transitionDefinition;
            $"Defining transition of type {transition.GetType().Name}".Log();
            return transitionDefinition;
        }

        protected virtual ITransitionDefinition<TState> CreateDefaultTransition()
        {
            return new TransitionDefinition<TState>();
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

        protected virtual async Task ArrivedState(ITransitionDefinition<TState> transition, TState currentState)
        {
            if (transition.ArrivedState != null)
            {
                await transition.ArrivedState(currentState);
            }
        }

        protected virtual async Task LeavingState(ITransitionDefinition<TState> transition, TState currentState, CancelEventArgs cancel)
        {
            if (transition.LeavingState != null)
            {
                await transition.LeavingState(currentState, cancel);
            }
        }

        protected virtual async Task ArrivingState(ITransitionDefinition<TState> transition)
        {
            if (transition.ArrivingState != null)
            {
                await transition.ArrivingState(transition.EndState);
            }
        }

    }
}