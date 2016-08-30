using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace CortanaControl
{
    public class CortanaForMediaElement
    {
        private SystemMediaTransportControls MediaControls { get; }
        private TimeSpan LastPosition { get; set; }


        public async void MediaElement_OnMediaOpended(object sender, RoutedEventArgs e)
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
        //public void MediaElement_OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
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

        public class CortanaControls : Behavior<MediaElement>
        {
            protected double forwordTime = 10;
            protected double backwordTime = -10;

            protected override void OnAttached()
            {
                base.OnAttached();

                PlayerControls.Pause += MediaElement_Pause;
                PlayerControls.Play += MediaElement_Play;
                PlayerControls.Forward += MediaElement_Forward;
                PlayerControls.Backward += MediaElement_Backward;

            }

            protected override void OnDetaching()
            {

                PlayerControls.Pause -= MediaElement_Pause;
                PlayerControls.Play -= MediaElement_Pause;
                PlayerControls.Forward -= MediaElement_Forward;
                PlayerControls.Backward -= MediaElement_Backward;

                base.OnDetaching();
            }

            private MediaElement MediaElement => AssociatedObject;


            private async void MediaElement_Pause(object sender, EventArgs e)
            {
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
                await OnMediaElementForward();
            }

#pragma warning disable 1998
            protected virtual async Task OnMediaElementForward()
#pragma warning restore 1998
            {
                //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(forwordTime)));
                MediaElement.Position += TimeSpan.FromSeconds(forwordTime);

            }

            private async void MediaElement_Backward(object sender, EventArgs e)
            {
                await OnMediaElementBackward();
            }


#pragma warning disable 1998
            protected virtual async Task OnMediaElementBackward()
#pragma warning restore 1998
            {
                //await Player.SeekAsync(Player.Position.Add(TimeSpan.FromSeconds(backwordTime)));
                MediaElement.Position = TimeSpan.FromSeconds(backwordTime);
            }
        }
    }
}