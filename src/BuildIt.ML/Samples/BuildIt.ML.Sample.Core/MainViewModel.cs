using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BuildIt.ML.Sample.Core
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string classifications;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Classifications
        {
            get => classifications;
            set
            {
                if (value != classifications)
                {
                    classifications = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public async Task InitAsync()
        {
            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
        }

        public async Task ClassifyAsync()
        {
            await CrossCustomVisionClassifier.Instance.InitAsync("Currency", new[] { "FivePounds", "TenPounds" });
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
            var imageClassifications = await CrossCustomVisionClassifier.Instance.ClassifyAsync(file.GetStream());
            Classifications = string.Join(",", imageClassifications.Select(c => $"{c.Label} {c.Confidence}"));
        }

        public async Task ClassifyAsync(object frame)
        {
            await CrossCustomVisionClassifier.Instance.InitAsync("fruits", new[] { "apple", "banana", "pineapple" });
            //await CrossCustomVisionClassifier.Instance.InitAsync("Currency", new[] { "FivePounds", "TenPounds" });
            var imageClassifications = await CrossCustomVisionClassifier.Instance.ClassifyNativeFrameAsync(frame);
            Classifications = string.Join("\n", imageClassifications.Select(c => $"{c.Label} {c.Confidence}"));
        }

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}