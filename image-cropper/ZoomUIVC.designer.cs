// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace imagecropper
{
    [Register ("ZoomUIVC")]
    partial class ZoomUIVC
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CropButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollViewFromUI { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CropButton != null) {
                CropButton.Dispose ();
                CropButton = null;
            }

            if (ScrollViewFromUI != null) {
                ScrollViewFromUI.Dispose ();
                ScrollViewFromUI = null;
            }
        }
    }
}