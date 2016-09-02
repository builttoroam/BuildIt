using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace BuildIt.Media
{
    public class CortanaMediaElementActions : Behavior<MediaElement>
    {
        public int SeekForwardIncrement { get; set; } = 10;
        public int SeekBackwardIncrement { get; set; } = -10;

        public bool IsPauseEnabled { get; set; } = true;
        public bool IsPlayEnabled { get; set; } = true;
        public bool IsSeekForwardEnabled { get; set; } = true;
        public bool IsSeekBackEnabled { get; set; } = true;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (IsPauseEnabled)
            {
                PlayerControls.Pause += MediaElement_Pause;
            }
            if (IsPlayEnabled)
            {
                PlayerControls.Play += MediaElement_Play;
            }

            if (IsSeekForwardEnabled)
            {
                PlayerControls.Forward += MediaElement_Forward;
            }
            if (IsSeekBackEnabled)
            {
                PlayerControls.Backward += MediaElement_Backward;
            }

        }

        protected override void OnDetaching()
        {
            if (IsPauseEnabled)
            {
                PlayerControls.Pause -= MediaElement_Pause;
            }
            if (IsPlayEnabled)
            {
                PlayerControls.Play -= MediaElement_Pause;
            }

            if (IsSeekForwardEnabled)
            {
                PlayerControls.Forward -= MediaElement_Forward;
            }
            if (IsSeekBackEnabled)
            {
                PlayerControls.Backward -= MediaElement_Backward;
            }
            base.OnDetaching();
        }

        private MediaElement MediaElement => AssociatedObject;


        private async void MediaElement_Pause(object sender, EventArgs e)
        {
            if (!IsPauseEnabled) return;
            await OnMediaElementPause();
        }

#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnMediaElementPause()
#pragma warning restore CS1998

        {
            MediaElement.Pause();
        }

        private async void MediaElement_Play(object sender, EventArgs e)
        {
            if (!IsPlayEnabled) return;
            await OnMediaElementPlay();
        }

#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnMediaElementPlay()
#pragma warning restore CS1998

        {
            MediaElement.Play();
        }

        private async void MediaElement_Forward(object sender, EventArgs e)
        {
            if(!IsSeekForwardEnabled) return;
            await OnMediaElementForward();
        }

#pragma warning disable 1998
        protected virtual async Task OnMediaElementForward()
#pragma warning restore 1998
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
            if (SeekForwardIncrement == 0) return;
            MediaElement.Position += TimeSpan.FromSeconds(SeekForwardIncrement);

        }

        private async void MediaElement_Backward(object sender, EventArgs e)
        {
            if(!IsSeekBackEnabled) return;
            await OnMediaElementBackward();
        }


#pragma warning disable 1998
        protected virtual async Task OnMediaElementBackward()
#pragma warning restore 1998
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
            if (SeekBackwardIncrement == 0) return;
            MediaElement.Position += TimeSpan.FromSeconds(SeekBackwardIncrement);
        }
    }
}