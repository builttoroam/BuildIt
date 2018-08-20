using Android.Hardware.Camera2;
using Java.Lang;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraCaptureListener(CameraPreviewControlRenderer owner)
        {
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            Process(result);
        }

        public override void OnCaptureProgressed(CameraCaptureSession session, CaptureRequest request, CaptureResult partialResult)
        {
            Process(partialResult);
        }

        private void Process(CaptureResult result)
        {
            switch (owner.State)
            {
                case CameraPreviewControlRenderer.StateWaitingLock:
                    {
                        Integer afState = (Integer)result.Get(CaptureResult.ControlAfState);
                        if (afState == null)
                        {
                            owner.CaptureStillPicture();
                            return;
                        }

                        var controlAFState = (ControlAFState)afState.IntValue();
                        if ((controlAFState == ControlAFState.FocusedLocked) ||
                                   (controlAFState == ControlAFState.NotFocusedLocked))
                        {
                            // ControlAeState can be null on some devices
                            Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                            if (aeState == null ||
                                    aeState.IntValue() == ((int)ControlAEState.Converged))
                            {
                                owner.State = CameraPreviewControlRenderer.StatePictureTaken;
                                owner.CaptureStillPicture();
                            }
                            else
                            {
                                owner.RunPrecaptureSequence();
                            }
                        }

                        break;
                    }

                case CameraPreviewControlRenderer.StateWaitingPrecapture:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null ||
                                aeState.IntValue() == ((int)ControlAEState.Precapture) ||
                                aeState.IntValue() == ((int)ControlAEState.FlashRequired))
                        {
                            owner.State = CameraPreviewControlRenderer.StateWaitingNonPrecapture;
                        }

                        break;
                    }

                case CameraPreviewControlRenderer.StateWaitingNonPrecapture:
                    {
                        // ControlAeState can be null on some devices
                        Integer aeState = (Integer)result.Get(CaptureResult.ControlAeState);
                        if (aeState == null || aeState.IntValue() != ((int)ControlAEState.Precapture))
                        {
                            owner.State = CameraPreviewControlRenderer.StatePictureTaken;
                            owner.CaptureStillPicture();
                        }

                        break;
                    }
            }
        }
    }
}