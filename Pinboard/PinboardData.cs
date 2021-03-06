﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using CoreGraphics;
using System.Drawing;

namespace Pinboard
{
    public class PinboardData : NSObject
    {
        public PinboardData()
        {
        }

        [Export("screenRectangle")]
        public RectangleInfo ScreenRectangle { get; set; }

        // TODO: Apple claims you can get performance improvements by implementing a to-many accessor pattern 
        // instead of returning the collection directly.
        // See https://developer.apple.com/library/mac/documentation/Cocoa/Conceptual/KeyValueCoding/Articles/AccessorConventions.html#//apple_ref/doc/uid/20002174-BAJEAIEE
        [Export("rectangles")]
        public NSMutableArray Rectangles { get; set; }

        private static PinboardData @default; 

        public static PinboardData Default
        {
            get
            {
                if (@default == null)
                {
                    @default = new PinboardData();
                    @default.ScreenRectangle = new PinboardData.RectangleInfo(
                        new RectangleF(0, 0, 800, 600), "Screen", new CGColor(1, 1, 1, 1));
                    @default.Rectangles = new NSMutableArray();
                }

                return @default;
            }
        }

        public class RectangleInfo : NSObject
        {
            public RectangleInfo()
            {
            }

            public RectangleInfo(RectangleF rect, string name)
                : this(rect, name, new CGColor(0.5f, 1.0f))
            {
            }

            public RectangleInfo(RectangleF rect, string name, CGColor color)
            {
                this.X = rect.X;
                this.Y = rect.Y;
                this.Width = rect.Width;
                this.Height = rect.Height;
                this.Color = color;
                this.Name = name;
            }

            [Export("name")]
            public string Name { get; set; }

            [Export("x")]
            public float X { get; set; }

            [Export("y")]
            public float Y { get; set; }

            [Export("width")]
            public float Width { get; set; }

            [Export("height")]
            public float Height { get; set; }

            [Export("rectangle")]
            public CGRect Rectangle
            {
                get
                {
                    return new CGRect(X, Y, Width, Height);
                }
            }

            [Export("keyPathsForValuesAffectingRectangle")]
            public static NSSet KeyPathsForValuesAffectingRectangle()
            {
                return new NSSet("x", "y", "width", "height");
            }

            [Export("size")]
            public SizeF Size
            {
                get
                {
                    return new SizeF(Width, Height);
                }
            }

            [Export("keyPathsForValuesAffectingSize")]
            public static NSSet KeyPathsForValuesAffectingSize()
            {
                return new NSSet("width", "height");
            }

            [Export("color")]
            public CGColor Color { get; set; }
        }
    }
}
