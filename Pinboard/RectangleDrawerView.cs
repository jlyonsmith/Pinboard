using System;
using AppKit;
using Foundation;

namespace Pinboard
{
    [Register("RectangleDrawerView")]
    public class RectangleDrawerView : NSView
    {
        public RectangleDrawerView(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RectangleDrawerView(NSCoder coder) : base(coder)
        {
        }
    }
}

