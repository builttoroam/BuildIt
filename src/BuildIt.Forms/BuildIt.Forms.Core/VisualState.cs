using System.Collections.Generic;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class VisualState : BindableObject
    {
        public IStateGroup StateGroup { get; set; }


        public string Name { get; set; }
        public static readonly BindableProperty SettersProperty =
            BindableProperty.CreateAttached(nameof(Setters), typeof(IList<Setter>),
                typeof(VisualState), null, BindingMode.OneWayToSource, null, SettersChanged, null, null, CreateDefaultValue);


        public AnimationGroup ArrivingAnimations
        {
            get => (AnimationGroup)GetValue(ArrivingAnimationsProperty);
            set => SetValue(ArrivingAnimationsProperty, value);
        }

        public static readonly BindableProperty ArrivingAnimationsProperty =
            BindableProperty.CreateAttached(nameof(ArrivingAnimations), typeof(AnimationGroup),
                typeof(VisualState), null, BindingMode.OneWayToSource);


        public AnimationGroup LeavingAnimations
        {
            get => (AnimationGroup)GetValue(LeavingAnimationsProperty);
            set => SetValue(LeavingAnimationsProperty, value);
        }

        public static readonly BindableProperty LeavingAnimationsProperty =
            BindableProperty.CreateAttached(nameof(LeavingAnimations), typeof(AnimationGroup),
                typeof(VisualState), null, BindingMode.OneWayToSource);



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
}