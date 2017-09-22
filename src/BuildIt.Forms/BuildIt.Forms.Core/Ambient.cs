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
        /// Font family attached property
        /// </summary>
        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.CreateAttached(
                FontFamilyPropertyName,
                typeof(string),
                typeof(Ambient),
                string.Empty,
                propertyChanged: FontChanged);

        private const string FontFamilyPropertyName = "FontFamily";

        /// <summary>
        /// Foreground color - used for nested Label elements
        /// </summary>
        public static readonly BindableProperty ForeColorProperty =
            BindableProperty.CreateAttached(ForeColorPropertyName,
                typeof(Color),
                typeof(Ambient),
                Color.Transparent,
                BindingMode.OneWayToSource,
                null,
                ColorChanged);

        private const string ForeColorPropertyName = "ForeColor";

        /// <summary>
        /// Font size attached property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.CreateAttached(
                FontSizePropertyName,
                typeof(double),
                typeof(Ambient),
                0.0,
                propertyChanged: FontSizeChanged);

        private const string FontSizePropertyName = "FontSize";

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

        /// <summary>
        /// Gets the font family
        /// </summary>
        /// <param name="view">The element to retrieve the font family for</param>
        /// <returns>The font family</returns>
        public static string GetFontFamily(BindableObject view)
        {
            return (string)view.GetValue(FontFamilyProperty);
        }

        /// <summary>
        /// Sets the font family
        /// </summary>
        /// <param name="view">The element to set the font family on</param>
        /// <param name="value">The font family value</param>
        public static void SetFontFamily(BindableObject view, string value)
        {
            view.SetValue(FontFamilyProperty, value);
        }

        private static void FontChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                //var view = bindable as View;

                //view?.Effects.Add(new FontEffect { FontName = newValue as string });

                var view = bindable as Element;

                ApplyToNested<Label>(view, FontFamilyProperty, label => label?.Effects.Add(new FontEffect { FontName = newValue as string }),true);
            }
            catch (Exception ex)
            {
                ex.LogFormsException();
            }
        }


        /// <summary>
        /// Gets the font size
        /// </summary>
        /// <param name="view">The element to retrieve the font size for</param>
        /// <returns>The font size</returns>
        public static double GetFontSize(BindableObject view)
        {
            return (double)view.GetValue(FontSizeProperty);
        }

        /// <summary>
        /// Sets the font size
        /// </summary>
        /// <param name="view">The element to set the font size on</param>
        /// <param name="value">The font size value</param>
        public static void SetFontSize(BindableObject view, double value)
        {
            view.SetValue(FontSizeProperty, value);
        }

        private static void FontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                $"size changed {newValue}".LogFormsInfo();
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis - doesn't work for casting
                if (!(newValue is double value) || value == 0.0)
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
                {
                    "size not a double".LogFormsInfo();
                    return;
                }

                $"size is a double {value}".LogFormsInfo();

                var view = bindable as Element;

                ApplyToNested<Label>(view,FontSizeProperty,  label => label.FontSize = value,true);
            }
            catch (Exception ex)
            {
                ex.LogFormsException();
            }
        }

        private static void ApplyToNested<TElement>(Element view, BindableProperty property, Action<TElement> action, bool root)
            where TElement : View
        {
            if (view == null)
            {
                "null".LogFormsInfo();
                return;
            }

            $"attempting to match {view.GetType().Name}".LogFormsInfo();
            if (!root)
            {
                var val = view.GetValue(property);
                var hasValue =  val!= property.DefaultValue;
                if (hasValue)
                {
                    $"has a value assigned {val}".LogFormsInfo();
                    return;
                }
            }

            if (view is TElement element)
            {
                "matching view found".LogFormsInfo();
                action(element);
            }
            else if (view is Layout layout)
            {
                $"matching view not found - searching children of {view.GetType().Name}".LogFormsInfo();
                foreach (var subelement in layout.Children)
                {
                    ApplyToNested(subelement, property, action,false);
                }

                layout.ChildAdded += (s, e) =>
                {
                    "child added".LogFormsInfo();
                    ApplyToNested(e.Element, property,action, false);
                };
            }
            else
            {
                $"matching view not found for {view.GetType().Name}".LogFormsInfo();
            }
        }
    }
}