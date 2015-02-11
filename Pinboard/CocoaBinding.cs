using System;
using Foundation;
using AppKit;

namespace Pinboard
{
    public class CocoaBinding
    {
        protected const string BindingContextPrefix = "com.jamoki.pinboard.";

        protected string bindingName;
        protected NSString contextName;
        protected NSObjectController controller;
        protected NSString keyPath;

        public CocoaBinding(string bindingName)
        {
            this.bindingName = bindingName;
            this.contextName = new NSString(BindingContextPrefix + bindingName);
        }

        public string BindingName
        {
            get 
            {
                return bindingName;
            }
        }

        public string ContextName
        {
            get
            {
                return contextName;
            }
        }

        public IntPtr ContextHandle
        {
            get
            {
                return contextName.Handle;
            }
        }

        public NSString KeyPath { get { return keyPath; } }

        public virtual void Bind(NSView view, NSObject withObj, string keyPath, NSDictionary options)
        {
            if (IsBound)
            {
                Unbind();
            }

            this.controller = (NSObjectController)withObj;
            this.keyPath = new NSString(keyPath);
        }

        public virtual void Unbind()
        {
            this.keyPath = null;
            this.controller = null;
        }

        public bool IsBound
        {
            get
            {
                return (controller != null);
            }
        }

        public T GetData<T>() where T: class
        {
            var objectController = this.controller as NSObjectController;

            if (objectController != null)
            {
                NSObject obj = controller.ValueForKeyPath(keyPath); 
                return obj as T;
            }
            else
                return default(T);
        }

        public void SetData<T>(T value) where T: NSObject
        {
            var objectController = this.controller as NSObjectController;

            if (objectController != null)
            {
                controller.SetValueForKeyPath((NSObject)value, keyPath); 
            }
        }
    }
}

