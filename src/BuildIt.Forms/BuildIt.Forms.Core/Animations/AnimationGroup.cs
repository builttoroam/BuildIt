using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Represents a group of animations - includes two groups: pre and post animations
    /// </summary>
    public class AnimationGroup : BindableObject
    {
        /// <summary>
        /// PreAnimations property
        /// </summary>
        public static readonly BindableProperty PreAnimationsProperty =
            BindableProperty.CreateAttached(nameof(PreAnimations), typeof(IList<StateAnimation>), typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);

        /// <summary>
        /// PostAnimations property
        /// </summary>
        public static readonly BindableProperty PostAnimationsProperty =
            BindableProperty.CreateAttached(nameof(PostAnimations), typeof(IList<StateAnimation>), typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);

        /// <summary>
        /// Gets or sets animations to be run prior to a state change
        /// </summary>
        public IList<StateAnimation> PreAnimations
        {
            get => (IList<StateAnimation>)GetValue(PreAnimationsProperty);
            set => SetValue(PreAnimationsProperty, value);
        }

        /// <summary>
        /// Gets or sets animations to be run after a state change
        /// </summary>
        public IList<StateAnimation> PostAnimations
        {
            get => (IList<StateAnimation>)GetValue(PostAnimationsProperty);
            set => SetValue(PostAnimationsProperty, value);
        }

        private static object CreateDefaultAnimations(BindableObject bindable)
        {
            return new List<StateAnimation>();
        }
    }
}