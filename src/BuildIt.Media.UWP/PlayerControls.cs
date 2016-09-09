using System;
using System.Threading.Tasks;

namespace BuildIt.Media
{
    public class PlayerControls
    {
        public static event EventHandler Pause;
        public static event EventHandler Play;
        public static event EventHandler Forward;
        public static event EventHandler Backward;
        public static event EventHandler VolumeUp;
        public static event EventHandler VolumeDown;
        public static event EventHandler Mute;
        public static event EventHandler Unmute;
        //public static event EventHandler Help;

        public static async Task<bool> Action(string actionName)
        {
            try
            {
                await Task.Delay(1000);
                switch ((actionName + "").ToLower())
                {
                    case "buildit_pause":
                        Pause?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_play":
                        Play?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_forward":
                        Forward?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_back":
                        Backward?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_volumeup":
                        VolumeUp?.Invoke(null,EventArgs.Empty);
                        break;
                    case "buildit_volumedown":
                        VolumeDown?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_mute":
                        Mute?.Invoke(null, EventArgs.Empty);
                        break;
                    case "buildit_unmute":
                        Unmute?.Invoke(null, EventArgs.Empty);
                        break;
                    //case "buildit_help":
                    //    Help?.Invoke(null,EventArgs.Empty);
                    //    break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }
    }
}