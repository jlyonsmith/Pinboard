using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;

namespace Pinboard
{
    [Register("PinboardWindowController")]
    public partial class PinboardWindowController : NSWindowController
    {
        [Outlet]
        public PinboardView PinboardView { get; set; }
        [Outlet]
        public NSDrawer RectangleDrawer { get; set; }
        [Outlet]
        public RectangleDrawerView RectangleView { get; set; }
        [Outlet]
        public NSToolbar Toolbar { get; set; }
        [Outlet]
        public NSView ZoomInOutView { get; set; }

        private PinboardDocument PinboardDocument
        {
            get 
            {
                return (PinboardDocument)this.Document;
            }
        }

        void ReleaseDesignerOutlets ()
        {
            if (PinboardView != null) 
            {
                PinboardView.Dispose ();
                PinboardView = null;
            }
        }

        public PinboardWindowController(IntPtr handle) : base(handle)
        {
        }

        public PinboardWindowController() : base("PinboardDocument")
        {
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            SizeF size = PinboardDocument.Pinboard.ScreenRectangle.Size;

            // If you don't do this the scroll bars never appear
            PinboardView.SetFrameOrigin(PointF.Empty);
            PinboardView.SetFrameSize(size);

            // Min size for the window is set in the XIB.  Set the max size here based on the content
            this.Window.ContentMaxSize = size;

            this.RectangleDrawer.ContentSize = this.RectangleView.Frame.Size;
            this.RectangleDrawer.ContentView = this.RectangleView;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseDesignerOutlets();
            PinboardView = null;
        }

        [Action("toggleDrawer")]
        public void ToggleDrawer(NSObject sender)
        {
            var state = this.RectangleDrawer.State;

            if (state == NSDrawerState.Opening || state == NSDrawerState.Open) 
                this.RectangleDrawer.Close(sender);
            else
                this.RectangleDrawer.OpenOnEdge(NSRectEdge.MaxXEdge);
        }
    }
}

