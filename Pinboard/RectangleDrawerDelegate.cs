using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;

namespace Pinboard
{
    [Register("RectangleDrawerDelegate")]
    public class RectangleDrawerDelegate : NSDrawerDelegate
    {
        [Outlet]
        public NSDrawer Drawer { get; set; }

        public RectangleDrawerDelegate(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RectangleDrawerDelegate(NSCoder coder) : base(coder)
        {
        }

        public override SizeF DrawerWillResizeContents(NSDrawer sender, SizeF toSize)
        {
            // Prevent resizing of the drawer
            return sender.ContentSize;
        }
    }
}

