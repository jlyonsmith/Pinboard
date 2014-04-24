using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;
using MonoMac.CoreGraphics;
using System.Collections.Generic;
using MonoMac.CoreImage;
using MonoMac.CoreText;

namespace Pinboard
{
    [Register ("PinboardView")]
    public partial class PinboardView : NSView
    {
        protected List<int> selectionIndexes = new List<int>();
        [Outlet]
        public PinboardWindowController Controller { get; set; }

        protected PinboardData Pinboard
        {
            get
            {
                return (PinboardData)Controller.ValueForKeyPath(new NSString("document.pinboard"));
            }
        }

        public PinboardView(IntPtr handle) : base(handle)
        {
        }

        public PinboardView()
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            // Start observing changes to PinboardData and each of the PinboardData.RectangleInfo's within it
            StartObservingRectangles();
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            base.ObserveValue(keyPath, ofObject, change, context);
        }

        private void StartObservingRectangles()
        {
        }

        private void StopObservingRectangles()
        {
        }

        public override void DrawRect(RectangleF dirtyRect)
        {
            CGContext g = NSGraphicsContext.CurrentContext.GraphicsPort;

            g.SaveState();
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            g.SetFillColorSpace(colorSpace);
            g.SetStrokeColorSpace(colorSpace);
            g.SetGrayFillColor(1.0f, 1.0f);
            g.FillRect(dirtyRect);

            PinboardData pinboard = this.Pinboard;

            DrawRectangle(g, pinboard.ScreenRectangle);

            for (int i = 0; i < pinboard.Rectangles.Count; i++)
            {
                var rectInfo = Pinboard.Rectangles.GetItem<PinboardData.RectangleInfo>(i);

                DrawRectangle(g, rectInfo);
            }

            DrawSelected(g);

            g.RestoreState();
        }

        void DrawRectangle(CGContext g, PinboardData.RectangleInfo rectInfo)
        {
            g.SetFillColor(rectInfo.Color);
            g.FillRect(rectInfo.Rectangle);

            float pw = 0.5f;
            CGColor rectBorderColor = new CGColor(0f, rectInfo.Color.Alpha);

            g.SetLineWidth(pw);
            g.SetStrokeColor(rectBorderColor);
            g.SetFillColor(rectInfo.Color);
            g.SetLineJoin(CGLineJoin.Miter);
            g.BeginPath();
            g.StrokeRect(rectInfo.Rectangle);

            int margin = 5;
            // TODO: Make the font configurable
            NSFont font = NSFont.FromFontName("Helvetica", 12f);
            NSObject[] objects = new NSObject[] { font, (NSNumber)0 };
            NSObject[] keys = new NSObject[] { NSAttributedString.FontAttributeName, NSAttributedString.LigatureAttributeName };
            NSDictionary attributes = NSDictionary.FromObjectsAndKeys(objects, keys);
            NSAttributedString attrString = new NSAttributedString(rectInfo.Name, attributes);

            attrString.DrawString(new RectangleF(rectInfo.X + margin, rectInfo.Y + margin, rectInfo.Width - 2 * margin, rectInfo.Height - 2 * margin));
        }

        void DrawSelected(CGContext g)
        {
            CGImage grabberImage;
            RectangleF targetRect = new RectangleF(0, 0, 10, 10);
            grabberImage = NSImage.ImageNamed("Grabber").AsCGImage(ref targetRect, NSGraphicsContext.CurrentContext, new NSDictionary());

            float hgw = grabberImage.Width / 2f;
            float hgh = grabberImage.Height / 2f;

            // TODO: Calculate the rectangle surrounding the selected rectangles
            PinboardData.RectangleInfo rectInfo = this.Pinboard.Rectangles.GetItem<PinboardData.RectangleInfo>(0);

            float x = rectInfo.X;
            float y = rectInfo.Y;
            float h = rectInfo.Height;
            float w = rectInfo.Width;

            PointF[] grabberPoints = new PointF[]
            {
                new PointF(x - hgw, y - hgh),
                new PointF(x - hgw, y + h / 2 - hgh),
                new PointF(x - hgw, y + h - hgw),
                new PointF(x + w / 2 - hgw, y - hgw),
                new PointF(x + w - hgw, y - hgw),
                new PointF(x + w - hgw, y + h / 2f - hgw),
                new PointF(x + w - hgw, y + h - hgw),
                new PointF(x + w / 2 - hgw, y + h - hgw)
            };

            for (int i = 0; i < grabberPoints.Length; i++)
            {
                RectangleF rect = new RectangleF(grabberPoints[i], new SizeF(grabberImage.Width, grabberImage.Height));

                g.DrawImage(rect, grabberImage);
            }
        }
    }
}

