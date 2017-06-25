using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Graphics;
using System.ComponentModel;
using System.Linq;
using System;
using Android.Views;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.Droid.FontEffect), nameof(BuildIt.Forms.Controls.Droid.FontEffect))]
namespace BuildIt.Forms.Controls.Droid
{
    public class FontEffect : PlatformEffect
    {
        //public FontEffect():base()
        //{

        //}

        private string FileName { get; set; }

        TextView control;
        protected override void OnAttached()
        {
            try
            {
                var view = Control == null ? Container : Control;

                var effect = (BuildIt.Forms.Controls.FontEffect)Element.Effects.
                         FirstOrDefault(e => e is BuildIt.Forms.Controls.FontEffect);
                FileName = effect.FontName.Split('#').FirstOrDefault();
                if (string.IsNullOrWhiteSpace(FileName)) return;

                ApplyToLabels(view as ViewGroup);



                //control = Control as TextView;
                //Typeface font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.ApplicationContext.Assets, "Fonts/" + fileName);
                //control.Typeface = font;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        private void ApplyToLabels(ViewGroup group)
        {
            if (group == null) return;

            var cnt = group.ChildCount;
            for (int i = 0; i < cnt; i++)
            {
                var child = group.GetChildAt(i);
                if(child is TextView control)
                {
                    Typeface font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.Assets, "Fonts/" + FileName);
                    control.Typeface = font;
                }
                else if(child is ViewGroup subgroup)
                {
                    ApplyToLabels(subgroup);
                }
            }
        }

        protected override void OnDetached()
        {
        }

        //protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        //{
        //    if (args.PropertyName == Fonts.FontFamilyProperty.PropertyName)
        //    {
        //        var effect = (BuildIt.Forms.Controls.FontEffect)Element.Effects.
        //                 FirstOrDefault(e => e is BuildIt.Forms.Controls.FontEffect);
        //        var fileName = effect.FontName.Split('#').FirstOrDefault();
        //        if (string.IsNullOrWhiteSpace(fileName)) return;
        //        Typeface font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.ApplicationContext.Assets, "Fonts/" + fileName);
        //        control.Typeface = font;
        //    }
        //}

    }
}

