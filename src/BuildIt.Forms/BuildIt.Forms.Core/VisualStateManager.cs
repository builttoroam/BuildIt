using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Forms.Animations;
using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.Typed.String;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Root class for interacting with visual states in Forms
    /// </summary>
    public static class VisualStateManager
    {
        /// <summary>
        /// Gets the visual state groups for a particular element
        /// </summary>
        public static readonly BindableProperty VisualStateGroupsProperty =
           BindableProperty.CreateAttached(
               VisualStateGroupsPropertyName,
               typeof(VisualStateGroups),
               declaringType: typeof(VisualStateManager),
               defaultValue: null,
               defaultBindingMode: BindingMode.OneWayToSource,
               validateValue: null,
               propertyChanged: StateGroupsChanged,
               propertyChanging: null,
               coerceValue: null,
               defaultValueCreator: CreateDefaultValue);

        private const string VisualStateGroupsPropertyName = "VisualStateGroups";

        /// <summary>
        /// Wraps the generation of a state value
        /// </summary>
        private interface IStateValueBuilder
        {
            /// <summary>
            /// Gets the current state value
            /// </summary>
            IStateValue Value { get; }
        }

        /// <summary>
        /// Transitions the element to the specified state
        /// </summary>
        /// <param name="element">The element to change state</param>
        /// <param name="stateName">The new state</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task GoToState(Element element, string stateName)
        {
            var groups = GetVisualStateGroups(element);
            var manager = groups.StateManager;

            var state = (from g in groups
                         from s in g
                         where s.Name == stateName
                         select new { Group = g, State = s }).FirstOrDefault();

            if (state == null)
            {
                return;
            }

            // TODO: This is a hack because there's an issue with the state manager
            // that cancels a current transition if we attempt to transition to the same
            // state again
            var current = manager.CurrentState(state.State.StateGroup.GroupName);
            if (current == state.State.Name)
            {
                return;
            }

            await manager.GoToVisualState(state.State);
        }

        /// <summary>
        /// Gets the visual state groups instance associated with the element
        /// </summary>
        /// <param name="view">The element to retrieve the visual state group for</param>
        /// <returns>A visual state groups instance</returns>
        public static VisualStateGroups GetVisualStateGroups(BindableObject view)
        {
            try
            {
                // If a content view, the state groups should be appended to the first child
                // (ie the first element inside the template
                if (view is ContentView cv)
                {
                    view = cv.Children.FirstOrDefault();
                }

                return (VisualStateGroups)view.GetValue(VisualStateGroupsProperty);
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return null;
        }

        /// <summary>
        /// Sets a visual state groups instance for an element
        /// </summary>
        /// <param name="view">The element to associated a visual state groups instance with</param>
        /// <param name="value">The visual state groups instance</param>
        public static void SetVisualStateGroups(BindableObject view, VisualStateGroups value)
        {
            try
            {
                // If a content view, the state groups should be appended to the first child
                // (ie the first element inside the template
                if (view is ContentView cv)
                {
                    view = cv.Children.FirstOrDefault();
                }

                view.SetValue(VisualStateGroupsProperty, value);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        /// <summary>
        /// Binds together two state managers
        /// </summary>
        /// <param name="element">The element that houses the state manager to be kept up to date</param>
        /// <param name="stateManager">The state manager to monitor</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<IStateBinder> Bind(Element element, IStateManager stateManager)
        {
            var groups = VisualStateManager.GetVisualStateGroups(element);
            if (groups?.StateManager == null)
            {
                return null;
            }

            var binder = await groups.StateManager.Bind(stateManager);
            return binder;
        }

        /// <summary>
        /// Builds the animation task for the series of animations
        /// </summary>
        /// <param name="animations">The list of animations</param>
        /// <param name="element">The element to animate</param>
        /// <param name="cancelToken">The cancellation token</param>
        /// <returns>Task to await</returns>
        public static Task BuildAnimationTasks(IList<StateAnimation> animations, Element element, CancellationToken cancelToken)
        {
            var tasks = new List<Task>();

            foreach (var animation in animations)
            {
                var target = element.FindByTarget(animation);
                var tg = target?.Item1 as VisualElement;
                var animateTask = animation.Animate(tg ?? (element as VisualElement), cancelToken);
                tasks.Add(animateTask);
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns a function that build animations for an element
        /// </summary>
        /// <param name="animations">The animations to use</param>
        /// <param name="element">The element to animat</param>
        /// <returns>The function to build an animations task</returns>
        public static Func<CancellationToken, Task> BuildAnimations(IList<StateAnimation> animations, Element element)
        {
            return (cancel) => BuildAnimationTasks(animations, element, cancel);
        }

        private static object CreateDefaultValue(BindableObject bindable)
        {
            "Creating empty visual state group".Log();
            return new VisualStateGroups();
        }

        private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            try
            {
                "Visual State Groups hydrated from XAML".Log();
                UpdateStateManager(bindable, (VisualStateGroups)newvalue);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        private static void UpdateStateManager(BindableObject view, VisualStateGroups groups)
        {
            if (groups == null || view == null || groups?.Count == 0)
            {
                "Missing parameter to build groups in state manager".Log();
                return;
            }

            var manager = groups.StateManager;
            foreach (var vsgroup in groups)
            {
                UpdateStateManagerWithStateGroup(manager, view, vsgroup);
            }
        }

        private static void UpdateStateManagerWithStateGroup(IStateManager manager, BindableObject view, VisualStateGroup vsgroup)
        {
            try
            {
                StateGroup sg;
                "Creating new StateGroup".Log();
                var key = vsgroup.DefinitionCacheKey;
                var isExisting = false;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    sg = new StateGroup(vsgroup.Name, key);
                    isExisting = sg.TypedGroupDefinition.States.Count != 0;
                }
                else
                {
                    sg = new StateGroup(vsgroup.Name);
                }

                vsgroup.StateGroup = sg;
                manager.AddStateGroup(sg);

                if (isExisting)
                {
                    BuildSetterValuesForAllStates(vsgroup, sg, view as Element);
                    return;
                }

                var elementIds = new Dictionary<Element, string>();

                $"Defining states for group {vsgroup.Name}".Log();
                foreach (var vstate in vsgroup)
                {
                    $"Creating new state {vstate.Name}".Log();
                    var stateDef = sg.TypedGroupDefinition.DefineStateFromName(vstate.Name);
                    var values = stateDef.Values;
                    vstate.StateGroup = sg;

                    "Building state setters".Log();
                    BuildStateSetters(vsgroup, vstate, view as Element, values, elementIds);

                    "Building arriving animations".Log();
                    var arriving = vstate.ArrivingAnimations;
                    if (arriving != null)
                    {
                        var animationFunction = BuildAnimations(arriving.PreAnimations, view as Element);
                        stateDef.ChangingTo = animationFunction;

                        var panimationFunction = BuildAnimations(arriving.PostAnimations, view as Element);
                        stateDef.ChangedTo = panimationFunction;
                    }

                    "Building leaving animations".Log();
                    var leaving = vstate.LeavingAnimations;
                    if (leaving != null)
                    {
                        var animationFunction = BuildAnimations(leaving.PreAnimations, view as Element);
                        stateDef.ChangingFrom = animationFunction;

                        var panimationFunction = BuildAnimations(leaving.PostAnimations, view as Element);
                        stateDef.ChangedFrom = panimationFunction;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        private static void BuildSetterValuesForAllStates(VisualStateGroup vsgroup, StateGroup sg, Element element)
        {
            foreach (var vsstate in vsgroup)
            {
                vsstate.StateGroup = sg;
                var setterIndex = -1;
                foreach (var setter in vsstate.Setters)
                {
                    setterIndex++;
                    var targetId = vsgroup.Name + "_" + vsstate.Name + "_" + setterIndex;
                    var target = element.FindByTarget(setter);
                    sg.StateValueTargets[targetId] = target.Item1;
                }
            }
        }

        private static void BuildStateSetters(VisualStateGroup group, VisualState state, Element element, IList<IStateValue> values, IDictionary<Element, string> elementIds)
        {
            // await Task.Yield();
            var setterIndex = -1;
            foreach (var setter in state.Setters)
            {
                setterIndex++;
                $"Setter: {setter.Property}".Log();
                var target = element.FindByTarget(setter);
                if (target == null)
                {
                    continue;
                }

                var setterTarget = target.Item1;
                var targetProp = target.Item2;
                var targetType = targetProp.PropertyType;
                var val = (object)setter.Value;
                TypeConverter converter = null;
                if (targetType != typeof(string))
                {
                    var converterType = targetType.GetTypeInfo()
                        .GetCustomAttribute<TypeConverterAttribute>(true)
                        ?.ConverterTypeName;
                    if (converterType != null)
                    {
                        converter = Activator.CreateInstance(Type.GetType(converterType)) as TypeConverter;
                    }
                }

                var targetId = group.Name + "_" + state.Name + "_" + setterIndex;
                var existing = elementIds.SafeValue(target.Item1);
                if (existing != null)
                {
                    targetId = existing;
                }
                else
                {
                    elementIds[target.Item1] = targetId;
                }

                var builderType = typeof(StateValueBuilder<,>).MakeGenericType(setterTarget.GetType(), targetProp.PropertyType);

                if (!string.IsNullOrWhiteSpace(group.DefinitionCacheKey))
                {
                    group.StateGroup.StateValueTargets[targetId] = setterTarget;
                    setterTarget = null;
                }

                var builder = Activator.CreateInstance(builderType, targetId, setterTarget, targetProp, (string)val, converter, setter.Duration) as IStateValueBuilder;

                values.Add(builder.Value);
            }
        }

        private class StateValueBuilder<TElement, TPropertyValue> : IStateValueBuilder
        {
            public StateValueBuilder(string targetId, TElement element, PropertyInfo prop, string value, TypeConverter converter, int duration)
            {
                TargetId = targetId;
                Element = element;
                Property = prop;
                RawValue = value;
                Converter = converter;
                Duration = duration;
            }

            public IStateValue Value
            {
                get
                {
                    var sv = new StateValue<TElement, TPropertyValue>();
                    sv.Element = Element;
                    sv.TargetId = TargetId;
                    if (Element != null)
                    {
                        sv.Key = new Tuple<object, string>(Element, Property.Name);
                    }
                    else
                    {
                        sv.Key = new Tuple<object, string>(TargetId, Property.Name);
                    }

                    sv.Getter = (element) =>
                    {
                        var val = Property.GetValue(element);
                        if (val is TPropertyValue)
                        {
                            return (TPropertyValue)val;
                        }

                        return default(TPropertyValue);
                    };

                    if (typeof(TElement).GetTypeInfo().IsSubclassOf(typeof(VisualElement)) && Duration == 0)
                    {
                        sv.Setter = VisualElementProperties.Lookup<TElement, TPropertyValue>(Property.Name);
                    }

                    if (sv.Setter == null)
                    {
                        sv.Setter = async (element, val) =>
                        {
                            if (Duration > 0)
                            {
                                var startTime = DateTime.Now;
                                var endTime = startTime.AddMilliseconds(Duration);
                                var stepduration = 1000.0 / 60.0; // ~60 frames per sec
                                var current = (double)Property.GetValue(element);
                                var end = (double)(object)val;
                                while (startTime < endTime)
                                {
                                    var remainingSteps = endTime.Subtract(startTime).TotalMilliseconds / stepduration;
                                    if (remainingSteps <= 0)
                                    {
                                        break;
                                    }

                                    var inc = (end - current) / remainingSteps;
                                    current += inc;
                                    Property.SetValue(element, current);
                                    await Task.Delay((int)stepduration);
                                    startTime = DateTime.Now;
                                }
                            }

                            Property.SetValue(element, val);
                        };
                    }

                    if (Converter?.CanConvertFrom(typeof(string)) ?? false)
                    {
                        sv.Value = (TPropertyValue)Converter.ConvertFromInvariantString(RawValue);
                    }
                    else if (RawValue is TPropertyValue)
                    {
                        sv.Value = (TPropertyValue)(object)RawValue;
                    }
                    else
                    {
                        var parse = typeof(TPropertyValue).GetRuntimeMethod("Parse", new Type[] { typeof(string) });
                        if (parse != null)
                        {
                            sv.Value = (TPropertyValue)parse.Invoke(null, new object[] { RawValue });
                        }
                    }

                    return sv;
                }
            }

            private TElement Element { get; set; }

            private string TargetId { get; set; }

            private PropertyInfo Property { get; set; }

            private string RawValue { get; set; }

            private TypeConverter Converter { get; set; }

            private int Duration { get; set; }
        }
    }
}
