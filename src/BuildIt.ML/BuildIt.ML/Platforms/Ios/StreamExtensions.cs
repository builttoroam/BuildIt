using Foundation;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace BuildIt.ML
{
    public static class StreamExtensions
    {
        public static async Task<UIImage> ToUIImageAsync(this Stream imageStream)
        {
            var bytes = new byte[imageStream.Length];
            await imageStream.ReadAsync(bytes, 0, bytes.Length);

            return UIImage.LoadFromData(NSData.FromArray(bytes));
        }
    }
}