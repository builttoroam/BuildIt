using BuildIt.Forms.Animations;
using BuildIt.States.Interfaces;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Represents a visual state.
    /// </summary>
    public class VisualState : BindableObject
    {
        /// <summary>
        /// List of setters to be applied for a state.
        /// </summary>
        public static readonly BindableProperty SettersProperty =
            BindableProperty.CreateAttached(nameof(Setters), typeof(IList<Setter>), typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultValue);

        /// <summary>
        /// Animations for arriving at a state.
        /// </summary>
        public static readonly BindableProperty ArrivingAnimationsProperty =
            BindableProperty.CreateAttached(nameof(ArrivingAnimations), typeof(AnimationGroup), typeof(VisualState), null, BindingMode.OneWayToSource);

        /// <summary>
        /// Animations for leaving a state.
        /// </summary>
        public static readonly BindableProperty LeavingAnimationsProperty =
            BindableProperty.CreateAttached(nameof(LeavingAnimations), typeof(AnimationGroup), typeof(VisualState), null, BindingMode.OneWayToSource);

        /// <summary>
        /// Gets or sets the name of the state.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the state group that the corresponding state definition belongs to.
        /// </summary>
        public IStateGroup StateGroup { get; set; }

        /// <summary>
        /// Gets or sets the animations for arriving at the state.
        /// </summary>
        public AnimationGroup ArrivingAnimations
        {
            get => (AnimationGroup)GetValue(ArrivingAnimationsProperty);
            set => SetValue(ArrivingAnimationsProperty, value);
        }

        /// <summary>
        /// Gets or sets the animatinos for leaving the state.
        /// </summary>
        public AnimationGroup LeavingAnimations
        {
            get => (AnimationGroup)GetValue(LeavingAnimationsProperty);
            set => SetValue(LeavingAnimationsProperty, value);
        }

        /// <summary>
        /// Gets the list of setters.
        /// </summary>
        public IList<Setter> Setters => (IList<Setter>)GetValue(SettersProperty);

        private static object CreateDefaultValue(BindableObject bindable)
        {
            return new List<Setter>();
        }
    }
}