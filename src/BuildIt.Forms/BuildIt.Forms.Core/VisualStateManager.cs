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


    public static class StateManagerHelper
    {
        public static async Task GoToVisualState(this IStateManager manager, VisualState state)
        {
            //var gts = manager.GetType().GetRuntimeMethod("GoToState", new Type[] { state.StateType.GetType(),typeof(bool) });
            var gts = manager.GetType().GetTypeInfo().GetDeclaredMethod("GoToState");
            gts = gts.MakeGenericMethod(state.StateType.GetType());
            var stateChange = gts.Invoke(manager, new object[] { state.StateType, true }) as Task<bool>;
            var ok = await stateChange;
        }
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

            }
        }

        private static void BuildStateSetters(VisualState state, Element element, IList<IStateValue> values)
        {
            foreach (var setter in state.Setters)
            {
                var target = setter.Target.Split('.');
                var name = target.FirstOrDefault();
                var prop = target.Skip(1).FirstOrDefault();
                var setterTarget = element.FindByName<Element>(name);
                if (setterTarget == null)
                {
                    var cv = element as ContentView;
                    if (cv == null) return;
                    foreach (var child in cv.Children)
                    {
                        setterTarget = child.FindByName<Element>(name);
                        if (setterTarget != null)
                        {
                            break;
                        }
                    }

                }

                var targetProp = element.GetType().GetProperty(prop);
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
                var builder = Activator.CreateInstance(builderType, setterTarget, targetProp, (string)val, converter) as IStateValueBuilder;

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

            public StateValueBuilder(TElement element, PropertyInfo prop, string value, TypeConverter converter)
            {
                Element = element;
                Property = prop;
                RawValue = value;
                Converter = converter;
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
                    sv.Setter = (element, val) =>
                      {

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
                 typeof(VisualStateManager), null, BindingMode.OneWayToSource, null, SettersChanged, null, null, CreateDefaultValue);

        private static void SettersChanged(BindableObject bindable, object oldvalue, object newvalue)
        {

        }

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<Setter>();
        }

        public IList<Setter> Setters
        {
            get
            {
                return (IList<Setter>)base.GetValue(VisualState.SettersProperty);
            }
        }
    }

    public class Setter
    {
        public string Value { get; set; }

        public string Target { get; set; }
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
