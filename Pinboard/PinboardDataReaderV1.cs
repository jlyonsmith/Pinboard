using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Foundation;
using CoreGraphics;

namespace Pinboard
{
    public class PinboardDataReaderV1
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

            var format = reader.GetAttribute("Format");

            if (format == null || format != "1")
                // TODO: Don't throw exception, return null with an error code
                throw new FormatException(String.Format("Pinboard "));

            reader.ReadStartElement("Pinboard");

            reader.MoveToContent();
            data.ScreenRectangle = ReadRectangleXml();

            data.Rectangles = ReadRectanglesXml(data.ScreenRectangle);

            reader.ReadEndElement();
            reader.MoveToContent();

            return data;
        }

        private NSMutableArray ReadRectanglesXml(PinboardData.RectangleInfo screenRectangle)
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

                // Flip the rectangle top to bottom relative to the screen rectangle
                rectInfo.Y = screenRectangle.Height - rectInfo.Y - rectInfo.Height;

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
            a = reader.ReadElementContentAsInt("A", "");
            reader.MoveToContent();
            r = reader.ReadElementContentAsInt("R", "");
            reader.MoveToContent();
            g = reader.ReadElementContentAsInt("G", "");
            reader.MoveToContent();
            b = reader.ReadElementContentAsInt("B", "");
            reader.MoveToContent();
            reader.ReadEndElement();
            reader.MoveToContent();

            return new CGColor(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
    }
}
