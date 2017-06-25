using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{

    public static class Fonts
    {
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.CreateAttached("FontFamily", typeof(string), typeof(Fonts), "", propertyChanged: OnFileNameChanged);
        public static string GetFontFamily(BindableObject view)
        {
            return (string)view.GetValue(FontFamilyProperty);
        }

        public static void SetFontFamily(BindableObject view, string value)
        {
            view.SetValue(FontFamilyProperty, value);
        }
        static void OnFileNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }
            view.Effects.Add(new FontEffect() { FontName = newValue as string });

            (view as VisualElement).PropertyChanged += Fonts_PropertyChanged;

        }

        private static void Fonts_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent")
            {
                var element = (sender as VisualElement);
                var parent = element?.Parent;
                if (parent == null) return;
                parent.Effects.Add(element.Effects.OfType<FontEffect>().FirstOrDefault());
                Debug.WriteLine("Test");
            }
        }
    }
   
}
