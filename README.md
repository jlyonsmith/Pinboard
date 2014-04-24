## Pinboard

Pinboard is an application written using [Xamarin.Mac](http://xamarin.com).  As an application it's functionality is pretty simple.  It allows you to create a collection of named, colored overlapping rectangles.  This is useful for screen layout and content placement, in a video game for example.

Perhaps even more useful is that the app serves as a very full featured example of how to write model/view/controller applications using Xamarin.Mac.  It makes for a much more functionally complete starting point for a real document centric application, including things like undo and multiple selection/edit.

Specifically, it shows how to use:

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

In addition the app makes use of .NET libraries for loading and saving XML files.  

If you have the [Sketch](http://bohemiancoding.com/sketch/) application, you can see how easy it is to create great looking resources for your OSX applications.

As it stands the application would make a great starting point for a simple vector drawing program, or other graphics centric application.

Enjoy!