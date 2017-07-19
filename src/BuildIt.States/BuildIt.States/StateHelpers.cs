using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.States.Completion;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.Builder;

namespace BuildIt.States
{
    /// <summary>
    /// Helper methods for building states
    /// </summary>
    public static class StateHelpers
    {
        /// <summary>
        /// Indicates whether all triggers are active
        /// </summary>
        /// <param name="state">The state definition</param>
        /// <returns>Whether all triggers are active</returns>
        public static bool AllTriggersActive(this IStateDefinition state)
        {
            if (state == null)
            {
                return false;
            }

            return state.Triggers.All(x => x.IsActive);
        }

        /// <summary>
        /// Returns a state group builder for a specific type (enum)
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the group to build</typeparam>
        /// <param name="vsm">The state manager</param>
        /// <returns>A state group builder (or null)</returns>
        public static IStateGroupBuilder<TState> Group<TState>(this IStateManager vsm)
            where TState : struct
        {
            if (vsm == null)
            {
                return null;
            }

            var existing = vsm.TypedStateGroup<TState>(); // StateGroups.SafeValue(typeof(TState)) as IStateGroup<TState>;
            if (existing == null)
            {
                existing = new EnumStateGroup<TState>();
                vsm?.AddStateGroup(existing);
            }

            return new StateGroupBuilder<TState>
            {
                StateManager = vsm,
                StateGroup = existing
            };
        }

        /// <summary>
        /// Returns a builder for the group
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <param name="vsmGroup">A state builder</param>
        /// <returns>New builder</returns>
        public static IStateGroupBuilder<TState> Group<TState>(
            this IStateBuilder vsmGroup)
            where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

            return vsmGroup.StateManager.Group<TState>();
        }

        /// <summary>
        /// Enables track history
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <param name="vsmGroup">The state group builder</param>
        /// <returns>Updated state group builder</returns>
        public static IStateGroupBuilder<TState> WithHistory<TState>(
    this IStateGroupBuilder<TState> vsmGroup)
    where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

            vsmGroup.StateGroup.TrackHistory = true;
            return vsmGroup;
        }

        /// <summary>
        /// Defines all states for a group
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the group to build</typeparam>
        /// <param name="vsmGroup">Existing group builder</param>
        /// <returns>A state group builder (or null)</returns>
        public static IStateGroupBuilder<TState> DefineAllStates<TState>(
    this IStateGroupBuilder<TState> vsmGroup)
    where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

            vsmGroup.StateGroup.TypedGroupDefinition.DefineAllStates();
            return vsmGroup;
        }

        /// <summary>
        /// Expoese a builder for the state definition
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <param name="vsmGroup">The state group builder</param>
        /// <param name="state">The state</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionBuilder<TState> DefineState<TState>(
            this IStateGroupBuilder<TState> vsmGroup,
            TState state)
            where TState : struct
        {
            var vs = vsmGroup?.StateGroup.TypedGroupDefinition.DefineTypedState(state);
            if (vs == null)
            {
                return null;
            }

            return new StateDefinitionBuilder<TState>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vs
            };
        }

        /// <summary>
        /// Adds a trigger to a state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="trigger">The trigger to add</param>
        /// <returns>The updated state definition builder</returns>
        public static IStateDefinitionBuilder<TState> AddTrigger<TState>(
            this IStateDefinitionBuilder<TState> vsmGroup,
            IStateTrigger trigger)
            where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

            if (trigger == null)
            {
                return vsmGroup;
            }

            // Add trigger to triggers collection
            vsmGroup.State.Triggers.Add(trigger);

            // Advise the state group to monitor triggers
            vsmGroup.StateGroup.WatchTrigger(trigger);
            return vsmGroup;
        }

        /// <summary>
        /// Expoese a builder for a state the completes
        /// </summary>
        /// <typeparam name="TState">The type (eum) of the state</typeparam>
        /// <typeparam name="TCompletion">The type of the completion</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completion">The completion value</param>
        /// <returns>New builder</returns>
        public static IStateCompletionBuilder<TState, TCompletion>
      OnComplete<TState, TCompletion>(
      this IStateDefinitionBuilder<TState> smInfo,
           TCompletion completion)
      where TState : struct
       where TCompletion : struct
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateCompletionBuilder<TState, TCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };
        }

        /// <summary>
        /// Exposes a builder to attach to the default completion of a state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <returns>A builder for the default completion</returns>
        public static IStateCompletionBuilder<TState, DefaultCompletion>
          OnDefaultComplete<TState>(
          this IStateDefinitionBuilder<TState> smInfo)
          where TState : struct
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateCompletionBuilder<TState, DefaultCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete
            };
        }

        /// <summary>
        /// Exposes a builder to attach to the completion of its state data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <typeparam name="TCompletion">The type of the state data completion</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completion">The completion value</param>
        /// <returns>A builder for the state data completion</returns>
        public static IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>
            OnComplete<TState, TStateData, TCompletion>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            TCompletion completion)
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateWithDataCompletionBuilder<TState, TStateData, TCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };
        }

        /// <summary>
        /// Exposes a builder for a state that has state data that completes
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <returns>New builder</returns>
        public static
           IStateWithDataCompletionBuilder<TState, TStateData, DefaultCompletion>
          OnDefaultComplete<TState, TStateData>(
          this
           IStateDefinitionWithDataBuilder<TState, TStateData> smInfo)
          where TState : struct
          where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateWithDataCompletionBuilder<TState, TStateData, DefaultCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete
            };
        }

        /// <summary>
        /// Expoese builder for a state that completes with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TCompletion">The type (enum) of the completion</typeparam>
        /// <typeparam name="TData">The type of the data to return</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completion">The completion</param>
        /// <param name="completionData">The completion data</param>
        /// <returns>New builder</returns>
        public static
           IStateCompletionWithDataBuilder<TState, TCompletion, TData>
          OnCompleteWithData<TState, TCompletion, TData>(
          this
           IStateDefinitionBuilder<TState> smInfo,
               TCompletion completion,
               Func<TData> completionData)
          where TState : struct
           where TCompletion : struct
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateCompletionWithDataBuilder<TState, TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion,
                Data = completionData
            };
        }

        /// <summary>
        /// Exposes a builder to attach to the default completion with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TData">The type of data to return from the state</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completionData">The completion data</param>
        /// <returns>The state completion with data builder</returns>
        public static IStateCompletionWithDataBuilder<TState, DefaultCompletion, TData>
          OnDefaultCompleteWithData<TState, TData>(
          this IStateDefinitionBuilder<TState> smInfo,
               Func<TData> completionData)
          where TState : struct
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateCompletionWithDataBuilder<TState, DefaultCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete,
                Data = completionData
            };
        }

        /// <summary>
        /// Exposes builder for state with state data the completes with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <typeparam name="TCompletion">The type (enum) of completion</typeparam>
        /// <typeparam name="TData">The type of the data to be returned</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completion">The completion</param>
        /// <returns>New builder</returns>
        public static IStateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
                 OnCompleteWithDataEvent<TState, TStateData, TCompletion, TData>(
                 this
                  IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                      TCompletion completion)
                 where TState : struct
                 where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion, TData>
                  where TCompletion : struct
        {
            return new StateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };
        }

        /// <summary>
        /// Exposes a builder for a state that has state data that completes with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <typeparam name="TCompletion">The type (enum) of completion</typeparam>
        /// <typeparam name="TData">The type of data to be returned from the state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completion">The completion</param>
        /// <param name="completionData">The completion data</param>
        /// <returns>New builder</returns>
        public static IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
                 OnCompleteWithData<TState, TStateData, TCompletion, TData>(
                 this
                  IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                      TCompletion completion,
                      Func<TStateData, TData> completionData)
                 where TState : struct
                 where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
                  where TCompletion : struct
        {
            return new StateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion,
                Data = completionData
            };
        }

        /// <summary>
        /// Expoeses a builder for a state with a state data that uses default completion and returns data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <typeparam name="TData">The type of data to be returned</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="completionData">The completion data</param>
        /// <returns>The new builder</returns>
        public static IStateWithDataCompletionWithDataBuilder<TState, TStateData, DefaultCompletion, TData>
          OnDefaultCompleteWithData<TState, TStateData, TData>(
          this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
               Func<TStateData, TData> completionData)
          where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        {
            if (smInfo == null)
            {
                return null;
            }

            return new StateWithDataCompletionWithDataBuilder<TState, TStateData, DefaultCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete,
                Data = completionData
            };
        }

        /// <summary>
        /// Exposes builder for the state definition target, for setting properties when switching to a state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TElement">The element to be adjusted</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="element">The element whose property is to be adjusted</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionValueTargetBuilder<TState, TElement> Target<TState, TElement>(
            this IStateDefinitionBuilder<TState> vsmGroup, TElement element)
            where TState : struct
        {
            if (vsmGroup == null || element == null)
            {
                return null;
            }

            return new StateDefinitionValueTargetBuilder<TState, TElement>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Target = element
            };
        }

        /// <summary>
        /// Returns a state definition property value builder - defines changing a property when moving to a state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TElement">The type of the element to adjust the property value on</typeparam>
        /// <typeparam name="TPropertyValue">The type of the property to adjust</typeparam>
        /// <param name="vsmGroup">The state definition value target builder</param>
        /// <param name="getter">The property getter</param>
        /// <param name="setter">The property setter</param>
        /// <returns>New builder</returns>
        public static
       IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
       Change<TState, TElement, TPropertyValue>(
       this IStateDefinitionValueTargetBuilder<TState, TElement> vsmGroup,
       Expression<Func<TPropertyValue>> getter,
       Action<TElement, TPropertyValue> setter = null)
            where TState : struct
        {
            if (vsmGroup == null || getter == null)
            {
                return null;
            }

            if (setter == null)
            {
                var propertyName = (getter.Body as MemberExpression)?.Member.Name;
                var pinfo = vsmGroup.Target.GetType().GetRuntimeProperty(propertyName);
                setter = (element, value) =>
                {
                    pinfo.SetValue(element, value);
                };
            }

            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Target, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Target,
                Getter = (vm) => getter.Compile().Invoke(),
                Setter = setter
            };
            return new StateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Value = vsv
            };
        }

        /// <summary>
        /// Exposes builder for changing property value on an element
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TElement">The type of the element to adjust</typeparam>
        /// <typeparam name="TPropertyValue">The type of the property to adjust</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="getter">Expression to the getter</param>
        /// <param name="setter">Action to set value</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            Change<TState, TElement, TPropertyValue>(
            this IStateDefinitionValueTargetBuilder<TState, TElement> vsmGroup,
            Expression<Func<TElement, TPropertyValue>> getter,
            Action<TElement, TPropertyValue> setter = null)
            where TState : struct
        {
            if (vsmGroup == null || getter == null)
            {
                return null;
            }

            if (setter == null)
            {
                var propertyName = (getter.Body as MemberExpression)?.Member.Name;
                var pinfo = vsmGroup.Target.GetType().GetRuntimeProperty(propertyName);
                setter = (element, value) =>
                {
                    pinfo.SetValue(element, value);
                };
            }

            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Target, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Target,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new StateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Value = vsv
            };
        }

        /// <summary>
        /// Exposes a builder that define the new property value
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TElement">The type of the element to adjust the property on</typeparam>
        /// <typeparam name="TPropertyValue">The type of the property to adjust</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="value">The value to set</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionBuilder<TState>
            ToValue<TState, TElement, TPropertyValue>(
            this
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> vsmGroup,
            TPropertyValue value)
            where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

            if (value == null)
            {
                return vsmGroup;
            }

            vsmGroup.Value.Value = value;
            vsmGroup.State.Values.Add(vsmGroup.Value);
            return vsmGroup;
        }

        /// <summary>
        /// Expoese a builder that changes a property value on an element
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TPropertyValue">The type of the property</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="getter">The expresion to the getter on the element</param>
        /// <param name="value">The value to set the property to</param>
        /// <returns>New builder</returns>
        public static
          IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TPropertyValue>(
          this IStateDefinitionBuilder<TState> vsmGroup,
          Expression<Func<TPropertyValue>> getter,
          TPropertyValue value)
            where TState : struct
        {
            var property = (getter.Body as MemberExpression)?.Expression as ConstantExpression;
            var element = property?.Value;

            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }

        /// <summary>
        /// Exposes a builder to change a property value
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TElement">The type of element to adjust property on</typeparam>
        /// <typeparam name="TPropertyValue">The type of property to adjust</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="element">The element to adjust</param>
        /// <param name="getter">The expression tree for the property</param>
        /// <param name="value">The value to set</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TElement, TPropertyValue>(
            this IStateDefinitionBuilder<TState> vsmGroup,
            TElement element,
            Expression<Func<TElement, TPropertyValue>> getter,
            TPropertyValue value)
            where TState : struct
        {
            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }

        /// <summary>
        /// Expoese a builder that changes a property value on an element
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TPropertyValue">The type of the property</typeparam>
        /// <param name="vsmGroup">The state definition builder</param>
        /// <param name="getter">The expresion to the getter on the element</param>
        /// <param name="value">The value to set the property to</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TPropertyValue>(
            this IStateDefinitionBuilder<TState> vsmGroup,
            Expression<Func<object, TPropertyValue>> getter,
            TPropertyValue value)
            where TState : struct
        {
            var property = (getter.Body as MemberExpression)?.Expression as ConstantExpression;
            var element = property?.Value;

            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }

        /// <summary>
        /// Defines an action to be called when AboutToChange the state, passing in the current state data and new data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when AboutToChange</param>
        /// <returns>The updated builder</returns>
        public static
            IStateDefinitionBuilder<TState> WhenAboutToChange<TState>(
           this IStateDefinitionBuilder<TState> smInfo,
           Action<CancelEventArgs> action)
            where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenAboutToChange(async cancel => action(cancel));
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an async action to be called when AboutToChange the state
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when AboutToChange</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionBuilder<TState> WhenAboutToChange<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Func<CancelEventArgs, Task> action)
            where TState : struct
        {
            if (smInfo?.State == null)
            {
                return null;
            }

            var stateDefinition = smInfo.State;

            "Adding Behaviour: AboutToChangeFrom".Log();
            stateDefinition.AboutToChangeFrom = action;
            return smInfo;
        }

        /// <summary>
        /// Defines an action to be called when ChangingFrom the state
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <param name="stateDefinition">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangingFrom</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionBuilder<TState> WhenChangingFrom<TState>(
            this IStateDefinitionBuilder<TState> stateDefinition,
            Action action)
            where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangingFrom(async () => action());
#pragma warning restore 1998
        }

        /// <summary>
        /// Define an async action to be called when ChangingFrom the state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <param name="smInfo">The builder</param>
        /// <param name="action">The action to be invoked when ChangingFrom</param>
        /// <returns>Updated builder</returns>
        public static IStateDefinitionBuilder<TState> WhenChangingFrom<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Func<Task> action)
            where TState : struct
        {
            if (smInfo?.State == null)
            {
                return null;
            }

            var stateDefinition = smInfo.State;

            "Adding Behaviour: ChangingFrom".Log();
            stateDefinition.ChangingFrom = action;
            return smInfo;
        }

        /// <summary>
        /// Defines an action to be called when ChangedTo the state
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionBuilder<TState> WhenChangedTo<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Action action)
            where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async () => action());
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an async action to be called when ChangedTo the state
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionBuilder<TState> WhenChangedTo<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Func<Task> action)
            where TState : struct
        {
            if (smInfo?.State == null)
            {
                return null;
            }

            var stateDefinition = smInfo.State;

            "Adding Behaviour: ChangedTo".Log();
            stateDefinition.ChangedTo = action;
            return smInfo;
        }

        /// <summary>
        /// Expoese a state definition builder for a state with state data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state group builder</param>
        /// <param name="state">The state to define</param>
        /// <returns>New builder</returns>
        public static
            IStateDefinitionWithDataBuilder<TState, TStateData>
           DefineStateWithData<TState, TStateData>(
                this IStateGroupBuilder<TState> smInfo,
                TState state)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            var vms = smInfo.StateGroup.TypedGroupDefinition.DefineTypedStateWithData<TStateData>(state);
            return new StateDefinitionWithDataBuilder<TState, TStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = vms.TypedState
            };
        }

        /// <summary>
        /// Exposes a builder that invokes the initialize method on a state definition
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="action">The action to run to initialize the state data</param>
        /// <returns>New builder</returns>
        public static

            IStateDefinitionWithDataBuilder<TState, TStateData>
            Initialise<TState, TStateData>(
                this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                Action<TStateData> action)
                where TState : struct
                where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return smInfo.Initialise(async vm => action(vm));
#pragma warning restore 1998
        }

        /// <summary>
        /// Initialize the state data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="action">The action to perform initializaetion</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> Initialise<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Initialization".Log();
            if (stateDefinition.Initialise == null || action == null)
            {
                stateDefinition.Initialise = action;
            }
            else
            {
                stateDefinition.Initialise += action;
            }

            return smInfo;
        }

        /// <summary>
        /// Defines an action to be called when AboutToChange the state, passing in the current state data and new data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when AboutToChange</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenAboutToChange<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData, CancelEventArgs> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return smInfo.WhenAboutToChange(async (vm, cancel) => action(vm, cancel));
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an async action to be called when AboutToChange the state, passing in the current state data and new data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when AboutToChange</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenAboutToChange<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, CancelEventArgs, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: AboutToChangeFromViewModel".Log();
            if (stateDefinition.AboutToChangeFrom == null || action == null)
            {
                stateDefinition.AboutToChangeFrom = action;
            }
            else
            {
                stateDefinition.AboutToChangeFrom += action;
            }

            return smInfo;
        }

        /// <summary>
        /// Defines an action that to be called when ChangingFrom the state, passing in the current state data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangingFrom</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangingFrom<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangingFrom(async vm => action(vm));
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an async action to be called when ChangingFrom the state, passing in the current state data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangingFrom</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangingFrom<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangingFromViewModel".Log();
            if (stateDefinition.ChangingFrom == null || action == null)
            {
                stateDefinition.ChangingFrom = action;
            }
            else
            {
                stateDefinition.ChangingFrom += action;
            }

            return smInfo;
        }

        /// <summary>
        /// Exposes builder to initialize a new state with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <typeparam name="TNewStateData">The type of the new state data</typeparam>
        /// <typeparam name="TData">The type of the data being passed in</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="action">The action to invoke to pass data into the state data</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData>
            InitializeNewState<TState, TStateData, TNewStateData, TData>(
    this IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TData> smInfo,
    Action<TNewStateData, TData> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
            where TNewStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.InitializeNewState<TState, TStateData, TNewStateData, TData>(async (vm, s) => action(vm, s));
#pragma warning restore 1998
        }

        /// <summary>
        /// Exposes builder to initialize (async) a new state with data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <typeparam name="TNewStateData">The type of the new state data</typeparam>
        /// <typeparam name="TData">The type of the data being passed in</typeparam>
        /// <param name="existingInfo">The state definition builder</param>
        /// <param name="action">The action to invoke to pass data into the state data</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData>
            InitializeNewState<TState, TStateData, TNewStateData, TData>(
            this IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TData> existingInfo,
            Func<TNewStateData, TData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
            where TNewStateData : INotifyPropertyChanged
        {
            var smInfo = existingInfo.DefineStateWithData<TState, TNewStateData>(existingInfo.NewState);

            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToWithDataViewModel".Log();

            var modAction = new Func<TNewStateData, string, Task>((vm, d) =>
            {
                var data = d.DecodeJson<TData>();
                return action(vm, data);
            });

            if (action == null)
            {
                stateDefinition.ChangedToWithData = null;
            }
            else
            {
                stateDefinition.ChangedToWithData += modAction;
            }

            return existingInfo;
        }

        /// <summary>
        /// Defines an action to be called when ChangedTo the state, passing in the current state data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedTo<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async vm => action(vm));
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an async action to be called when ChangedTo the state, passing in the current state data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedTo<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToViewModel".Log();

            if (stateDefinition.ChangedTo == null || action == null)
            {
                stateDefinition.ChangedTo = action;
            }
            else
            {
                stateDefinition.ChangedTo += action;
            }

            return smInfo;
        }

        /// <summary>
        /// Defines an action to be called when ChangedTo the state, passing in the current state data and new data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <typeparam name="TData">The type of data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedToWithData<TState, TStateData, TData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData, TData> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedToWithData<TState, TStateData, TData>(async (vm, s) => action(vm, s));
#pragma warning restore 1998
        }

        /// <summary>
        /// Defines an action to be called when ChangedTo the state, passing in the current state data and new data
        /// </summary>
        /// <typeparam name="TState">The typeo (enum) of the state group</typeparam>
        /// <typeparam name="TStateData">The type of state data that will be passed to the action</typeparam>
        /// <typeparam name="TData">The type of data that will be passed to the action</typeparam>
        /// <param name="smInfo">The builder to update</param>
        /// <param name="action">The action to be invoked when ChangedTo</param>
        /// <returns>The updated builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedToWithData<TState, TStateData, TData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, TData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null)
            {
                return null;
            }

            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToWithDataViewModel".Log();

            var modAction = new Func<TStateData, string, Task>((vm, d) =>
            {
                var data = d.DecodeJson<TData>();
                return action(vm, data);
            });

            if (action == null)
            {
                stateDefinition.ChangedToWithData = null;
            }
            else
            {
                stateDefinition.ChangedToWithData += modAction;
            }

            return smInfo;
        }

        /// <summary>
        /// Expoese builder for attaching to an event on the state data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="subscribe">The action to call when subscribing</param>
        /// <param name="unsubscribe">The action to call when unsubscribing</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataEventBuilder<TState, TStateData>
            OnEvent<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData, EventHandler> subscribe,
            Action<TStateData, EventHandler> unsubscribe)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.State == null)
            {
                return null;
            }

            return new StateDefinitionWithDataEventBuilder<TState, TStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Subscribe = subscribe,
                Unsubscribe = unsubscribe
            };
        }

        /// <summary>
        /// Exposes builder to change state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <typeparam name="TData">The type of data being passed into the new state</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="stateToChangeTo">The state to change to</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TData>
            ChangeState<TState, TStateData, TData>(
            this IStateWithDataActionDataBuilder<TState, TStateData, TData> smInfo,
            TState stateToChangeTo)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null)
            {
                return null;
            }

            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToNewState(stateToChangeTo))
                .WhenChangingFrom(smInfo.WhenChangingFromNewState(stateToChangeTo))
                .IncludeStateInit<TState, TStateData, TData>(stateToChangeTo);

            return returnd;

            // new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        /// <summary>
        /// Exposes builder for changing state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The state data</typeparam>
        /// <param name="smInfo">The state definition builder</param>
        /// <param name="stateToChangeTo">The state to change to</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeState<TState, TStateData>(
    this IStateWithDataActionBuilder<TState, TStateData> smInfo,
    TState stateToChangeTo)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null)
            {
                return null;
            }

            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToNewState(stateToChangeTo))
                .WhenChangingFrom(smInfo.WhenChangingFromNewState(stateToChangeTo));

            return returnd;
        }

        /// <summary>
        /// Exposes builder that will change to previous state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state</typeparam>
        /// <typeparam name="TStateData">The type of the state data</typeparam>
        /// <param name="smInfo">The state builder</param>
        /// <returns>New builder</returns>
        public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeToPreviousState<TState, TStateData>(
            this IStateWithDataActionBuilder<TState, TStateData> smInfo)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null)
            {
                return null;
            }

            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToPreviousState())
                .WhenChangingFrom(smInfo.WhenChangingFromPreviousState());

            return returnd;
        }

        private static IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            IncludeStateInit
            <TState, TStateData, TNewStateData>(this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo, TState newState)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            return new StateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                NewState = newState
            };
        }

        #region Builder Implementations
        private class StateBuilder : IStateBuilder
        {
            public IStateManager StateManager { get; set; }
        }

        private class StateGroupBuilder<TState> : StateBuilder, IStateGroupBuilder<TState>
            where TState : struct
        {
            public ITypedStateGroup<TState> StateGroup { get; set; }
        }

        private class StateDefinitionBuilder<TState> : StateGroupBuilder<TState>, IStateDefinitionBuilder<TState>
            where TState : struct
        {
            public ITypedStateDefinition<TState> State { get; set; }
        }

        private class StateDefinitionWithDataBuilder<TState, TData> : StateGroupBuilder<TState>,
            IStateDefinitionWithDataBuilder<TState, TData>
            where TData : INotifyPropertyChanged
            where TState : struct
        {
            public ITypedStateDefinition<TState> State { get; set; }

            public IStateDefinitionTypedDataWrapper<TData> StateDataWrapper
                => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
        }

        private class StateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData> :
            StateDefinitionWithDataBuilder<TState, TStateData>, IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            where TStateData : INotifyPropertyChanged
            where TState : struct
        {
            public TState NewState { get; set; }
        }

        private class EventHandlerCache<TState, TEventHandler>
            where TState : struct
        {
            private IDictionary<TState, TEventHandler> Handlers { get; } = new Dictionary<TState, TEventHandler>();

            public TEventHandler BuildHandler(TState newState, Func<TState, TEventHandler> createHandler)
            {
                var changeStateAction = Handlers.SafeValue(newState);
                if (changeStateAction == null)
                {
                    changeStateAction = createHandler(newState);
                    Handlers[newState] = changeStateAction;
                }

                return changeStateAction;
            }
        }

        private class StateDefinitionWithDataEventBuilder<TState, TData> : StateDefinitionWithDataBuilder<TState, TData>,
            IStateDefinitionWithDataEventBuilder<TState, TData>
            where TData : INotifyPropertyChanged
            where TState : struct
        {
            public Action<TData, EventHandler> Subscribe { get; set; }

            public Action<TData, EventHandler> Unsubscribe { get; set; }

            private EventHandlerCache<TState, EventHandler> HandlerCache { get; } = new EventHandlerCache<TState, EventHandler>();

            private EventHandler PreviousHandler { get; set; }

            public Action<TData> WhenChangedToNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create);

                return vm =>
                {
                    Subscribe(vm, changeStateAction);
                };
            }

            public Action<TData> WhenChangingFromNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create); // BuildHandler(newState);

                return vm =>
                {
                    Unsubscribe(vm, changeStateAction);
                };
            }

            public Action<TData> WhenChangedToPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return vm =>
                {
                    Subscribe(vm, changeStateAction);
                };
            }

            public Action<TData> WhenChangingFromPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return vm =>
                {
                    Unsubscribe(vm, changeStateAction);
                };
            }

            private EventHandler Create(TState newState)
            {
                EventHandler changeStateAction = (s, e) =>
                {
                    StateManager.GoToState(newState);
                };
                return changeStateAction;
            }

            private EventHandler CreatePrevious()
            {
                if (PreviousHandler != null)
                {
                    return PreviousHandler;
                }

                PreviousHandler = (s, e) =>
                {
                    StateManager.GoBackToPreviousState();
                };
                return PreviousHandler;
            }
        }

        private class StateDefinitionValueTargetBuilder<TState, TElement> :
            StateDefinitionBuilder<TState>, IStateDefinitionValueTargetBuilder<TState, TElement>
            where TState : struct
        {
            public TElement Target { get; set; }
        }

        private class StateDefinitionValueBuilder<TState, TElement, TPropertyValue> :
            StateDefinitionBuilder<TState>, IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            where TState : struct
        {
            public StateValue<TElement, TPropertyValue> Value { get; set; }
        }

        private class StateCompletionBuilder<TState, TCompletion> : StateDefinitionBuilder<TState>,
            IStateCompletionBuilder<TState, TCompletion>
            where TState : struct
            where TCompletion : struct
        {
            public TCompletion Completion { get; set; }
        }

        private class StateCompletionWithDataBuilder<TState, TCompletion, TData> :
            StateCompletionBuilder<TState, TCompletion>,
            IStateCompletionWithDataBuilder<TState, TCompletion, TData>
            where TState : struct
            where TCompletion : struct
        {
            public Func<TData> Data { get; set; }
        }

        private abstract class StateWithDataCompletionBaseBuilder<TState, TStateData, TCompletion, TCompletionArgs> :
            StateCompletionBuilder<TState, TCompletion>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged
            where TCompletionArgs : CompletionEventArgs<TCompletion>
        {
            public IStateDefinitionTypedDataWrapper<TStateData> StateDataWrapper
                => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TStateData>;

            private EventHandler<TCompletionArgs> Previous { get; set; }

            private EventHandlerCache<TState, EventHandler<TCompletionArgs>> HandlerCache { get; }
                = new EventHandlerCache<TState, EventHandler<TCompletionArgs>>();

            public Action<TStateData> WhenChangedToNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create);

                return WireHandlerNewState(changeStateAction);
                // vm =>
                // {
                //    vm.Complete += changeStateAction;
                // };
            }

            public Action<TStateData> WhenChangingFromNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create);

                return UnwireHandlerNewState(changeStateAction);
                // vm =>
                // {
                //    vm.Complete -= changeStateAction;
                // };
            }

            public Action<TStateData> WhenChangedToPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return WireHandlerPreviousState(changeStateAction);
                // vm =>
                // {
                //    vm.Complete += changeStateAction;
                // };
            }

            public Action<TStateData> WhenChangingFromPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return UnwireHandlerPreviousState(changeStateAction);

                // vm =>
                // {
                //    vm.Complete -= changeStateAction;
                // };
            }

            protected abstract Action<TStateData> WireHandlerNewState(EventHandler<TCompletionArgs> complete);

            protected abstract Action<TStateData> UnwireHandlerNewState(EventHandler<TCompletionArgs> complete);

            protected abstract Action<TStateData> WireHandlerPreviousState(EventHandler<TCompletionArgs> complete);

            protected abstract Action<TStateData> UnwireHandlerPreviousState(EventHandler<TCompletionArgs> complete);

            protected virtual EventHandler<TCompletionArgs> Create(TState newState)
            {
                EventHandler<TCompletionArgs> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToState(newState);
                    }
                };

                return changeStateAction;
            }

            protected virtual EventHandler<TCompletionArgs> InternalCreatePrevious()
            {
                EventHandler<TCompletionArgs> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoBackToPreviousState();
                    }
                };

                return changeStateAction;
            }

            private EventHandler<TCompletionArgs> CreatePrevious()
            {
                if (Previous != null)
                {
                    return Previous;
                }

                Previous = InternalCreatePrevious();
                return Previous;
            }
        }

        private class StateWithDataCompletionBuilder<TState, TStateData, TCompletion> :
            StateWithDataCompletionBaseBuilder<TState, TStateData, TCompletion, CompletionEventArgs<TCompletion>>,
            IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        {
            protected override Action<TStateData> WireHandlerNewState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerNewState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete -= complete;
                };
            }

            protected override Action<TStateData> WireHandlerPreviousState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerPreviousState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete -= complete;
                };
            }
        }

        private class StateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData> :
            StateWithDataCompletionBaseBuilder<TState, TStateData, TCompletion, CompletionWithDataEventArgs<TCompletion, TData>>,
            IStateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion, TData>
        {
            protected override Action<TStateData> WireHandlerNewState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerNewState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData -= complete;
                };
            }

            protected override Action<TStateData> WireHandlerPreviousState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerPreviousState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData -= complete;
                };
            }

            protected override EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> Create(TState newState)
            {
                var changeStateAction = new EventHandler<CompletionWithDataEventArgs<TCompletion, TData>>((s, e) =>
                {
                    var dataVal = e.Data;
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToStateWithData(newState, dataVal);
                    }
                });
                return changeStateAction;
            }
        }

        private class StateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> :
            StateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
            IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        {
            public Func<TStateData, TData> Data { get; set; }

            protected override EventHandler<CompletionEventArgs<TCompletion>> Create(TState newState)
            {
                var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
                {
                    var dataVal = default(TData);
                    var data = Data;
                    if (e is CompletionWithDataEventArgs<TCompletion, TData> cc)
                    {
                        dataVal = cc.Data;
                    }
                    if (data != null)
                    {
                        var vvm = (TStateData)s;
                        dataVal = data(vvm);
                    }
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToStateWithData(newState, dataVal);
                    }
                });
                return changeStateAction;
            }

            protected override EventHandler<CompletionEventArgs<TCompletion>> InternalCreatePrevious()
            {
                EventHandler<CompletionEventArgs<TCompletion>> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoBackToPreviousState();
                    }
                };

                return changeStateAction;
            }
        }

        #endregion

    }
}