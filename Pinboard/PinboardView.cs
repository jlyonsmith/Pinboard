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
        PinboardWindowController Controller { get; set; }

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

            DrawPinboardFile(g, pinboard.ScreenRectangle, false);

            for (int i = 0; i < pinboard.Rectangles.Count; i++)
            {
                var rectInfo = Pinboard.Rectangles.GetItem<PinboardData.RectangleInfo>(i);

                DrawPinboardFile(g, rectInfo, isSelected: false);
            }

            g.RestoreState();
        }

        void DrawPinboardFile(CGContext g, PinboardData.RectangleInfo rectInfo, bool isSelected)
        {
            g.SetFillColor(rectInfo.Color);
            g.FillRect(rectInfo.Rectangle);

            float pw = 0.5f; // Pen width
            float gw = 6.0f; // Grabber width
            float gsw = 1.0f; // Grabber shadow width
            float hgw = gw / 2f; // Half grabber width
            float x = rectInfo.X;
            float y = rectInfo.Y;
            float h = rectInfo.Height;
            float w = rectInfo.Width;
            CGColor rectBorderColor = new CGColor(0f, rectInfo.Color.Alpha);
            CGColor grabberColor = new CGColor(1f, 1f);
            CGGradient grabberGradient = null;
            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                grabberGradient = new CGGradient(colorSpace, new CGColor[] 
                {
                    new CGColor(1f, 1f),
                    new CGColor(0.9f, 1f)
                });
            }

            g.SetLineWidth(pw);

            if (isSelected)
            {
                PointF[] grabbers = new PointF[]
                {
                    new PointF(x - hgw, y - hgw),
                    new PointF(x - hgw, y + h / 2f - hgw),
                    new PointF(x - hgw, y + h - hgw),
                    new PointF(x + w / 2f - hgw, y - hgw),
                    new PointF(x + w - hgw, y - hgw),
                    new PointF(x + w - hgw, y + h / 2f - hgw),
                    new PointF(x + w - hgw, y + h - hgw),
                    new PointF(x + w / 2f - hgw, y + h - hgw)
                };

                SizeF size = new SizeF(gw, gw);
                g.SetFillColor(grabberColor);

                for (int i = 0; i < grabbers.Length; i++)
                {
                    RectangleF rect = new RectangleF(grabbers[i], size);
                    RectangleF shadowRect = new RectangleF(rect.Location, rect.Size);
                    shadowRect.Inflate(new SizeF(gsw, gsw));
                    g.SaveState();
                    g.SetFillColor(new CGColor(0.5f, 0.3f));
                    g.FillRect(shadowRect);
                    g.BeginPath();
                    g.AddRect(rect);
                    g.Clip();
                    g.DrawLinearGradient(grabberGradient, new PointF(rect.GetMidX(), rect.Bottom), new PointF(rect.GetMidX(), rect.Top), 0);
                    g.RestoreState();
                }
            }
            else
            {
                g.SetStrokeColor(rectBorderColor);
                g.SetFillColor(rectInfo.Color);
                g.SetLineJoin(CGLineJoin.Miter);
                g.BeginPath();
                g.StrokeRect(rectInfo.Rectangle);
            }

            int margin = 5;

            NSFont font = NSFont.FromFontName("Helvetica", 12f);
            NSObject[] objects = new NSObject[] { font, (NSNumber)0 };
            NSObject[] keys = new NSObject[] { NSAttributedString.FontAttributeName, NSAttributedString.LigatureAttributeName };
            NSDictionary attributes = NSDictionary.FromObjectsAndKeys(objects, keys);
            NSAttributedString attrString = new NSAttributedString(rectInfo.Name, attributes);
            attrString.DrawString(new RectangleF(rectInfo.X + margin, rectInfo.Y + margin, rectInfo.Width - 2 * margin, rectInfo.Height - 2 * margin));
        }
    }
}

