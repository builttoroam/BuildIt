using System;
using System.Threading.Tasks;

namespace PlayerSample
{
    public static class PlayerControls
    {
        public static event EventHandler Pause;
        public static event EventHandler Play;
        public static event EventHandler Forward;
        public static event EventHandler Backward;
        
        public static async void Action(string actionName)
        {
            await Task.Delay(1000);
            switch ((actionName + "").ToLower())
            {
                case "pause":
                    Pause?.Invoke(null, EventArgs.Empty);
                    break;
                case "play":
                    Play?.Invoke(null, EventArgs.Empty);
                    break;
                case "forward":
                    Forward?.Invoke(null, EventArgs.Empty);
                    break;
                case "back":
                    Backward?.Invoke(null, EventArgs.Empty);
                    break;
            }
        }
    }
}