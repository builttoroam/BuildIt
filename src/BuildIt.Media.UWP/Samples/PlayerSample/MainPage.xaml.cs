﻿using BuildIt.Media;
using Microsoft.Xaml.Interactivity;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        //private async void MediaControls_ButtonPressed(SystemMediaTransportControls sender,
        //    SystemMediaTransportControlsButtonPressedEventArgs args)
        //{
        //    switch (args.Button)
        //    {
        //        case SystemMediaTransportControlsButton.FastForward:
        //            await Player.SeekAsync(TimeSpan.FromSeconds(10));
        //            break;
        //        case SystemMediaTransportControlsButton.Rewind:
        //            await Player.SeekAsync(TimeSpan.FromSeconds(-10));
        //            break;
        //        case SystemMediaTransportControlsButton.Next:
        //            // Set player to show next video
        //            break;
        //        case SystemMediaTransportControlsButton.Previous:
        //            // Set player to show previous video
        //            break;
        //    }
        //}

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
        //private void MyMediaElement_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        //{
        //    var mp = sender as MediaPlayer;
        //    var newPosition = mp.Position;
        //    if (Math.Abs(LastPosition.Subtract(newPosition).TotalSeconds) < 5) return;
        //    var timeline = new SystemMediaTransportControlsTimelineProperties
        //    {
        //        Position = mp.Position,
        //    };

        //    MediaControls.UpdateTimelineProperties(timeline);
        //}

        private TimeSpan LastPosition { get; set; }
        //private async void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        //{
        //    var updater = MediaControls.DisplayUpdater;

        //    // This may not be auto-detected, and if not set, an error
        //    // will be raised if you attempt to set corresponding attributes
        //    // eg if setting video properties when Type not set to Video
        //    updater.Type = MediaPlaybackType.Video;

        //    updater.VideoProperties.Title = "Big Bunny";
        //    updater.VideoProperties.Subtitle = "Player sample";

        //    var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

        //    updater.Thumbnail = RandomAccessStreamReference.CreateFromFile(storageFile);
        //    updater.Update();

        //    var mp = sender as MediaPlayer;
        //    var timeline = new SystemMediaTransportControlsTimelineProperties
        //    {
        //        StartTime = TimeSpan.FromSeconds(0),
        //        MinSeekTime = TimeSpan.FromSeconds(0),
        //        Position = TimeSpan.FromSeconds(0),
        //        MaxSeekTime = mp.NaturalDuration.TimeSpan,
        //        EndTime = mp.NaturalDuration.TimeSpan
        //    };
        //    MediaControls.UpdateTimelineProperties(timeline);

        //}



        //private void MediaPlayer_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        //{
        //    var mp = sender as MediaPlayer;
        //    var newPosition = mp.Position;
        //    if (Math.Abs(LastPosition.Subtract(newPosition).TotalSeconds) < 5) return;
        //    var timeline = new SystemMediaTransportControlsTimelineProperties
        //    {
        //        Position = mp.Position,
        //    };

        //    MediaControls.UpdateTimelineProperties(timeline);
        //}

        // Create this variable at a global scope. Set it to null.
        private DisplayRequest appDisplayRequest = null;


        private void MyMediaElement_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            if (mediaElement != null && mediaElement.IsAudioOnly == false)
            {
                if (mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    if (appDisplayRequest == null)
                    {
                        // This call creates an instance of the DisplayRequest object. 
                        appDisplayRequest = new DisplayRequest();
                        appDisplayRequest.RequestActive();
                    }
                }
                else // CurrentState is Buffering, Closed, Opening, Paused, or Stopped. 
                {
                    if (appDisplayRequest != null)
                    {
                        // Deactivate the display request and set the var to null.
                        appDisplayRequest.RequestRelease();
                        appDisplayRequest = null;
                    }
                }
            }
        }
    }

//    public class CortanaMediaElementControls : Behavior<MediaElement>
//    {
//        protected double forwordTime = 10;
//        protected double backwordTime = -10;
//        protected double volumeUp = 0.1;
//        protected double volumeDown = -0.1;

//        protected override void OnAttached()
//        {
//            base.OnAttached();

//            PlayerControls.Pause += MyMediaElement_Pause;
//            PlayerControls.Play += MyMediaElement_Play;
//            PlayerControls.Forward += MyMediaElement_Forward;
//            PlayerControls.Backward += MyMediaElement_Backward;
//            PlayerControls.VolumeUp += MyMediaElement_VolumeUp;
//            PlayerControls.VolumeDown += MyMediaElement_VolumeDown;
//            PlayerControls.Mute += MyMediaElement_Mute;
//            PlayerControls.Unmute += MyMediaElement_Unmute;
//            PlayerControls.Help += MyMediaElement_Help;
//        }


//        protected override void OnDetaching()
//        {

//            PlayerControls.Pause -= MyMediaElement_Pause;
//            PlayerControls.Play -= MyMediaElement_Pause;
//            PlayerControls.Forward -= MyMediaElement_Forward;
//            PlayerControls.Backward -= MyMediaElement_Backward;
//            PlayerControls.VolumeUp -= MyMediaElement_VolumeUp;
//            PlayerControls.VolumeDown -= MyMediaElement_VolumeDown;
//            PlayerControls.Mute -= MyMediaElement_Mute;
//            PlayerControls.Unmute -= MyMediaElement_Unmute;
//            PlayerControls.Help -= MyMediaElement_Help;

//            base.OnDetaching();
//        }

//        private MediaElement MediaElement => AssociatedObject;


//        private async void MyMediaElement_Pause(object sender, EventArgs e)
//        {
//            await OnMediaElementPause();
//        }

//#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
//        protected virtual async Task OnMediaElementPause()
//#pragma warning restore CS1998 

//        {
//            MediaElement.Pause();
//        }

//        private async void MyMediaElement_Play(object sender, EventArgs e)
//        {
//            await OnMediaElementPlay();
//        }

//#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
//        protected virtual async Task OnMediaElementPlay()
//#pragma warning restore CS1998 

//        {
//            MediaElement.Play();
//        }

//        private async void MyMediaElement_Forward(object sender, EventArgs e)
//        {
//            await OnMediaElementForward();
//        }

//        protected virtual Task OnMediaElementForward()
//        {
//            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
//            MediaElement.Position += TimeSpan.FromSeconds(forwordTime);
//            return Task.FromResult(true);
//        }

//        private async void MyMediaElement_Backward(object sender, EventArgs e)
//        {
//            await OnMediaElementBackward();
//        }

//        protected virtual Task OnMediaElementBackward()
//        {
//            //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
//            MediaElement.Position = TimeSpan.FromSeconds(backwordTime);
//            return Task.FromResult(true);
//        }

//        private async void MyMediaElement_VolumeUp(object sender, EventArgs e)
//        {
//            await OnMediaElementVolumeUp();
//        }

//        protected virtual Task OnMediaElementVolumeUp()
//        {
//            if (volumeUp == 0) return Task.FromResult(true);
//            MediaElement.Volume += volumeUp;
//            return Task.FromResult(true);
//        }

//        private async void MyMediaElement_VolumeDown(object sender, EventArgs e)
//        {
//            await OnMediaElementVolumeDown();
//        }

//        protected virtual Task OnMediaElementVolumeDown()
//        {
//            if (volumeDown == 0) return Task.FromResult(true);
//            MediaElement.Volume += volumeDown;
//            return Task.FromResult(true);
//        }
//        private async void MyMediaElement_Mute(object sender, EventArgs e)
//        {
//            await OnMediaElementMute();
//        }

//        protected virtual Task OnMediaElementMute()
//        {
//            MediaElement.IsMuted = true;
//            return Task.FromResult(true);
//        }

//        private async void MyMediaElement_Unmute(object sender, EventArgs e)
//        {
//            await OnMediaElementMute();
//        }

//        protected virtual Task OnMediaElementUnmute()
//        {
//            MediaElement.IsMuted = false;
//            return Task.FromResult(true);
//        }

//        private async void MyMediaElement_Help(object sender, EventArgs e)
//        {
//            await OnMediaElementMute();
//        }

//        protected virtual Task OnMediaElementHelp()
//        {
//            return Task.FromResult(true);
//        }
//    }


    #region For playerFramework
    // For playerFramework
    //    public class CortanaControls : Behavior<MediaPlayer>
    //    {
    //        protected double forwordTime = 10;
    //        protected double backwordTime = -10;
    //        protected override void OnAttached()
    //        {
    //            base.OnAttached();

    //            PlayerControls.Pause += PlayerControls_Pause;
    //            PlayerControls.Play += PlayerControls_Play;
    //            PlayerControls.Forward += PlayerControls_Forward;
    //            PlayerControls.Backward += PlayerControls_Backward;

    //        }

    //        protected override void OnDetaching()
    //        {

    //            PlayerControls.Pause -= PlayerControls_Pause;
    //            PlayerControls.Play -= PlayerControls_Play;
    //            PlayerControls.Forward -= PlayerControls_Forward;
    //            PlayerControls.Backward -= PlayerControls_Backward;

    //            base.OnDetaching();
    //        }

    //        private MediaPlayer Player
    //        {
    //            get { return AssociatedObject; }
    //        }

    //        private async void PlayerControls_Pause(object sender, EventArgs e)
    //        {
    //            await OnPlayerPause();
    //        }

    //#pragma warning disable CS1998 // This is async because it may be overrridden and want to allow for async methods
    //        protected virtual async Task OnPlayerPause()
    //#pragma warning restore CS1998

    //        {
    //            Player.Pause();
    //        }

    //        private async void PlayerControls_Play(object sender, EventArgs e)
    //        {
    //            await OnPlayerPlay();
    //        }


    //#pragma warning disable 1998 // This is async because it may be overrridden and want to allow for async methods
    //        protected virtual async Task OnPlayerPlay()
    //#pragma warning restore 1998
    //        {
    //            Player.Play();
    //        }

    //        private async void PlayerControls_Forward(object sender, EventArgs e)
    //        {
    //            await OnPlayerPlay();
    //        }

    //        protected virtual async Task OnPlayerForward()
    //        {
    //            await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
    //        }



    //        private async void PlayerControls_Backward(object sender, EventArgs e)
    //        {
    //            await OnPlayerBackward();
    //        }

    //        protected virtual async Task OnPlayerBackward()
    //        {
    //            await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
    //        }
    //    }

    public class CustomCortanaControls : CortanaMediaElementActions
    {
        protected override async Task OnMediaElementPause()
        {
            // Do my own pausing - but don't call base
            await base.OnMediaElementPause();
            // base.OnPlayerPause();
        }

        protected override async Task OnMediaElementPlay()
        {
            await base.OnMediaElementPlay();
        }

        protected override async Task OnMediaElementForward()
        {
            await base.OnMediaElementForward();
        }

        protected override async Task OnMediaElementBackward()
        {
            await base.OnMediaElementBackward();
        }
    }
    #endregion


}