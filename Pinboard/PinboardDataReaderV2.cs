using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;
using Foundation;
using CoreGraphics;

namespace Pinboard
{
    public class PinboardDataReaderV2
    {
        private string rectanglesAtom;
        private XmlReader reader;

        public PinboardData ReadFromUrl(string url)
        {
            using (reader = XmlReader.Create(url))
            {
                rectanglesAtom = reader.NameTable.Add("Rectangles");

                reader.MoveToContent();
                PinboardData data = ReadPinboardXml();
                return data;
            }
        }

        private PinboardData ReadPinboardXml()
        {
            PinboardData data = new PinboardData();

            reader.ReadStartElement("Pinboard");

            // TODO: Check that version is 2

            reader.MoveToContent();
            data.ScreenRectangle = ReadRectangleXml();

            data.Rectangles = ReadRectanglesXml();

            reader.ReadEndElement();
            reader.MoveToContent();

            return data;
        }

        private NSMutableArray ReadRectanglesXml()
        {
            var list = new NSMutableArray();

            // Read <Rectangles>
            reader.ReadStartElement(rectanglesAtom);
            reader.MoveToContent();

            while (true)
            {
                if (String.ReferenceEquals(reader.Name, rectanglesAtom))
                {
                    reader.ReadEndElement();
                    reader.MoveToContent();
                    break;
                }

                var rectInfo = ReadRectangleXml();

                list.Add(rectInfo);
            }

            return list;
        }

        private PinboardData.RectangleInfo ReadRectangleXml()
        {
            var rectInfo = new PinboardData.RectangleInfo();

            // Read <Rectangle>
            reader.ReadStartElement("Rectangle");
            reader.MoveToContent();
            rectInfo.Name = reader.ReadElementContentAsString("Name", "");
            reader.MoveToContent();
            rectInfo.X = reader.ReadElementContentAsInt("X", "");
            reader.MoveToContent();
            rectInfo.Y = reader.ReadElementContentAsInt("Y", "");
            reader.MoveToContent();
            rectInfo.Width = reader.ReadElementContentAsInt("Width", "");
            reader.MoveToContent();
            rectInfo.Height = reader.ReadElementContentAsInt("Height", "");
            reader.MoveToContent();
            rectInfo.Color = ReadColorXml();
            reader.ReadEndElement();
            reader.MoveToContent();

            return rectInfo;
        }

        private CGColor ReadColorXml()
        {
            float a, r, g, b;

            reader.ReadStartElement("Color");
            reader.MoveToContent();
            r = reader.ReadElementContentAsFloat("R", "");
            reader.MoveToContent();
            g = reader.ReadElementContentAsFloat("G", "");
            reader.MoveToContent();
            b = reader.ReadElementContentAsFloat("B", "");
            reader.MoveToContent();
            a = reader.ReadElementContentAsFloat("A", "");
            reader.MoveToContent();
            reader.ReadEndElement();
            reader.MoveToContent();

            return new CGColor(r, g, b, a);
        }
    }
}
