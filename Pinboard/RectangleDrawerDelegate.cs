using System;
using Foundation;
using AppKit;
using System.Drawing;
using CoreGraphics;

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

        public override CoreGraphics.CGSize DrawerWillResizeContents(NSDrawer sender, CGSize toSize)
        {
            // Prevent resizing of the drawer
            return sender.ContentSize;
        }
    }
}

