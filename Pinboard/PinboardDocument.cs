﻿using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Pinboard
{
    [Register("PinboardDocument")]
    public class PinboardDocument : NSDocument
    {
        [Export("pinboard")]
        public PinboardData Pinboard { get; set; }

        public PinboardDocument(IntPtr handle) : base(handle)
        {
            Pinboard = PinboardData.Default;
        }

        [Export("initWithCoder:")]
        public PinboardDocument(NSCoder coder) : base(coder)
        {
            Pinboard = PinboardData.Default;
        }

        public override void MakeWindowControllers()
        {
            AddWindowController(new PinboardWindowController());
        }

        // Save support:
        //    Override one of GetAsData, GetAsFileWrapper, or WriteToUrl.
        // This method should store the contents of the document using the given typeName
        // on the return NSData value.
        public override NSData GetAsData(string documentType, out NSError outError)
        {
            outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);

            // TODO: Check the document type
            // TODO: Write the document to a string then copy to an NSData

            return null;
        }

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = null;

            try
            {
                unsafe
                {
                    this.Pinboard = new PinboardDataReaderV1().ReadFromUrl(url.AbsoluteString);
                }
            }
            catch
            {
                // TODO: Set pinboard specific error here
                outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);
                return false;
            }

            return true;
        }

        // If this returns the name of a XIB/NIB file instead of null, a NSDocumentController
        // is automatically created for you.
        public override string WindowNibName
        { 
            get
            {
                // Name of the .xib file
                return "PinboardDocument";
            }
        }
    }
}

