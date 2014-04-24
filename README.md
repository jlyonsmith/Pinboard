## Pinboard

Pinboard is a simple document centric application written using [Xamarin.Mac](http://xamarin.com).  It allows you to create an XML document containing a collection of named, colored overlapping rectangles.  This is useful for screen layout and content placement. It is used to create the screen layouts for the iOS app [Jamoki Spider Solitaire](https://itunes.apple.com/us/app/spider-solitaire-by-jamoki/id511985351) for example.

The app serves as a very full featured example of how to write model/view/controller applications using Xamarin.Mac.  It is a great starting place for a real document centric application because it includes most of the things you are likely to need in such an app, including undo and multi-selection.

Specifically, it shows how to use the follow classes and concepts in Cocoa on iOS:

- NSScrollView (for scrolling and magification)
- KVC/KVO and binding
- NSWindowController
- NSDrawer
- NSArrayController
- NSObjectController
- NSUndoManager
- NSToolbar
- NSMenu/NSMenuItem
- Custom NSViews
- Quartz graphics
- Advanced XIB files

The app also makes use of Mono libraries for loading and saving XML files.  

If you have the [Sketch](http://bohemiancoding.com/sketch/) application, you can see how easy it is to create great looking resources for your OSX applications using it.  The folder `RawResources` contains the origin `.sketch` artwork that is used to generate all the `.png` files in the `Resources` directory.

As it stands now, the codebase makes a great starting point for a simple vector drawing program, or other graphics centric application.

Enjoy!
