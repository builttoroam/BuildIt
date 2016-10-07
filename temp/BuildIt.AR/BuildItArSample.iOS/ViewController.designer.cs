// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BuildItArSample.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView  CameraView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MarkerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if ( CameraView != null) {
                 CameraView.Dispose ();
                 CameraView = null;
            }

            if (MarkerView != null) {
                MarkerView.Dispose ();
                MarkerView = null;
            }
        }
    }
}