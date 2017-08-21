using System;
using CoreGraphics;
using UIKit;

namespace imagecropper
{
    public partial class ImageVC : UIViewController
    {
        protected ImageVC(IntPtr handle) : base(handle)
        {
        }

		UIImageView imageView;
		CropperView cropperView;
		UIPanGestureRecognizer pan;
		UIPinchGestureRecognizer pinch;
		UITapGestureRecognizer doubleTap;

        //UIScrollView scrollView;


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var bounds = UIScreen.MainScreen.Bounds;

			//using (var image = MinResizeImage(UIImage.FromFile("monkey.png"), (float)bounds.Width, (float)bounds.Height))
			using (var image = UIImage.FromFile("monkey.png"))

			{
				imageView = new UIImageView(new CGRect(0, 0, image.Size.Width, image.Size.Height));
				imageView.Image = image;
				//imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                //imageView.Center = View.Center;
			}
		


			cropperView = new CropperView { Frame = View.Frame };
			View.AddSubviews(imageView, cropperView);

			nfloat dx = 0;
			nfloat dy = 0;

			pan = new UIPanGestureRecognizer(() =>
			{
				if ((pan.State == UIGestureRecognizerState.Began || pan.State == UIGestureRecognizerState.Changed) && (pan.NumberOfTouches == 1))
				{

					var p0 = pan.LocationInView(View);

					if (dx == 0)
                    {
                        dx = p0.X - cropperView.Origin.X;

						if (p0.X + cropperView.CropSize.Width - dx > bounds.Width)
						{
							//Console.Write("too far");
                            dx = 0;
                            p0.X = bounds.Width - cropperView.CropSize.Width;
						}

                        if (p0.X - dx < 0)
                        {
							dx = 0;
                            p0.X = 0;
                        }
                    }


                    if (dy == 0)
                    {
                        dy = p0.Y - cropperView.Origin.Y;

						if (p0.Y + cropperView.CropSize.Height - dy > bounds.Height)
						{
							//Console.Write("too far");
							dy = 0;
							p0.Y = bounds.Height - cropperView.CropSize.Height;
						}

						if (p0.Y - dy < 0)
						{
							dy = 0;
							p0.Y = 0;
						}
                    
                    }

					var p1 = new CGPoint(p0.X - dx, p0.Y - dy);



					cropperView.Origin = p1;
				}
				else if (pan.State == UIGestureRecognizerState.Ended)
				{
					dx = 0;
					dy = 0;
				}
			});

			nfloat s0 = 1;

			pinch = new UIPinchGestureRecognizer(() =>
			{
				nfloat s = pinch.Scale;
				nfloat ds = (nfloat)Math.Abs(s - s0);
				nfloat sf = 0;
				const float rate = 0.5f;

				if (s >= s0)
				{
					sf = 1 + ds * rate;
				}
				else if (s < s0)
				{
					sf = 1 - ds * rate;
				}
				s0 = s;

                var newCropperWidth = cropperView.CropSize.Width * sf;
                var newCropperHeight = cropperView.CropSize.Height * sf;

                if (newCropperWidth > bounds.Width)
                {
                    newCropperWidth = bounds.Width;
                    newCropperHeight = bounds.Width;
                }

				cropperView.CropSize = new CGSize(newCropperWidth, newCropperHeight);

				if (pinch.State == UIGestureRecognizerState.Ended)
				{
					s0 = 1;
				}
			});

			doubleTap = new UITapGestureRecognizer((gesture) => Crop())
			{
				NumberOfTapsRequired = 2,
				NumberOfTouchesRequired = 1
			};

			cropperView.AddGestureRecognizer(pan);
			cropperView.AddGestureRecognizer(pinch);
			cropperView.AddGestureRecognizer(doubleTap);
		}

		void Crop()
		{
            //var bounds = UIScreen.MainScreen.Bounds;

            //var inputCGImage = MinResizeImage(UIImage.FromFile("monkey.png"), (float)bounds.Width, (float)bounds.Height).CGImage;

            var inputCGImage = UIImage.FromFile("monkey.png").CGImage;

			var image = inputCGImage.WithImageInRect(cropperView.CropRect);
			using (var croppedImage = UIImage.FromImage(image))
			{

				imageView.Image = croppedImage;
				imageView.Frame = cropperView.CropRect;
				imageView.Center = View.Center;

				cropperView.Origin = new CGPoint(imageView.Frame.Left, imageView.Frame.Top);
				cropperView.Hidden = true;


			}

            UndoButton.Hidden = false;
		}

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static UIImage MinResizeImage (UIImage sourceImage, float maxWidth, float maxHeight)
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
