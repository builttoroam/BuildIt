using System.Threading;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Trigger action to run a storyboard.
    /// </summary>
    public class StoryboardTriggerAction : TriggerAction<Button>
    {
        /// <summary>
        /// Gets or sets animations to be run prior to a state change.
        /// </summary>
        public Storyboard Storyboard
        {
            get;
            set;
        }

        /// <summary>
        /// Application developers override this method to provide the action that is performed when the trigger condition is met.
        /// </summary>
        /// <param name="entry">The button to invoke the action on.</param>
        protected override async void Invoke(Button entry)
        {
            if (Storyboard == null)
            {
                return;
            }

            await Storyboard.Animate(CancellationToken.None);
        }
    }
}