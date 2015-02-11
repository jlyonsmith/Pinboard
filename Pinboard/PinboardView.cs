using System;
using AppKit;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using CoreImage;
using CoreText;
using System.Diagnostics;
using System.Drawing;

namespace Pinboard
{
    [Register ("PinboardView")]
    public partial class PinboardView : NSView
    {
        [Outlet]
        public PinboardWindowController Controller { get; set; }

        public static NSString IndividualRectangleObservationContext = new NSString("com.jamoki.pinboard.individualRectangle");

        // I've implemented a simple wrapper around Cocoa bindable properties to make it all a bit cleaner to code.  Ideally, binding
        // would be done in IB but Xamarin.Mac does not yet enable custom view bindings in IB.  As far as I can tell it would probably 
        // need lots of help from the Obj-C code generator, because IB needs a bunch more NSKeyValueBindingCreation methods to 
        // be implemented than just Bind and Unbind.
        //
        private class RectanglesBinding : CocoaBinding
        {
            public RectanglesBinding() : base("rectangles")
            {
            }

            public override void Bind(NSView view, NSObject withObj, string keyPath, NSDictionary options)
            {
                base.Bind(view, withObj, keyPath, options);

                var array = this.GetData<NSMutableArray>();

                // Start observing each of the rectangles in the array
                // BUG: Xamaran.Mac does not support the override to bind to a range of objects.
                // See https://bugzilla.xamarin.com/show_bug.cgi?id=19325

                NSString rectangleKey = new NSString("rectangle");
                NSString colorKey = new NSString("color");
                NSString nameKey = new NSString("name");

                for (nuint i = 0; i < array.Count; i++)
                {
                    NSObject obj = array.GetItem<NSObject>(i);

                    obj.AddObserver(view, rectangleKey, NSKeyValueObservingOptions.OldNew, IndividualRectangleObservationContext.Handle);
                    obj.AddObserver(view, colorKey, NSKeyValueObservingOptions.New, IndividualRectangleObservationContext.Handle);
                    obj.AddObserver(view, nameKey, NSKeyValueObservingOptions.New, IndividualRectangleObservationContext.Handle);
                }

                controller.AddObserver(view, KeyPath, NSKeyValueObservingOptions.OldNew, ContextHandle);

                view.NeedsDisplay = true;
            }
        }

        private class SelectionIndexesBinding : CocoaBinding
        {
            public SelectionIndexesBinding() : base("selectionIndexes")
            {
            }

            public override void Bind(NSView view, NSObject withObj, string keyPath, NSDictionary options)
            {
                base.Bind(view, withObj, keyPath, options);

                // Need the new and old indexes for correct screen update rectangle
                controller.AddObserver(view, KeyPath, NSKeyValueObservingOptions.OldNew, ContextHandle);

                view.NeedsDisplay = true;
            }
        }

        private class ScreenRectangleBinding : CocoaBinding
        {
            public ScreenRectangleBinding() : base("screenRectangle")
            {
            }

            public override void Bind(NSView view, NSObject withObj, string keyPath, NSDictionary options)
            {
                base.Bind(view, withObj, keyPath, options);

                controller.AddObserver(view, KeyPath, NSKeyValueObservingOptions.New, ContextHandle); 

                view.NeedsDisplay = true;
            }
        }

        private RectanglesBinding rectanglesBinding = new RectanglesBinding();
        private SelectionIndexesBinding selectionIndexesBinding = new SelectionIndexesBinding();
        private ScreenRectangleBinding screenRectangleBinding = new ScreenRectangleBinding();
        private SizeF grabberSize = new SizeF(10, 10);
        private CGRect selectionRect = new CGRect();
        private CGRect[] grabberRects = new CGRect[8];
        private NSCursor swneCursor;
        private NSCursor nwseCursor;
        private NSCursor nsCursor;
        private NSCursor weCursor;

        public PinboardView(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PinboardView(NSCoder coder) : base(coder)
        {
        }

        public PinboardView()
        {
        }

        public override void AwakeFromNib()
        {
            swneCursor = MakeCursor("SW-NE-Arrow");
            nwseCursor = MakeCursor("NW-SE-Arrow");
            weCursor = MakeCursor("W-E-Arrow");
            nsCursor = MakeCursor("N-S-Arrow");

            base.AwakeFromNib();
        }

        private NSCursor MakeCursor(string imageName)
        {
            NSImage image = NSImage.ImageNamed(imageName);

            return new NSCursor(image, new CGPoint(image.Size.Width / 2, image.Size.Height / 2));
        }

        public override void Bind(string name, NSObject withObj, string keyPath, NSDictionary options)
        {
            if (name == rectanglesBinding.BindingName)
            {
                rectanglesBinding.Bind(this, withObj, keyPath, options);
            }
            else if (name == selectionIndexesBinding.BindingName)
            {
                selectionIndexesBinding.Bind(this, withObj, keyPath, options);
            }
            else if (name == screenRectangleBinding.BindingName)
            {
                screenRectangleBinding.Bind(this, withObj, keyPath, options);
            }
            else
                base.Bind(name, withObj, keyPath, options);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == selectionIndexesBinding.ContextHandle || 
                context == IndividualRectangleObservationContext.Handle)
            {
                CalculateBoundingRectangle(selectionIndexesBinding.GetData<NSIndexSet>(), out selectionRect);
                CalculateGrabberRectangles();
                // TODO: Only invalidate the combination of the old and the new selection
                this.NeedsDisplay = true;
                this.Window.InvalidateCursorRectsForView(this);
            }
            else if (context == rectanglesBinding.ContextHandle)
            {
                this.NeedsDisplay = true;
            }
            else if (context == screenRectangleBinding.ContextHandle)
            {
                this.NeedsDisplay = true;
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        public override void DrawRect(CGRect dirtyRect)
        {
            CGContext g = NSGraphicsContext.CurrentContext.GraphicsPort;

            g.SaveState();
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            g.SetFillColorSpace(colorSpace);
            g.SetStrokeColorSpace(colorSpace);
            g.SetFillColor(1.0f, 1.0f);
            g.FillRect(dirtyRect);

            DrawRectangle(g, screenRectangleBinding.GetData<PinboardData.RectangleInfo>());

            var rectangles = rectanglesBinding.GetData<NSMutableArray>();

            for (nuint i = 0; i < rectangles.Count; i++)
            {
                var rectInfo = rectangles.GetItem<PinboardData.RectangleInfo>(i);

                DrawRectangle(g, rectInfo);
            }

            DrawSelected(g);

            g.RestoreState();
        }

        public override void ResetCursorRects()
        {
            if (selectionRect.IsEmpty)
                return;

            this.AddCursorRect(grabberRects[0], swneCursor);
            this.AddCursorRect(grabberRects[4], swneCursor);

            this.AddCursorRect(grabberRects[1], weCursor);
            this.AddCursorRect(grabberRects[5], weCursor);

            this.AddCursorRect(grabberRects[2], nwseCursor);
            this.AddCursorRect(grabberRects[6], nwseCursor);

            this.AddCursorRect(grabberRects[3], nsCursor);
            this.AddCursorRect(grabberRects[7], nsCursor);
        }

        private void DrawRectangle(CGContext g, PinboardData.RectangleInfo rectInfo)
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

        private void CalculateBoundingRectangle(NSIndexSet selectionIndexes, out CGRect boundingRectangle)
        {
            boundingRectangle = CGRect.Empty;

            if (selectionIndexes.Count == 0)
            {
                // No selection
                return; 
            }

            var rectangles = rectanglesBinding.GetData<NSMutableArray>();

            foreach (var selectionIndex in selectionIndexes)
            {
                var rect = rectangles.GetItem<PinboardData.RectangleInfo>(selectionIndex).Rectangle;

                if (boundingRectangle.IsEmpty)
                {
                    boundingRectangle = rect;
                }
                else
                {
                    boundingRectangle = CGRect.Union(boundingRectangle, rect);
                }
            }
        }

        private void CalculateGrabberRectangles()
        {
            float hgw = grabberSize.Width / 2f;
            float hgh = grabberSize.Height / 2f;
            nfloat x = selectionRect.X;
            nfloat y = selectionRect.Y;
            nfloat h = selectionRect.Height;
            nfloat w = selectionRect.Width;

            // Bottom-left then clockwise...
            grabberRects[0] = new CGRect(new CGPoint(x - hgw, y - hgh), grabberSize);
            grabberRects[1] = new CGRect(new CGPoint(x - hgw, y + h / 2 - hgh), grabberSize);
            grabberRects[2] = new CGRect(new CGPoint(x - hgw, y + h - hgw), grabberSize);
            grabberRects[3] = new CGRect(new CGPoint(x + w / 2 - hgw, y + h - hgw), grabberSize);
            grabberRects[4] = new CGRect(new CGPoint(x + w - hgw, y + h - hgw), grabberSize);
            grabberRects[5] = new CGRect(new CGPoint(x + w - hgw, y + h / 2f - hgw), grabberSize);
            grabberRects[6] = new CGRect(new CGPoint(x + w - hgw, y - hgw), grabberSize);
            grabberRects[7] = new CGRect(new CGPoint(x + w / 2 - hgw, y - hgw), grabberSize);
        }

        private void DrawSelected(CGContext g)
        {
            if (selectionRect.IsEmpty)
                return;

            CGImage grabberImage;
            var targetRect = new CGRect(CGPoint.Empty, grabberSize);

            grabberImage = NSImage.ImageNamed("Grabber").AsCGImage(ref targetRect, NSGraphicsContext.CurrentContext, new NSDictionary());

            g.SetStrokeColor(new CGColor(0.5f, 0.5f));
            g.StrokeRectWithWidth(selectionRect, 0.5f);

            for (int i = 0; i < grabberRects.Length; i++)
            {
                g.DrawImage(grabberRects[i], grabberImage);
            }
        }

        private bool RectangleFromPoint(CGPoint point, out uint index)
        {
            var rects = rectanglesBinding.GetData<NSMutableArray>();

            for (nuint i = 0; i < rects.Count; i++)
            {
                if (rects.GetItem<PinboardData.RectangleInfo>(i).Rectangle.Contains(point))
                {
                    index = (uint)i;
                    return true;
                }
            }

            index = uint.MaxValue;
            return false;
        }

        public override void MouseDown(NSEvent theEvent)
        {
            if (theEvent.Type == NSEventType.LeftMouseDown)
            {
                uint index;
                bool haveIndex = RectangleFromPoint(theEvent.LocationInWindow, out index);

                NSMutableIndexSet indexSet = new NSMutableIndexSet(selectionIndexesBinding.GetData<NSIndexSet>());

                if ((theEvent.ModifierFlags & NSEventModifierMask.CommandKeyMask) != 0)
                {
                    if (!haveIndex)
                        // We did not select or unselect anything.  Don't mess with the current selection
                        return;

                    if (indexSet.Contains(index))
                        indexSet.Remove(index);
                    else 
                        indexSet.Add(index);
                }
                else
                {
                    indexSet.Clear();

                    if (haveIndex)
                        indexSet.Add(index);
                }

                selectionIndexesBinding.SetData(indexSet);

                // TODO: Start a drag operation
            }
            else
                base.MouseDown(theEvent);
        }
    }
}

