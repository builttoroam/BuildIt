using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace CortanaControl
{
    public class CortanaForPlayerFramework
    {
        private SystemMediaTransportControls MediaControls { get; }
        private TimeSpan LastPosition { get; set; }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async void MediaControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.FastForward:
                    //await Player.SeekAsync(TimeSpan.FromSeconds(10));
                    break;
                case SystemMediaTransportControlsButton.Rewind:
                    //await Player.SeekAsync(TimeSpan.FromSeconds(-10));
                    break;
                case SystemMediaTransportControlsButton.Next:
                    // Set player to show next video
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    // Set player to show previous video
                    break;
            }
        }

        public async void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            var updater = MediaControls.DisplayUpdater;

            // This may not be auto-detected, and if not set, an error
            // will be raised if you attempt to set corresponding attributes
            // eg if setting video properties when Type not set to Video
            updater.Type = MediaPlaybackType.Video;

            updater.VideoProperties.Title = "Big Bunny";
            updater.VideoProperties.Subtitle = "Player sample";

            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

            updater.Thumbnail = RandomAccessStreamReference.CreateFromFile(storageFile);
            updater.Update();

            var mp = sender as MediaPlayer;
            var timeline = new SystemMediaTransportControlsTimelineProperties
            {
                StartTime = TimeSpan.FromSeconds(0),
                MinSeekTime = TimeSpan.FromSeconds(0),
                Position = TimeSpan.FromSeconds(0),
                MaxSeekTime = mp.NaturalDuration.Duration(),
                EndTime = mp.NaturalDuration.Duration()
            };
            MediaControls.UpdateTimelineProperties(timeline);

        }

        public void MediaPlayer_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            var mp = sender as MediaPlayer;
            var newPosition = mp.Position;
            if (Math.Abs(LastPosition.Subtract(newPosition).TotalSeconds) < 5) return;
            var timeline = new SystemMediaTransportControlsTimelineProperties
            {
                Position = mp.Position,
            };

            MediaControls.UpdateTimelineProperties(timeline);
        }

    }
    public class CortanaControls : Behavior<Microsoft.PlayerFramework.MediaPlayer>
    {
        protected double forwordTime = 10;
        protected double backwordTime = -10;
        protected override void OnAttached()
        {
            base.OnAttached();

            PlayerControls.Pause += PlayerControls_Pause;
            PlayerControls.Play += PlayerControls_Play;
            PlayerControls.Forward += PlayerControls_Forward;
            PlayerControls.Backward += PlayerControls_Backward;

        }

        protected override void OnDetaching()
        {

            PlayerControls.Pause -= PlayerControls_Pause;
            PlayerControls.Play -= PlayerControls_Play;
            PlayerControls.Forward -= PlayerControls_Forward;
            PlayerControls.Backward -= PlayerControls_Backward;

            base.OnDetaching();
        }

        private Microsoft.PlayerFramework.MediaPlayer Player
        {
            get { return AssociatedObject; }
        }

        private async void PlayerControls_Pause(object sender, EventArgs e)
        {
            await OnPlayerPause();
        }

        protected virtual Task OnPlayerPause()
        {
            Player.Pause();
            return Task.FromResult(true);
        }

        private async void PlayerControls_Play(object sender, EventArgs e)
        {
            await OnPlayerPlay();
        }


        protected virtual Task OnPlayerPlay()
        {
            Player.Play();
            return Task.FromResult(true);
        }

        private async void PlayerControls_Forward(object sender, EventArgs e)
        {
            await OnPlayerForward();
        }

        protected virtual async Task OnPlayerForward()
        {
            await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
        }
        
        private async void PlayerControls_Backward(object sender, EventArgs e)
        {
            await OnPlayerBackward();
        }

        protected virtual async Task OnPlayerBackward()
        {
            await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
        }
    }
}