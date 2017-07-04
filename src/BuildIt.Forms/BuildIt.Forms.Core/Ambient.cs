using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms.Core
{
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
                    if (currentVal == null || (Color)currentVal == default(Color))
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
}