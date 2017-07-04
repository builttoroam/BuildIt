using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
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

    public abstract class StateAnimation : TargettedStateAction
    {
        public abstract Task Animate(VisualElement visualElement);
    }
}