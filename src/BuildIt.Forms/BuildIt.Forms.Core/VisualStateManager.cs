﻿using BuildIt.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    /// <summary>
    /// Root class for interacting with visual states in Forms
    /// </summary>
    public static class VisualStateManager
    {
        private const string VisualStateGroupsPropertyName = "VisualStateGroups";
        private const string StateManagerPropertyName = "StateManager";

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

        /// <summary>
        /// Gets the state manager for the particular element
        /// </summary>
        public static BindableProperty StateManagerProperty { get; } =
            BindableProperty.CreateAttached(
                StateManagerPropertyName,
                typeof(IStateManager),
                declaringType: typeof(VisualStateManager),
                defaultValue: null,
                defaultValueCreator: (bo) =>
                {
                    return new StateManager();
                });

        /// <summary>
        /// Transitions the element to the specified state
        /// </summary>
        /// <param name="element">The element to change state</param>
        /// <param name="stateName">The new state</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task GoToState(Element element, string stateName)
        {
            var groups = GetVisualStateGroups(element);
            var manager = GetStateManager(element);

            var state = (from g in groups
                         from s in g
                         where s.Name == stateName
                         select new { Group = g, State = s }).FirstOrDefault();

            if (state == null)
            {
                var cvv = element as ContentView;
                foreach (var child in cvv.Children)
                {
                    var gps = GetVisualStateGroups(child);
                    state = (from g in gps
                             from s in g
                             where s.Name == stateName
                             select new { Group = g, State = s }).FirstOrDefault();
                    if (state != null)
                    {
                        manager = GetStateManager(child);
                        break;
                    }
                }
            }

            if (state == null || state.Group?.CurrentState == state.State)
            {
                return;
            }

            await manager.GoToVisualState(state.State);
        }

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new VisualStateGroups();
        }

        private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var manager = GetStateManager(bindable);
            UpdateStateManager(manager, bindable, (VisualStateGroups)newvalue);
        }

        private static IStateManager GetStateManager(BindableObject view)
        {
            return (IStateManager)view.GetValue(StateManagerProperty);
        }

        public static VisualStateGroups GetVisualStateGroups(BindableObject view)
        {
            return (VisualStateGroups)view.GetValue(VisualStateGroupsProperty);
        }

        public static void SetVisualStateGroups(BindableObject view, VisualStateGroups value)
        {
            view.SetValue(VisualStateGroupsProperty, value);
        }

        private static void UpdateStateManager(IStateManager manager, BindableObject view, IList<VisualStateGroup> groups)
        {
            var idx = 0;
            foreach (var vsgroup in groups)
            {
                UpdateStateManagerWithStateGroup(manager, view, vsgroup, idx);
                idx++;
            }
        }

        private static void UpdateStateManagerWithStateGroup(IStateManager manager, BindableObject view, VisualStateGroup vsgroup, int groupIdx)
        {
            var sg = new StateGroup(vsgroup.Name);
            vsgroup.StateGroup = sg;

            manager.AddStateGroup(sg);

            foreach (var vstate in vsgroup)
            {
                var stateDef = sg.DefineState(vstate.Name);
                var values = stateDef.Values;
                vstate.StateGroup = sg;

                BuildStateSetters(vstate, view as Element, values);

                var arriving = vstate.ArrivingAnimations;
                if (arriving != null)
                {
                    var animationFunction = BuildAnimations(arriving.PreAnimations, view as Element);
                    stateDef.ChangingTo = animationFunction;

                    var panimationFunction = BuildAnimations(arriving.PostAnimations, view as Element);
                    stateDef.ChangedTo = panimationFunction;
                }

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

        private static Task BuildAnimationTasks(IList<StateAnimation> animations, Element element)
        {
            var tasks = new List<Task>();

            foreach (var animation in animations)
            {
                var target = element.FindByTarget(animation);
                var tg = target?.Item1 as VisualElement;
                var animateTask = animation.Animate(tg ?? (element as VisualElement));
                tasks.Add(animateTask);
            }

            return Task.WhenAll(tasks);
        }

        private static Func<CancelEventArgs, Task> BuildCancellableAnimations(IList<StateAnimation> animations, Element element)
        {
            return (cancel) => BuildAnimationTasks(animations, element);
        }

        private static Func<Task> BuildAnimations(IList<StateAnimation> animations, Element element)
        {
            return () => BuildAnimationTasks(animations, element);
        }

        private static void BuildStateSetters(VisualState state, Element element, IList<IStateValue> values)
        {
            foreach (var setter in state.Setters)
            {
                var target = element.FindByTarget(setter);


                if (target == null) continue;
                var setterTarget = target.Item1;
                var targetProp = target.Item2;//.GetType().GetProperty(prop);
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


                var builderType = typeof(StateValueBuilder<,>).MakeGenericType(setterTarget.GetType(), targetProp.PropertyType);
                var builder = Activator.CreateInstance(builderType, setterTarget, targetProp, (string)val, converter, setter.Duration) as IStateValueBuilder;

                values.Add(builder.Value);

            }
        }

        private interface IStateValueBuilder
        {
            IStateValue Value { get; }
        }

        private class StateValueBuilder<TElement, TPropertyValue> : IStateValueBuilder
        {
            public TElement Element { get; set; }
            public PropertyInfo Property { get; set; }

            public string RawValue { get; set; }
            public TypeConverter Converter { get; set; }

            public int Duration { get; set; }

            public StateValueBuilder(TElement element, PropertyInfo prop, string value, TypeConverter converter, int duration)
            {
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
                    sv.Key = new Tuple<object, string>(Element, Property.Name);
                    sv.Getter = (element) =>
                    {
                        var val = Property.GetValue(element);
                        if (val is TPropertyValue) return (TPropertyValue)val;
                        return default(TPropertyValue);
                    };
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
                                  if (remainingSteps <= 0) break;
                                  var inc = (end - current) / remainingSteps;
                                  current += inc;
                                  Property.SetValue(element, current);
                                  await Task.Delay((int)stepduration);
                                  startTime = DateTime.Now;
                              }
                          }
                          Property.SetValue(element, val);
                      };
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
        }

        public static async Task<IStateBinder> Bind(Element element, IStateManager stateManager)
        {
            var sm = VisualStateManager.GetStateManager(element);
            var binder = sm.Bind(stateManager);
            await binder.Bind();
            return binder;
        }
    }
}
