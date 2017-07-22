using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BuildIt.States.Interfaces;

namespace BuildIt.States.Typed
{
    /// <summary>
    /// Definition of a typed state group
    /// </summary>
    /// <typeparam name="TState">The state type</typeparam>
    /// <typeparam name="TTypedStateDefinition">The type of the state definitions to create</typeparam>
    public class TypedStateGroupDefinition<TState, TTypedStateDefinition>
        : BaseStateGroupDefinition<TTypedStateDefinition>, ITypedStateGroupDefinition<TState, TTypedStateDefinition>
        where TTypedStateDefinition : class, ITypedStateDefinition<TState>, new()
    {
        /// <summary>
        /// Gets the name of the state group (based on enum type)
        /// </summary>
        public override string GroupName => typeof(TState).Name;

        /// <summary>
        /// Gets the typed state definitions
        /// </summary>
        public IReadOnlyDictionary<TState, ITypedStateDefinition<TState>> TypedStates => (from s in States
                                                                                          let ts = s.Value as ITypedStateDefinition<TState>
                                                                                          select new
                                                                                          {
                                                                                              Key = ts.State,
                                                                                              Value = ts
                                                                                          })
            .ToDictionary(x => x.Key, y => y.Value);

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public TTypedStateDefinition DefineTypedState(TState state)
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState)?.Equals(state) ?? false)
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            var stateDefinition = CreateDefinitionFromState(state); // new TypedStateDefinition<TState>(state);
            $"Defined state for {state}".Log();
            return DefineTypedState(stateDefinition);
        }

        /// <summary>
        /// Defines a new typed state definition based on an enum value, with data
        /// </summary>
        /// <typeparam name="TStateData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public virtual ITypedStateDefinitionWithData<TState, TTypedStateDefinition, TStateData> DefineTypedStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState)?.Equals(state) ?? false)
            {
                "Attempted to add the default state definition".Log();
                return null;
            }

            $"Defining state for {typeof(TState).Name} with data type {typeof(TStateData)}".Log();
            var stateDefinition = CreateDefinitionFromStateWithData(state, new StateDefinitionTypedDataWrapper<TStateData>()); // new TypedStateDefinition<TState>(state)
            // stateDefinition.UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>();
            var stateDef = DefineTypedState(stateDefinition);
            return new TypedStateDefinitionWithDataWrapper<TState, TTypedStateDefinition, TStateData> { State = stateDef };
        }

        /// <summary>
        /// Whether the supplied enum value is the default state
        /// </summary>
        /// <param name="enumDef">The type (enum) of the state group</param>
        /// <returns>True if it is the default state</returns>
        public virtual bool IsDefaultState(TState enumDef)
        {
            return default(TState)?.Equals(enumDef) ?? false;
        }

        /// <summary>
        /// Create a new state definition based on a state
        /// </summary>
        /// <param name="state">The state</param>
        /// <returns>The new state definition</returns>
        public virtual TTypedStateDefinition CreateDefinitionFromState(TState state)
        {
            var stateDef = CreateDefinition();
            stateDef.State = state;
            return stateDef;
        }

        /// <summary>
        /// Create a new state definition based on a state
        /// </summary>
        /// <typeparam name="TData">The type of the data</typeparam>
        /// <param name="state">The state</param>
        /// <param name="dataWrapper">The data wrapper</param>
        /// <returns>The new state definition</returns>
        public virtual TTypedStateDefinition CreateDefinitionFromStateWithData<TData>(TState state, IStateDefinitionTypedDataWrapper<TData> dataWrapper)
            where TData : INotifyPropertyChanged
        {
            var stateDef = CreateDefinitionFromState(state);
            stateDef.UntypedStateDataWrapper = dataWrapper;
            return stateDef;
        }
    }
}