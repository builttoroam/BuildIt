using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using CognitiveServicesDemo.Service.Interfaces;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Platform;
using Xamarin.Forms;
using Com.Microsoft.Bing.Speech;
using Com.Microsoft.SpeechSDK;
using Com.Microsoft.Cognitiveservices.Speechrecognition;
using System.Linq;

namespace CognitiveServicesDemo.Droid
{
    public enum FinalResponseStatus { NotReceived, OK, Timeout }

    [Activity(Label = "Cognitive Service", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity//, ISpeechRecognitionServerEvents, IBingSpeech
    {
        //private readonly Com.Microsoft.Cognitiveservices.Speechrecognition.SpeechRecognitionMode speechMode = Com.Microsoft.Cognitiveservices.Speechrecognition.SpeechRecognitionMode.LongDictation; //speech mode  short for20，long for200
        //string locale = "en-au"; //language
        //string locale2 = "zh-CN";
        //string key = "1e7fe3d56350428197a55f6873c45269";
        //DataRecognitionClient dataClient = null;
        //MicrophoneRecognitionClient micClient = null;
        //FinalResponseStatus isReceivedResponse = FinalResponseStatus.NotReceived;
        //Com.Microsoft.Cognitiveservices.Speechrecognition.SpeechRecognitionMode

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            var mvxFormsApp = new MvxFormsApp();
            LoadApplication(mvxFormsApp);

            var presenter = Mvx.Resolve<IMvxViewPresenter>() as MvxFormsDroidPagePresenter;
            presenter.MvxFormsApp = mvxFormsApp;
            //Mvx.RegisterType<IBingSpeech, MainActivity>();

            Mvx.Resolve<IMvxAppStart>().Start();
            //App.Init(this);
        }

        private Action<string> sepeechende;
        //public void StartSpeech(Action<string> action)
        //{
        //    sepeechende = action;
        //    if (this.micClient == null)
        //    {
        //        this.micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
        //                this,
        //                speechMode,
        //                locale,
        //                this,
        //                key);

        //        // micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(speechMode, locale, key);

        //    }

        //    this.micClient.StartMicAndRecognition();
        //}

        /// <summary>
        /// * Called when the microphone status has changed.
        /// * @param recording The current recording state
        /// </summary>
        /// <param name="p0">If set to <c>true</c> p0.</param>
        public void OnAudioEvent(bool p0)
        {
            this.WriteLine("--- Microphone status change received by onAudioEvent() ---");
            this.WriteLine("********* Microphone status: " + p0 + " *********");
            if (p0)
            {
                this.WriteLine("Please start speaking.");
            }

            WriteLine();
            if (!p0)
            {
                //this.micClient.EndMicAndRecognition();
            }
        }

        public void OnError(int p0, string p1)
        {
            this.WriteLine("--- Error received by onError() ---");
            this.WriteLine("Error code: " + p0);
            this.WriteLine("Error text: " + p1);
            this.WriteLine();
        }

        //public void OnFinalResponseReceived(RecognitionResult p0)
        //{
        //    bool isFinalDicationMessage = speechMode == SpeechRecognitionMode.LongDictation &&
        //        (p0.RecognitionStatus == RecognitionStatus.EndOfDictation ||
        //                p0.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout);

        //    if (isFinalDicationMessage)
        //    {
        //        // we got the final result, so it we can end the mic reco.  No need to do this
        //        // for dataReco, since we already called endAudio() on it as soon as we were done
        //        // sending all the data.
        //        this.micClient.EndMicAndRecognition();
        //    }
        //    if (isFinalDicationMessage)
        //    {
        //        //this._startButton.setEnabled (true);
        //        this.isReceivedResponse = FinalResponseStatus.OK;
        //    }

        //    if (!isFinalDicationMessage)
        //    {
        //        this.WriteLine("********* Final n-BEST Results *********");
        //        string allStr = string.Join("", p0.Results.Select(o => o.DisplayText).ToList());
        //        for (int i = 0; i < p0.Results.Count; i++)
        //        {
        //            this.WriteLine("[" + i + "]" + " Confidence=" + p0.Results[i].Confidence +
        //                    " Text=\"" + p0.Results[i].DisplayText + "\"");
        //        }
        //        //sepeechende?.Invoke(allStr);
        //        this.micClient.EndMicAndRecognition();
        //        this.WriteLine();
        //    }
        //}


        /// <summary>
        /// Called when a final response is received and its intent is parsed
        /// </summary>
        /// <param name="p0">P0.</param>
        public void OnIntentReceived(string p0)
        {
            this.WriteLine("--- Intent received by onIntentReceived() ---");
            this.WriteLine(p0);
            this.WriteLine();

            //string allStr = string.Join("", p0.Results.Select(o => o.DisplayText).ToList());

        }

        public void OnPartialResponseReceived(string p0)
        {
            this.WriteLine("--- Partial result received by onPartialResponseReceived() ---");
            this.WriteLine(p0);
            this.WriteLine();
            sepeechende?.Invoke(p0);
        }


        public void WriteLine(string s = "")
        {
            System.Diagnostics.Debug.WriteLine(s);

        }
    }
}

