using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class AnimationGroup : BindableObject
    {
        public IList<StateAnimation> PreAnimations
        {
            get => (IList<StateAnimation>)GetValue(PreAnimationsProperty);
            set => SetValue(PreAnimationsProperty, value);
        }

        public static readonly BindableProperty PreAnimationsProperty =
            BindableProperty.CreateAttached(nameof(PreAnimations), typeof(IList<StateAnimation>),
                typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);


        public IList<StateAnimation> PostAnimations
        {
            get => (IList<StateAnimation>)GetValue(PostAnimationsProperty);
            set => SetValue(PostAnimationsProperty, value);
        }

        public static readonly BindableProperty PostAnimationsProperty =
            BindableProperty.CreateAttached(nameof(PostAnimations), typeof(IList<StateAnimation>),
                typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);
        private static object CreateDefaultAnimations(BindableObject bindable)
        {
            return new List<StateAnimation>();
        }

    }
}