﻿// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BuildIt.Client.iOS.UI.Test.View
{
    [Register ("ConfigViewController")]
    partial class ConfigViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GetConfigButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GetConfigButton != null) {
                GetConfigButton.Dispose ();
                GetConfigButton = null;
            }
        }
    }
}