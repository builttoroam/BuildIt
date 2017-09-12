using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Helper class for working with fonts
    /// </summary>
    public static class Fonts
    {
        /// <summary>
        /// Font family attached property
        /// </summary>
        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.CreateAttached(
                FontFamilyPropertyName,
                typeof(string),
                typeof(Fonts),
                string.Empty,
                propertyChanged: FontChanged);

        private const string FontFamilyPropertyName = "FontFamily";
        
        /// <summary>
        /// Font size attached property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.CreateAttached(
                FontSizePropertyName,
                typeof(double),
                typeof(Fonts),
                0.0,
                propertyChanged: FontSizeChanged);

        private const string FontSizePropertyName = "FontSize";

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
                var view = bindable as View;

                view?.Effects.Add(new FontEffect { FontName = newValue as string });
            }
            catch (Exception ex)
            {
                ex.LogException();
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
                $"size changed {newValue}".Log();
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis - doesn't work for casting
                if (!(newValue is double value) || value == 0.0)
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
                {
                    "size not a double".Log();
                    return;
                }

                $"size is a double {value}".Log();

                var view = bindable as Element;

                ApplyToNested<Label>(view, label => label.FontSize = value);
            }
            catch (Exception ex)
            {
                ex.Message.Log();
                ex.StackTrace.Log();
                ex.LogException();
            }
        }

        private static void ApplyToNested<TElement>(Element view, Action<TElement> action)
            where TElement : View
        {
            if (view == null)
            {
                "null".Log();
                return;
            }

            $"attempting to match {view.GetType().Name}".Log();
            if (view is TElement element)
            {
                "matching view found".Log();
                action(element);
            }
            else if (view is Layout layout)
            {
                $"matching view not found - searching children of {view.GetType().Name}".Log();
                foreach (var subelement in layout.Children)
                {
                    ApplyToNested(subelement, action);
                }

                layout.ChildAdded += (s, e) =>
                        {
                            "child added".Log();
                            ApplyToNested(e.Element, action);
                        };
            }
            else
            {
                $"matching view not found for {view.GetType().Name}".Log();
            }
        }
    }
}
