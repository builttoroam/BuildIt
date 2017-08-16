using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BuildIt.Forms
{
    /// <summary>
    /// Includes attached properties that will cascade to all nested elements (eg forecolor)
    /// </summary>
    public class Ambient
    {
        /// <summary>
        /// Foreground color - used for nested Label elements
        /// </summary>
        public static readonly BindableProperty ForeColorProperty = BindableProperty.CreateAttached(ForeColorPropertyName, typeof(Color), typeof(Ambient), Color.Transparent, BindingMode.OneWayToSource, null, ColorChanged);

        private const string ForeColorPropertyName = "ForeColor";

        private static IDictionary<Type, FieldInfo> ColorProperties { get; } = new Dictionary<Type, FieldInfo>();

        /// <summary>
        /// Gets the forecolor set on an element
        /// </summary>
        /// <param name="view">The element to get the forecolor of</param>
        /// <returns>The foreground color</returns>
        public static Color GetForeColor(BindableObject view)
        {
            return (Color)view.GetValue(ForeColorProperty);
        }

        /// <summary>
        /// Sets the forecolor property on a specific element
        /// </summary>
        /// <param name="view">The element</param>
        /// <param name="value">The color to set</param>
        public static void SetForeColor(BindableObject view, Color value)
        {
            view.SetValue(ForeColorProperty, value);
        }

        private static void ColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var clr = (Color)newValue;
            ApplyForeColor(bindable, clr);
        }

        private static void ApplyForeColor(BindableObject bindable, Color foreColor)
        {
            if (bindable == null)
            {
                return;
            }

            var objType = bindable.GetType();
            FieldInfo colorProp = null;
            if (!ColorProperties.ContainsKey(objType))
            {
                colorProp = objType.GetField("TextColorProperty");
                ColorProperties[objType] = colorProp;
            }

            var prop = colorProp?.GetValue(bindable) as BindableProperty;
            if (prop != null)
            {
                var currentVal = bindable.GetValue(prop);
                if (currentVal == null || (Color)currentVal == default(Color))
                {
                    bindable.SetValue(prop, foreColor);
                }
            }

            var element = bindable as Layout;
            if (element == null)
            {
                return;
            }

            foreach (var emt in element.Children)
            {
                ApplyForeColor(emt, foreColor);
            }

            var clr = foreColor;
            element.ChildAdded += (s, e) =>
            {
                ApplyForeColor(e.Element, clr);
            };
        }
    }
}