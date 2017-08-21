using System;
using CoreGraphics;
using UIKit;

namespace imagecropper
{
	public class CircleOverlay : UIView
	{
		CGPoint origin;
		CGSize cropSize;

		public CircleOverlay(CGRect frame)
		{
            Frame = frame;
            UserInteractionEnabled = false;

            // Add 40 padding left and right
            var width = frame.Width - 80;
            var heigh = width;

            var xPos = 40;
            var yPos = ((frame.Height / 2) - (heigh / 2));

			origin = new CGPoint(xPos, yPos);
			cropSize = new CGSize(width, heigh);



			BackgroundColor = UIColor.Clear;
			Opaque = false;

			Alpha = 0.4f;
		}

		public CircleOverlay(UIView viewtoDrawCircleAround, CGRect parentFrame)
		{
            Frame = parentFrame;
			UserInteractionEnabled = false;

            //origin = new CGPoint(viewtoDrawCircleAround.Frame.X, viewtoDrawCircleAround.Frame.Y);
            origin = viewtoDrawCircleAround.Center;
			cropSize = new CGSize(viewtoDrawCircleAround.Frame.Width, viewtoDrawCircleAround.Frame.Height);

			BackgroundColor = UIColor.Clear;
			Opaque = false;

			Alpha = 0.4f;
			
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			using (var g = UIGraphics.GetCurrentContext())
			{

				g.SetFillColor(UIColor.Black.CGColor);
				g.FillRect(rect);

				g.SetBlendMode(CGBlendMode.Clear);
				UIColor.Clear.SetColor();

				// Draw background circle
				CGPath path = new CGPath();
                var _radius = cropSize.Width / 2;

				path.AddArc(origin.X, origin.Y, _radius, 0, 2.0f * (float)Math.PI, true);
				g.AddPath(path);
				g.DrawPath(CGPathDrawingMode.Fill);

                // Draw Square
				//var path = new CGPath();
				//path.AddRect(new CGRect(origin, cropSize));

				//g.AddPath(path); 
				//g.DrawPath(CGPathDrawingMode.Fill);
			}
		}
    }
}
