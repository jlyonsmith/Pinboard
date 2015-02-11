using System;
using AppKit;
using System.Diagnostics;
using ObjCRuntime;
using Foundation;

namespace Pinboard
{
    [Register("PinboardToolbarDelegate")]
    class PinboardToolbarDelegate : NSToolbarDelegate
    {
        public const string ToggleRectProps = "ToggleRectProps";
        public const string ZoomInOut = "ZoomInOut";
        [Outlet]
        public NSMenu ZoomInOutMenu { get; set; }

        public PinboardToolbarDelegate(IntPtr handle) : base(handle)
        {
        }

        [Outlet]
        public PinboardWindowController Controller { get; set; }

        #region NSToolbarDelegate Implementation

        public override string[] AllowedItemIdentifiers(NSToolbar toolbar)
        {
            return new string[0];
        }

        public override string[] DefaultItemIdentifiers(NSToolbar toolbar)
        {
            return new string[] 
            {
                ZoomInOut,
                NSToolbar.NSToolbarFlexibleSpaceItemIdentifier,
                ToggleRectProps
            };
        }

        public override void DidRemoveItem(NSNotification notification)
        {
        }

        public override string[] SelectableItemIdentifiers(NSToolbar toolbar)
        {
            return new string[0];
        }

        public override void WillAddItem(NSNotification notification)
        {
        }

        public override NSToolbarItem WillInsertItem(NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
        {
            NSToolbarItem toolbarItem = null;

            if (itemIdentifier == ToggleRectProps)
            {
                toolbarItem = ToolbarItemWithIdentifier(
                    ToggleRectProps,
                    "Rectangle Properties",
                    "Rectangle Properties",
                    "Toggle the rectangle properties drawer",
                    this.Controller,
                    NSImage.ImageNamed("Drawer"),
                    new Selector("toggleDrawer"),
                    null
                );

            }
            else if (itemIdentifier == ZoomInOut)
            {
                toolbarItem = ToolbarItemWithIdentifier(
                    ZoomInOut,
                    "100%",
                    "",
                    "Zoom canvas in or out",
                    this.Controller,
                    this.Controller.ZoomInOutView,
                    null,
                    ZoomInOutMenu
                );
            }

            return toolbarItem;
        }

        #endregion

        private NSToolbarItem ToolbarItemWithIdentifier(
            string identifier,
            string label,
            string paletteLabel,
            string toolTip,
            NSObject target,
            object imageOrView,
            Selector action,
            NSMenu menu)
        {
            NSToolbarItem item = new NSToolbarItem(identifier);

            item.Label = label;
            item.PaletteLabel = paletteLabel;
            item.ToolTip = toolTip;
            item.Target = target;
            item.Action = action;

            if (imageOrView is NSImage)
            {
                item.Image = (NSImage)imageOrView;
            }
            else if (imageOrView is NSView)
            {
                item.View = (NSView)imageOrView;
            }
            else 
                // Invalid content
                Debug.Assert(false);

            if (menu != null)
            {
                // Create a sub-menu for the menu form representation.
                // What, you never clicked on the chevrons you see when you shrink the toolbar width?
                NSMenuItem menuItem = new NSMenuItem(label);

                menuItem.Submenu = menu;
                item.MenuFormRepresentation = menuItem;
            }

            return item;
        }
    }
}

