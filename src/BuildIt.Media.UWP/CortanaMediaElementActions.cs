using Microsoft.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BuildIt.Media
{
    public class CortanaMediaElementActions : Behavior<MediaElement>
    {
        public int SeekForwardIncrement { get; set; } = 10;
        public int SeekBackwardIncrement { get; set; } = -10;
        public double VolumeUpIncrement { get; set; } = 0.1;
        public double VolumeDownIncrement { get; set; } = -0.1;

        public bool IsPauseEnabled { get; set; } = true;
        public bool IsPlayEnabled { get; set; } = true;
        public bool IsSeekForwardEnabled { get; set; } = true;
        public bool IsSeekBackEnabled { get; set; } = true;
        public bool IsVolumeUpEnabled { get; set; } = true;
        public bool IsVolumeDownEnabled { get; set; } = true;
        public bool IsMuteEnabled { get; set; } = true;
        public bool IsUnmuteEnabled { get; set; } = true;

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
            if (IsVolumeUpEnabled)
            {
                PlayerControls.VolumeUp += MediaElement_VolumeUp;
            }
            if (IsVolumeDownEnabled)
            {
                PlayerControls.VolumeDown += MediaElement_VolumeDown;
            }
            if (IsMuteEnabled)
            {
                PlayerControls.Mute += MediaElement_Mute;
            }
            if (IsUnmuteEnabled)
            {
                PlayerControls.Unmute += MediaElement_Unmute;
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
            if (IsVolumeUpEnabled)
            {
                PlayerControls.VolumeUp -= MediaElement_VolumeUp;
            }
            if (IsVolumeDownEnabled)
            {
                PlayerControls.VolumeDown -= MediaElement_VolumeDown;
            }
            if (IsMuteEnabled)
            {
                PlayerControls.Mute -= MediaElement_Mute;
            }
            if (IsUnmuteEnabled)
            {
                PlayerControls.Unmute -= MediaElement_Unmute;
            }
            base.OnDetaching();
        }

        private MediaElement MediaElement => AssociatedObject;


        private async void MediaElement_Pause(object sender, EventArgs e)
        {
            if (!IsPauseEnabled) return;
            await OnMediaElementPause();
        }

        protected virtual Task OnMediaElementPause()
        {
            MediaElement.Pause();
            return Task.FromResult(true);
        }

        private async void MediaElement_Play(object sender, EventArgs e)
        {
            if (!IsPlayEnabled) return;
            await OnMediaElementPlay();
        }

        protected virtual Task OnMediaElementPlay()
        {
            MediaElement.Play();
            return Task.FromResult(true);
        }

        private async void MediaElement_Forward(object sender, EventArgs e)
        {
            if (!IsSeekForwardEnabled) return;
            await OnMediaElementForward();
        }

        protected virtual Task OnMediaElementForward()
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
            if (SeekForwardIncrement == 0) return Task.FromResult(true);
            MediaElement.Position += TimeSpan.FromSeconds(SeekForwardIncrement);
            return Task.FromResult(true);
        }

        private async void MediaElement_Backward(object sender, EventArgs e)
        {
            if (!IsSeekBackEnabled) return;
            await OnMediaElementBackward();
        }


        protected virtual Task OnMediaElementBackward()
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
            if (SeekBackwardIncrement == 0) return Task.FromResult(true);
            MediaElement.Position += TimeSpan.FromSeconds(SeekBackwardIncrement);
            return Task.FromResult(true);
        }

        private async void MediaElement_VolumeUp(object sender, EventArgs e)
        {
            if (!IsVolumeUpEnabled) return;
            await OnMediaElementVolumeUp();

        }
        protected virtual Task OnMediaElementVolumeUp()
        {
            if (VolumeUpIncrement == 0) return Task.FromResult(true);
            MediaElement.Volume += VolumeUpIncrement;
            return Task.FromResult(true);
        }

        private async void MediaElement_VolumeDown(object sender, EventArgs e)
        {
            if (!IsVolumeDownEnabled) return;
            await OnMediaElementVolumeDown();
        }

        protected virtual Task OnMediaElementVolumeDown()
        {
            if (VolumeDownIncrement == 0) return Task.FromResult(true);
            MediaElement.Volume += VolumeDownIncrement;
            return Task.FromResult(true);
        }

        private async void MediaElement_Mute(object sender, EventArgs e)
        {
            if (!IsVolumeDownEnabled) return;
            await OnMediaElementMute();
        }

        protected virtual Task OnMediaElementMute()
        {
            MediaElement.IsMuted = true;
            return Task.FromResult(true);
        }

        private async void MediaElement_Unmute(object sender, EventArgs e)
        {
            if (!IsVolumeDownEnabled) return;
            await OnMediaElementUnmute();
        }

        protected virtual Task OnMediaElementUnmute()
        {
            MediaElement.IsMuted = false;
            return Task.FromResult(true);
        }
    }
}