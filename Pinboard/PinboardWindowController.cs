using System;
using AppKit;
using Foundation;
using ObjCRuntime;
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
        [Outlet]
        public NSArrayController RectangleController { get; set; }
        [Outlet]
        public NSObjectController ScreenRectangleController { get; set; }

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

        public PinboardWindowController() : base("PinboardDocument")
        {
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            RectangleController.Content = PinboardDocument.Pinboard.Rectangles;
            ScreenRectangleController.Content = PinboardDocument.Pinboard.ScreenRectangle;

            // Sadly, we have to do this manually for the time being.  See comment in PinboardView
            this.PinboardView.Bind("rectangles", this.RectangleController, "content", new NSDictionary()); 
            this.PinboardView.Bind("selectionIndexes", this.RectangleController, "selectionIndexes", new NSDictionary()); 
            this.PinboardView.Bind("screenRectangle", this.ScreenRectangleController, "content", new NSDictionary()); 

            SizeF size = PinboardDocument.Pinboard.ScreenRectangle.Size;

            // If you don't do this the scroll bars never appear
            PinboardView.SetFrameOrigin(PointF.Empty);
            PinboardView.SetFrameSize(size);

            // TODO: Adjust window size to match content size

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

        [Action("toggle:")]
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

