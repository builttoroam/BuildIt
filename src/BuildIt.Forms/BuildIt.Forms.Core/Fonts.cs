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
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.CreateAttached(FontFamilyPropertyName, typeof(string), typeof(Fonts), string.Empty, propertyChanged: OnFileNameChanged);

        private const string FontFamilyPropertyName = "FontFamily";

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

        private static void OnFileNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            view.Effects.Add(new FontEffect() { FontName = newValue as string });

           // (view as VisualElement).PropertyChanged += Fonts_PropertyChanged;
        }

        private static void Fonts_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent")
            {
                var element = sender as VisualElement;
                var parent = element?.Parent;
                if (parent == null)
                {
                    return;
                }

                parent.Effects.Add(element.Effects.OfType<FontEffect>().FirstOrDefault());
                Debug.WriteLine("Test");
            }
        }
    }
}
