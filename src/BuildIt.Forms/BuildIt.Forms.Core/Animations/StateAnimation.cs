using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public abstract class StateAnimation : TargettedStateAction
    {
        public abstract Task Animate(VisualElement visualElement);
    }
}