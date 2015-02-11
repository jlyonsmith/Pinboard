using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using CoreGraphics;

namespace Pinboard
{
    public class PinboardDataWriter
    {
        public void WriteXml(XmlWriter writer, PinboardData data)
        {
            WritePinboardXml(writer, data);
        }

        private void WritePinboardXml(XmlWriter writer, PinboardData data)
        {
            writer.WriteStartElement("Pinboard");
            writer.WriteAttributeString("Format", "2");

            WriteRectangleXml(writer, data.ScreenRectangle);

            WriteRectanglesXml(writer, data);
            writer.WriteEndElement();
        }

        private void WriteRectanglesXml(XmlWriter writer, PinboardData data)
        {
            writer.WriteStartElement("Rectangles");

            for (nuint i = 0; i < data.Rectangles.Count; i++)
            {
                var rectInfo = data.Rectangles.GetItem<PinboardData.RectangleInfo>(i);
                WriteRectangleXml(writer, rectInfo);
            }

            writer.WriteEndElement();
        }

        private void WriteRectangleXml(XmlWriter writer, PinboardData.RectangleInfo rectInfo)
        {
            writer.WriteStartElement("Rectangle");
            writer.WriteElementString("Name", rectInfo.Name.ToString());
            writer.WriteElementString("X", rectInfo.X.ToString());
            writer.WriteElementString("Y", rectInfo.Y.ToString());
            writer.WriteElementString("Width", rectInfo.Width.ToString());
            writer.WriteElementString("Height", rectInfo.Height.ToString());
            WriteColorXml(writer, rectInfo.Color);
            writer.WriteEndElement();
        }

        private void WriteColorXml(XmlWriter writer, CGColor color)
        {
            writer.WriteStartElement("Color");
            writer.WriteElementString("R", color.Components[0].ToString());
            writer.WriteElementString("G", color.Components[1].ToString());
            writer.WriteElementString("B", color.Components[2].ToString());
            writer.WriteElementString("A", color.Alpha.ToString());
            writer.WriteEndElement();
        }
    }
}
