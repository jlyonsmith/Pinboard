using System;
using Foundation;
using AppKit;
using CoreGraphics;
using ObjCRuntime;

namespace Pinboard
{
    [Register("CGColorToNSColorValueTransformer")]
    public class CGColorToNSColorValueTransformer : NSValueTransformer
    {
        public CGColorToNSColorValueTransformer() : base()
        {
        }

        public CGColorToNSColorValueTransformer(IntPtr handle) : base(handle)
        {
        }

        public override NSObject TransformedValue(NSObject value)
        {
            CGColor cgColor = new CGColor(value.Handle);
            return NSColor.FromCGColor(cgColor);
        }

        public override NSObject ReverseTransformedValue(NSObject value)
        {
            var nsColor = (NSColor)value;
            return Runtime.GetNSObject(new CGColor(nsColor.RedComponent, nsColor.GreenComponent, nsColor.BlueComponent, nsColor.AlphaComponent).Handle);
        }
    }
}

