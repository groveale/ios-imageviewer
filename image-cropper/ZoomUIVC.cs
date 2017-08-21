using Foundation;
using System;
using UIKit;
using CoreGraphics;
using CoreImage;

namespace imagecropper
{
    public partial class ZoomUIVC : UIViewController
    {
        public ZoomUIVC (IntPtr handle) : base (handle)
        {
        }

        UIImageView imageView;
		//CropperView cropperView;
		CircleOverlay circleView;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

          
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.Black;

			this.Title = "Scroll View";

			// create our image view
			var image = UIImage.FromFile("shroom.jpg");
			var largestSide = Math.Max(image.Size.Width, image.Size.Height);
			var sizeOfContainerView = largestSide * 1.1;

		
            // background image
			imageView = new UIImageView(new CGRect(0, 0, (nfloat)sizeOfContainerView, (nfloat)sizeOfContainerView));

            // blur the background image
            var bluredImage = Blur(MinResizeImage(image));

			imageView.Image = bluredImage;
			imageView.ContentMode = UIViewContentMode.ScaleToFill;

			//blur over background(stretched) image
			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
			var blurView = new UIVisualEffectView(blur)
			{
				Frame = imageView.Frame
			};
			// add blur
			imageView.AddSubview(blurView);


			// actual image
			var theImage = new UIImageView(new CGRect(0, 0, (nfloat)sizeOfContainerView, (nfloat)sizeOfContainerView));

			theImage.Image = image;
			theImage.ContentMode = UIViewContentMode.Center;
            imageView.AddSubview(theImage);

            // stop if from spilling out
            imageView.ClipsToBounds = true;

			ScrollViewFromUI.ContentSize = imageView.Frame.Size;
			
			ScrollViewFromUI.AddSubview(imageView);

			ScrollViewFromUI.ContentInset = UIEdgeInsets.Zero;
            ScrollViewFromUI.ClipsToBounds = false;

            // prevent white space at top of scroll viwe
            this.AutomaticallyAdjustsScrollViewInsets = false;

			// default to center of image
			//ScrollViewFromUI.ContentOffset = imageView.Center;


			//circle overlay view (fixed)
			circleView = new CircleOverlay(ScrollViewFromUI, View.Frame);
   

			View.AddSubview(circleView);

			// set allow zooming
			ScrollViewFromUI.MaximumZoomScale = 1f;
			ScrollViewFromUI.MinimumZoomScale = .1f;

			ScrollViewFromUI.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };
			ScrollViewFromUI.DidZoom += ScrollView_DidZoom;

            // zoom all teh way out
            ScrollViewFromUI.SetZoomScale(.1f, false);

            // add action to crop button
            CropButton.TouchUpInside += CropButton_TouchUpInside;
       }


		void ScrollView_DidZoom(object sender, EventArgs e)
		{
			CGSize boundsSize = ScrollViewFromUI.Bounds.Size;
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
                contentsFrame.Y = ((boundsSize.Height - contentsFrame.Size.Height) / 2.0f);
			}
			else
			{
				contentsFrame.Y = 0.0f;
			}

			imageView.Frame = contentsFrame;
		}



        void CropButton_TouchUpInside(object sender, EventArgs e)
        {
			CGSize pageSize = ScrollViewFromUI.Frame.Size;

			UIGraphics.BeginImageContext(pageSize);

			var resizedContext = UIGraphics.GetCurrentContext();

			// Translate
			resizedContext.TranslateCTM(-ScrollViewFromUI.ContentOffset.X, -ScrollViewFromUI.ContentOffset.Y);

			ScrollViewFromUI.Layer.RenderInContext(resizedContext);

			UIImage image = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();


			CroppedImageViewer croppedImageVC = this.Storyboard.InstantiateViewController("CroppedImageViewer") as CroppedImageViewer;

			if (croppedImageVC != null)
			{
				croppedImageVC.croppedImage = image;
			}

			NavigationController.PushViewController(croppedImageVC, true);

        }

		public UIImage Blur(UIImage image, float radius = 5f)
		{

			// Create a new blurred image.
			var imageToBlur = new CIImage(image);
			var blur = new CIGaussianBlur();
			blur.Image = imageToBlur;
			blur.Radius = radius;

			var blurImage = blur.OutputImage;
			var context = CIContext.FromOptions(new CIContextOptions { UseSoftwareRenderer = false });
			var cgImage = context.CreateCGImage(blurImage, new CGRect(new CGPoint(0, 0), image.Size));
			var newImage = UIImage.FromImage(cgImage);

			return newImage;
			
		}

		public static UIImage MinResizeImage(UIImage sourceImage, float maxWidth = 350, float maxHeight = 350)
		{
			var sourceSize = sourceImage.Size;
			var minResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

			if (minResizeFactor > 1)
			{
				return sourceImage;
			}

			var width = minResizeFactor * sourceSize.Width;
			var height = minResizeFactor * sourceSize.Height;

			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return resultImage;
		}
	
    }
}