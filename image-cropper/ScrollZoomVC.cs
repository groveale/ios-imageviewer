using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace imagecropper
{
    public partial class ScrollZoomVC : UIViewController
    {
        public ScrollZoomVC (IntPtr handle) : base (handle)
        {
        }

		UIScrollView scrollView;
		UIImageView imageView;
        //CropperView cropperView;
        CircleOverlay circleView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.White;

			this.Title = "Scroll View";

			

			// create our scroll view
			scrollView = new UIScrollView(
				new CGRect(0, 0, View.Frame.Width
				, View.Frame.Height - NavigationController.NavigationBar.Frame.Height));
			View.AddSubview(scrollView);


            // create our image view
            var image = UIImage.FromFile("shroom.jpg");
			var largestSide = Math.Max(image.Size.Width, image.Size.Height);
			var sizeOfContainerView = largestSide * 1.1;

            imageView = new UIImageView(new CGRect(0,0,(nfloat)sizeOfContainerView, (nfloat)sizeOfContainerView));
            imageView.BackgroundColor = UIColor.Green;
            imageView.Image = image;
            imageView.ContentMode = UIViewContentMode.Center;



    

            //scrollView.ContentSize = imageView.Image.Size;
            scrollView.ContentSize = imageView.Frame.Size;
			scrollView.AddSubview(imageView);
            //scrollView.BackgroundColor = UIColor.Black;

            scrollView.ContentInset = UIEdgeInsets.Zero;



            //cropper view (fixed)
            //cropperView = new CropperView { Frame = scrollView.Frame };
            //cropperView.UserInteractionEnabled = false;
            // View.AddSubview(cropperView);

            //circle overlay view (fixed)
            circleView = new CircleOverlay(scrollView.Frame);
			
			View.AddSubview(circleView);

			// set allow zooming
			scrollView.MaximumZoomScale = 3f;
			scrollView.MinimumZoomScale = .1f;
			scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };
            scrollView.DidZoom += ScrollView_DidZoom;


        }


        void ScrollView_DidZoom(object sender, EventArgs e)
        {
			CGSize boundsSize = scrollView.Bounds.Size;
            CGRect contentsFrame = imageView.Frame;

			if (contentsFrame.Size.Width < boundsSize.Width)
			{
				contentsFrame.X = (boundsSize.Width - contentsFrame.Size.Width) / 2.0f;
			}
			else
			{
				contentsFrame.X = 0.0f;
			}

			if (contentsFrame.Size.Height < boundsSize.Height)
			{
                contentsFrame.Y = ((boundsSize.Height - contentsFrame.Size.Height) / 2.0f) - NavigationController.NavigationBar.Frame.Height;
			}
			else
			{
				contentsFrame.Y = 0.0f;
			}

			imageView.Frame = contentsFrame;
        }
    }
}