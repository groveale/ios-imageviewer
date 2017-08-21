using Foundation;
using System;
using UIKit;

namespace imagecropper
{
    public partial class CroppedImageViewer : UIViewController
    {
        public CroppedImageViewer (IntPtr handle) : base (handle)
        {
        }

        public UIImage croppedImage { get; set; }


        public override void ViewDidLoad()
        {
            if (croppedImage != null)
            {
                CroppedImage.Image = croppedImage;
            }
        }
    }
}