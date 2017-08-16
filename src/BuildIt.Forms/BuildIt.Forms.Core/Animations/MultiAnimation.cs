using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Base multi-animation class (see ParallelAnimation and SequenceAnimation)
    /// </summary>
    [ContentProperty("Animations")]
    public abstract class MultiAnimation : StateAnimation
    {
        /// <summary>
        /// Gets the list of animations
        /// </summary>
        public List<StateAnimation> Animations { get; } = new List<StateAnimation>();
    }
}