using BuildIt.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms.Core
{
    public enum _Group0
    {
        _State0,
        _State1,
        _State2,
        _State3,
        _State4,
        _State5,
        _State6,
        _State7,
        _State8,
        _State9
    }
    public enum _Group1
    {
        _State0,
        _State1,
        _State2,
        _State3,
        _State4,
        _State5,
        _State6,
        _State7,
        _State8,
        _State9
    }
    public enum _Group2
    {
        _State0,
        _State1,
        _State2,
        _State3,
        _State4,
        _State5,
        _State6,
        _State7,
        _State8,
        _State9
    }


    public class VisualStateManager
    {
        public static Type[] StateTypes = new[] { typeof(_Group0), typeof(_Group1), typeof(_Group2) };

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

            if (state == null || state.Group?.CurrentState == state.State) return;
            await manager.GoToVisualState(state.State);
            //foreach (var setter in state.State.Setters)
            //{
            //    var target = setter.Target.Split('.');
            //    var name = target.FirstOrDefault();
            //    var prop = target.Skip(1).FirstOrDefault();
            //    var setterTarget = element.FindByName<Element>(name);
            //    if (setterTarget == null)
            //    {
            //        var cv = element as ContentView;
            //        foreach (var child in cv.Children)
            //        {
            //            setterTarget = child.FindByName<Element>(name);
            //            if (setterTarget != null)
            //            {
            //                break;
            //            }
            //        }
            //    }

            //    var targetProp = element.GetType().GetProperty(prop);
            //    var targetType = targetProp.PropertyType;
            //    var val = (object)setter.Value;
            //    if (targetType != typeof(string))
            //    {
            //        var converterType = targetType.GetTypeInfo()
            //            .GetCustomAttribute<TypeConverterAttribute>(true)
            //            ?.ConverterTypeName;
            //        var converter = Activator.CreateInstance(Type.GetType(converterType)) as TypeConverter;
            //        if (!converter?.CanConvertFrom(typeof(string)) ?? false) return;
            //        val = converter.ConvertFromInvariantString((string)val);
            //    }

            //    targetProp?.SetValue(setterTarget, val);
            //}

            //state.Group.CurrentState = state.State;
        }
        /*
         * internal static readonly BindablePropertyKey TriggersPropertyKey = 
         * BindableProperty.CreateReadOnly(
         *      "Triggers", 
         *      typeof(IList<TriggerBase>), 
         *      typeof(VisualElement), 
         *      null, 
         *      BindingMode.OneWayToSource, 
         *      null, null, 
         *      null, null, 
         *      new BindableProperty.CreateDefaultValueDelegate(VisualElement.<>c.<>9.<.cctor>b__214_5));

         * 
         * public IList<TriggerBase> Triggers
{
	get
	{
		return (IList<TriggerBase>)base.GetValue(VisualElement.TriggersProperty);
	}
}
         * 
         * 
         */


        public static readonly BindableProperty VisualStateGroupsProperty =
            BindableProperty.CreateAttached("VisualStateGroups", typeof(IList<VisualStateGroup>),
                typeof(VisualStateManager), null, BindingMode.OneWayToSource, null, StateGroupsChanged, null, null, CreateDefaultValue);

        public static readonly BindableProperty StateManagerProperty =
    BindableProperty.CreateAttached("StateManager", typeof(IStateManager),
        typeof(VisualStateManager), null, defaultValueCreator: (bo) =>
         {

             return new StateManager();
         });

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<VisualStateGroup>();
        }

        private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var manager = GetStateManager(bindable);
            UpdateStateManager(manager, bindable, (IList<VisualStateGroup>)newvalue);
        }

        public static IStateManager GetStateManager(BindableObject view)
        {
            return (IStateManager)view.GetValue(StateManagerProperty);
        }

        public static IList<VisualStateGroup> GetVisualStateGroups(BindableObject view)
        {
            return (IList<VisualStateGroup>)view.GetValue(VisualStateGroupsProperty);
        }

        public static void SetVisualStateGroups(BindableObject view, IList<VisualStateGroup> value)
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
            var stateType = StateTypes[groupIdx]; // TODO: Need to protect against out of range 
            vsgroup.StateGroupType = stateType;

            var sgType = typeof(StateGroup<>).MakeGenericType(stateType);
            var sg = Activator.CreateInstance(sgType) as IStateGroup;
            vsgroup.StateGroup = sg;

            manager.AddStateGroup(stateType, sg);

            var idx = 1;
            foreach (var vstate in vsgroup)
            {
                var state = Enum.GetValues(stateType).GetValue(idx);
                var defState = sg.GetType().GetRuntimeMethod("DefineState", new Type[] { stateType });
                var stateDef = defState.Invoke(sg, new object[] { state });
                vstate.StateType = state;
                idx++;

                var valuesProp = stateDef.GetType().GetProperty("Values");
                var values = valuesProp.GetValue(stateDef) as IList<IStateValue>;


                BuildStateSetters(vstate, view as Element, values);

                var animationFunction = BuildAnimations(vstate, view as Element);
                if(animationFunction!=null)
                {
                    var changingTo = stateDef.GetType().GetProperty("ChangedTo");
                    changingTo.SetValue(stateDef, animationFunction);
                }
            }
        }
        private static Func<Task> BuildAnimations(VisualState state, Element element)
        {
            return new Func<Task>(() =>
            {
                var tasks = new List<Task>();

                foreach (var animation in state.Animations)
                {
                    var target = element.FindByTarget(animation);
                    var animateTask = animation.Animate(target.Item1 as VisualElement);
                    tasks.Add(animateTask);
                }

                return Task.WhenAll(tasks);
            });
        }

            private static void BuildStateSetters(VisualState state, Element element, IList<IStateValue> values)
        {
            foreach (var setter in state.Setters)
            {
                var target = element.FindByTarget(setter);
                //var target = setter.Target.Split('.');
                //var name = target.FirstOrDefault();
                //var prop = target.Skip(1).FirstOrDefault();
                //var setterTarget = element.FindByName<Element>(name);
                //if (setterTarget == null)
                //{
                //    var cv = element as ContentView;
                //    if (cv == null) return;
                //    foreach (var child in cv.Children)
                //    {
                //        setterTarget = child.FindByName<Element>(name);
                //        if (setterTarget != null)
                //        {
                //            break;
                //        }
                //    }

                //}

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
                          if (Duration >0)
                          {
                              var startTime = DateTime.Now;
                              var endTime = startTime.AddMilliseconds(Duration);
                              var stepduration = 1000.0/60.0; // ~60 frames per sec
                              var current = (double)Property.GetValue(element);
                              var end = (double)(object)val;
                              while(startTime<endTime)
                              {
                                  var remainingSteps = endTime.Subtract(startTime).TotalMilliseconds / stepduration;
                                  if (remainingSteps <= 0) break;
                                  var inc = (end- current) / remainingSteps;
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


        //public static readonly BindableProperty VisualStateGroupsProperty =
        //    BindableProperty.CreateAttached("VisualStateGroups", typeof(Array), 
        //        typeof(VisualStateManager),null,BindingMode.OneWay,null,StateGroupsChanged);

        //private static void StateGroupsChanged(BindableObject bindable, object oldvalue, object newvalue)
        //{
        //}

        //public static Array GetVisualStateGroups(BindableObject view)
        //{
        //    return (Array)view.GetValue(VisualStateGroupsProperty);
        //}

        //public static void SetVisualStateGroups(BindableObject view, Array value)
        //{
        //    view.SetValue(VisualStateGroupsProperty, value);
        //}
        public static void Bind(Element element, IStateManager stateManager)
        {
            // Retrieve the list of state groups from the ViewModel's StateManager
            var groups = stateManager.StateGroups;
            var visualStateManagerGroups = VisualStateManager.GetVisualStateGroups(element);


           if (visualStateManagerGroups?.Count==0)
            {
                var cvv = element as ContentView;
                foreach (var child in cvv.Children)
                {
                    visualStateManagerGroups = GetVisualStateGroups(child);
                    if (visualStateManagerGroups?.Count != 0) break;
                }
            }

            // For each group we need to locate the state group in the StateManager
            // in the xaml and then wire up statechanged event handlers
            foreach (var group in groups)
            {
                var groupName = group.Key.Name; // The state groups are defined by an enum type

                var visualStateGroup = (from vsg in visualStateManagerGroups
                    where vsg.Name == groupName
                    select vsg).FirstOrDefault();
                if (visualStateGroup == null) continue;


                var helperType = typeof(StateChangeHelper<>).MakeGenericType(group.Key);
                var helper = Activator.CreateInstance(helperType, group.Value, element);
            }
        }

        private class StateChangeHelper<TState> where TState : struct
        {
            private IStateGroup<TState> StateGroup { get; }
            private Element Element { get; }
            public StateChangeHelper(IStateGroup<TState> stateGroup, Element element)
            {
                StateGroup = stateGroup;
                Element = element;

                StateGroup.StateChanged += async (s, e) =>
                {
                    await VisualStateManager.GoToState(Element, e.State.ToString());
                };
            }
        }
    }

    public static class ElementHelper
    {
        public static Tuple<Element, PropertyInfo> FindByTarget(this Element element, TargettedStateAction setter)
        {
            //var setterTarget
            var target = setter.Target.Split('.');
            var name = target.FirstOrDefault();
            var prop = target.Skip(1).FirstOrDefault();
            var setterTarget = element.FindByName<Element>(name);
            if (setterTarget == null)
            {
                var cv = element as ContentView;
                if (cv != null)
                {
                    foreach (var child in cv.Children)
                    {
                        setterTarget = child.FindByName<Element>(name);
                        if (setterTarget != null)
                        {
                            break;
                        }
                    }
                }
            }
            var targetProp = prop!=null?setterTarget?.GetType()?.GetProperty(prop):null;
            if (setterTarget == null) return null;
            return new Tuple<Element, PropertyInfo>(setterTarget,targetProp);
        }
    }

    public class VisualStateGroup : List<VisualState>
    {
        // TODO: Fix so we don't need to track the type used in the state manager
        public Type StateGroupType { get; set; }
        public IStateGroup StateGroup { get; set; }

        public string Name { get; set; }

        public VisualState CurrentState { get; set; }



        public VisualStateGroup()
        {

        }

    }

    public class VisualState : BindableObject
    {
        // TODO: Remove need to track state type object (use Name instead)
        public object StateType { get; set; }

        public string Name { get; set; }
        public static readonly BindableProperty SettersProperty =
             BindableProperty.CreateAttached("Setters", typeof(IList<Setter>),
                 typeof(VisualState), null, BindingMode.OneWayToSource, null, SettersChanged, null, null, CreateDefaultValue);



        public IList<StateAnimation> Animations
        {
            get { return (IList<StateAnimation>)GetValue(AnimationsProperty); }
            set { SetValue(AnimationsProperty, value); }
        }

        public static readonly BindableProperty AnimationsProperty =
             BindableProperty.CreateAttached("Animations", typeof(IList<StateAnimation>),
                 typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);



        private static void SettersChanged(BindableObject bindable, object oldvalue, object newvalue)
        {

        }

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<Setter>();
        }

        private static object CreateDefaultAnimations(BindableObject bindable)
        {
            return new List<StateAnimation>();
        }
        public IList<Setter> Setters
        {
            get
            {
                return (IList<Setter>)base.GetValue(VisualState.SettersProperty);
            }
        }


    }

    public class RotateAnimation:StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RotateTo(Rotation, (uint)Duration);
        }
    }

    public class TranslateAnimation : StateAnimation
    {
        public double TranslationX { get; set; }
        public double TranslationY { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.TranslateTo(TranslationX,TranslationY, (uint)Duration);
        }
    }

    public class ScaleAnimation : StateAnimation
    {
        public double Scale { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.ScaleTo(Scale, (uint)Duration);
        }
    }
    public class IncrementRotateAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RelRotateTo(Rotation, (uint)Duration);
        }
    }

    public class IncrementScaleAnimation : StateAnimation
    {
        public double Scale { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RelScaleTo(Scale, (uint)Duration);
        }
    }


    public class RotateYAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RotateYTo(Rotation, (uint)Duration);
        }
    }
    public class RotateXAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RotateXTo(Rotation, (uint)Duration);
        }
    }

    public class FadeAnimation : StateAnimation
    {
        public double Opacity { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.FadeTo(Opacity, (uint)Duration);
        }
    }

    public abstract class StateAnimation : TargettedStateAction
    {
        public abstract Task Animate(VisualElement visualElement);
    }

    public class Setter: TargettedStateAction
    {
        public string Value { get; set; }

        
    }

    public class TargettedStateAction
    {
        public string Target { get; set; }

        public int Duration { get; set; }
    }

    public class Ambient
    {
        public static readonly BindableProperty ForeColorProperty =
           BindableProperty.CreateAttached("ForeColor", typeof(Color),
               typeof(Ambient), Color.Transparent, BindingMode.OneWayToSource, null, ColorChanged);

        private static void ColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var clr = (Color)newValue;
            ApplyForeColor(bindable, clr);

        }

        private static IDictionary<Type, FieldInfo> ColorProperties = new Dictionary<Type, FieldInfo>();
        private static void ApplyForeColor(BindableObject bindable, Color foreColor)
        {
            if (bindable == null) return;

            var objType = bindable.GetType();
            FieldInfo colorProp = null;
            if (!ColorProperties.ContainsKey(objType))
            {
                colorProp = objType.GetField("TextColorProperty");
                ColorProperties[objType] = colorProp;
            }

            if (colorProp != null)
            {
                var prop = colorProp.GetValue(bindable) as BindableProperty;
                if (prop != null)
                {
                    var currentVal = bindable.GetValue(prop);
                    if (currentVal == null || (Color)currentVal==default(Color))
                    {
                        bindable.SetValue(prop, foreColor);
                    }
                }
            }

            var element = bindable as Layout;
            if (element != null)
            {
                foreach (var emt in element.Children)
                {
                    ApplyForeColor(emt, foreColor);
                }
                var clr = foreColor;
                element.ChildAdded += (s, e) =>
                {
                    ApplyForeColor(e.Element as BindableObject, clr);
                };
            }
        }
    }


    /*
     *  <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name="States">
                <VisualState x:Name="Test">
                    <VisualState.Setters>
                        <Setter Value="Red"
                                Target="BG.(Grid.Background)"/>
                    </VisualState.Setters>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>
     * 
     */
}
