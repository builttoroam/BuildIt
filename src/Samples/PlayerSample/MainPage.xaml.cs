using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework;
using Microsoft.Xaml.Interactivity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PlayerSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SystemMediaTransportControls MediaControls { get; }

        public MainPage()
        {
            InitializeComponent();

            MediaControls = SystemMediaTransportControls.GetForCurrentView();

            MediaControls.IsNextEnabled = true;
            MediaControls.IsPreviousEnabled = true;

            MediaControls.IsFastForwardEnabled = true;
            MediaControls.IsRewindEnabled = true;

            //MediaControls.ButtonPressed += MediaControls_ButtonPressed;
        }

        private async void MediaControls_ButtonPressed(SystemMediaTransportControls sender,
            SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.FastForward:
                    await Player.SeekAsync(TimeSpan.FromSeconds(10));
                    break;
                case SystemMediaTransportControlsButton.Rewind:
                    await Player.SeekAsync(TimeSpan.FromSeconds(-10));
                    break;
                case SystemMediaTransportControlsButton.Next:
                    // Set player to show next video
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    // Set player to show previous video
                    break;
            }
        }

        private async void MyMediaElement_OnMediaOpened(object sender, RoutedEventArgs e)
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

            var me = sender as MediaElement;
            var timeline = new SystemMediaTransportControlsTimelineProperties
            {
                StartTime = TimeSpan.FromSeconds(0),
                MinSeekTime = TimeSpan.FromSeconds(0),
                Position = TimeSpan.FromSeconds(0),
                MaxSeekTime = me.NaturalDuration.TimeSpan,
                EndTime = me.NaturalDuration.TimeSpan
            };
            MediaControls.UpdateTimelineProperties(timeline);

        }
        private void MyMediaElement_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
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

        private TimeSpan LastPosition { get; set; }
        private async void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
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
                MaxSeekTime = mp.NaturalDuration.TimeSpan,
                EndTime = mp.NaturalDuration.TimeSpan
            };
            MediaControls.UpdateTimelineProperties(timeline);

        }



        private void MediaPlayer_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
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

    public class CortanaMediaElementControls : Behavior<MediaElement>
    {
        protected double forwordTime = 10;
        protected double backwordTime = -10;

        protected override void OnAttached()
        {
            base.OnAttached();

            PlayerControls.Pause += MyMediaElement_Pause;
            PlayerControls.Play += MyMediaElement_Play;
            PlayerControls.Forward += MyMediaElement_Forward;
            PlayerControls.Backward += MyMediaElement_Backward;

        }

        protected override void OnDetaching()
        {

            PlayerControls.Pause -= MyMediaElement_Pause;
            PlayerControls.Play -= MyMediaElement_Pause;
            PlayerControls.Forward -= MyMediaElement_Forward;
            PlayerControls.Backward -= MyMediaElement_Backward;

            base.OnDetaching();
        }

        private MediaElement MediaElement => AssociatedObject;


        private async void MyMediaElement_Pause(object sender, EventArgs e)
        {
            await OnMediaElementPause();
        }

#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnMediaElementPause()
#pragma warning restore CS1998 

        {
            MediaElement.Pause();
        }

        private async void MyMediaElement_Play(object sender, EventArgs e)
        {
            await OnMediaElementPlay();
        }

#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnMediaElementPlay()
#pragma warning restore CS1998 

        {
            MediaElement.Play();
        }

        private async void MyMediaElement_Forward(object sender, EventArgs e)
        {
            await OnMediaElementForward();
        }

        protected virtual async Task OnMediaElementForward()
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
            MediaElement.Position += TimeSpan.FromSeconds(forwordTime);

        }

        private async void MyMediaElement_Backward(object sender, EventArgs e)
        {
            await OnMediaElementBackward();
        }

        protected virtual async Task OnMediaElementBackward()
        {
            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
            MediaElement.Position = TimeSpan.FromSeconds(backwordTime);
        }
    }



    // For playerFramework
    public class CortanaControls : Behavior<MediaPlayer>
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

        private MediaPlayer Player
        {
            get { return AssociatedObject; }
        }

        private async void PlayerControls_Pause(object sender, EventArgs e)
        {
            await OnPlayerPause();
        }

#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnPlayerPause()
#pragma warning restore CS1998 

        {
            Player.Pause();
        }

        private async void PlayerControls_Play(object sender, EventArgs e)
        {
            await OnPlayerPlay();
        }


#pragma warning disable 1998 // This is async because it may be overrridden and want to allow for async methods
        protected virtual async Task OnPlayerPlay()
#pragma warning restore 1998
        {
            Player.Play();
        }

        private async void PlayerControls_Forward(object sender, EventArgs e)
        {
            await OnPlayerPlay();
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

    public class CustomCortanaControls : CortanaControls
    {
        protected override async Task OnPlayerPause()
        {
            // Do my own pausing - but don't call base
            await base.OnPlayerPause();
            // base.OnPlayerPause();
        }

        protected override async Task OnPlayerPlay()
        {
            await base.OnPlayerPlay();
        }

        protected override async Task OnPlayerForward()
        {
            await base.OnPlayerForward();
        }

        protected override async Task OnPlayerBackward()
        {
            await base.OnPlayerBackward();
        }
    }

}