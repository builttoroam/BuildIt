using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    [ContentProperty(nameof(Animations))]
    public class Storyboard : BindableObject
    {
        /// <summary>
        /// Animations property
        /// </summary>
        public static readonly BindableProperty AnimationsProperty =
            BindableProperty.CreateAttached(nameof(Animations), typeof(IList<StateAnimation>), typeof(VisualState), null, BindingMode.OneWayToSource, null, null, null, null, CreateDefaultAnimations);

        /// <summary>
        /// Gets or sets the target element. Can be set either
        /// in storyboard, or in invidual animations
        /// </summary>
        public Element Element { get; set; }

        /// <summary>
        /// Gets or sets animations to be run prior to a state change
        /// </summary>
        public IList<StateAnimation> Animations
        {
            get => (IList<StateAnimation>)GetValue(AnimationsProperty);
            set => SetValue(AnimationsProperty, value);
        }

        public async Task Animate(CancellationToken cancelToken)
        {
            await VisualStateManager.BuildAnimationTasks(Animations, Element,cancelToken);
        }
        private static object CreateDefaultAnimations(BindableObject bindable)
        {
            return new List<StateAnimation>();
        }
    }
}