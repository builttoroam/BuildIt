using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StateByState.Views
{
    public class WindowSizeTrigger : BuildIt.States.Interfaces.StateTriggerBase
    {
        public WindowSizeTrigger(Page page)
        {
            page.SizeChanged += Page_SizeChanged;
        }

        public int MinWidth { get; set; }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateIsActive(e.NewSize.Width >= MinWidth);
        }
    }
}