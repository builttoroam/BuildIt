using System.Collections.Generic;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    [ContentProperty("Animations")]
    public abstract class MultiAnimation : StateAnimation
    {
        public List<StateAnimation> Animations { get; } = new List<StateAnimation>();
    }
}