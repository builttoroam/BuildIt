using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class EnumStateGroup<TState> : StateGroup, IEnumStateGroup<TState>
        where TState : struct

    {

#pragma warning disable CS0067 // See TODO
        // TODO: Raise events at correct point when changing state
        public event EventHandler<EnumStateEventArgs<TState>> EnumStateChanged;
        public event EventHandler<EnumStateCancelEventArgs<TState>> EnumStateChanging;
#pragma warning restore CS0067
        public override string GroupName => typeof(TState).Name;

        public IReadOnlyDictionary<TState, IEnumStateDefinition<TState>> EnumStates => (from s in States
                let ekey = Utilities.EnumParse<TState>(s.Key)
                let eval = s.Value as IEnumStateDefinition<TState>
                select new { Key = ekey, Value = eval })
            .ToDictionary(x => x.Key, y => y.Value);

        public TState CurrentEnumState { get; private set; }

        public override string CurrentStateName
        {
            get => (!default(TState).Equals(CurrentEnumState))? CurrentEnumState + "":null;
            protected set => CurrentEnumState = value.EnumParse<TState>();
        }


        protected override bool IsDefaultState(IStateDefinition stateDefinition)
        {
            var enumDef = stateDefinition as IEnumStateDefinition<TState>;
            return enumDef?.EnumState.Equals(default(TState)) ?? false;
        }

        public void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineEnumState((TState)enumVal);
            }
        }

        //private IEnumStateDefinition<TState> EnumStateDefinition<TFindState>(TFindState state)
        //        where TFindState : struct
        //    {
        //        if (typeof(TFindState) != typeof(TState)) return null;
        //        var searchState = (TState)(object)state;
        //        return States.SafeValue(searchState + "") as IEnumStateDefinition<TState>;
        //    }

        public virtual IEnumStateDefinition<TState> DefineEnumState(IEnumStateDefinition<TState> stateDefinition)
        {
            return DefineState(stateDefinition) as IEnumStateDefinition<TState>;
            //if (stateDefinition == null)
            //{
            //    $"Can't define null state definition".Log();
            //    return null;
            //}

            //// Determine if this state is already defined
            //var existing = EnumStateDefinition(stateDefinition.EnumState);// EnumStates.SafeValue(stateDefinition.EnumState);
            //if (existing != null)
            //{
            //    $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
            //    return existing;
            //}

            //$"Defining state of type {stateDefinition.GetType().Name}".Log();
            //States[stateDefinition.EnumState+""] = stateDefinition;
            //return stateDefinition;
        }

        public virtual IEnumStateDefinition<TState> DefineEnumState(TState state)
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }
            var stateDefinition = new EnumStateDefinition<TState>(state);
            return DefineEnumState(stateDefinition);
        }


        public virtual IEnumStateDefinitionWithData<TState, TStateData> DefineEnumStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                "Attempted to add the default state definition".Log();
                return null;
            }


            $"Defining state for {typeof(TState).Name} with data type {typeof(TStateData)}".Log();
            var stateDefinition = new EnumStateDefinition<TState>(state)
            {
                UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>()
            };
            var stateDef = DefineEnumState(stateDefinition);
            return new EnumStateDefinitionWithDataWrapper<TState, TStateData> { State = stateDef };
        }


        public async Task<bool> ChangeToWithData<TData>(TState findState, TData data, bool useTransitions = true)
        {
            return await ChangeToWithData(findState + "", data, useTransitions);
        }

        public async Task<bool> ChangeTo(TState findState, bool useTransitions = true)
        {
            return await ChangeTo(findState + "", useTransitions);
        }



        public async Task<bool> ChangeBackTo(TState findState, bool useTransitions = true)
        {
            if (TrackHistory == false) throw new Exception("History tracking not enabled");

            return await ChangeBackTo(findState + "", useTransitions);
        }

    }
}