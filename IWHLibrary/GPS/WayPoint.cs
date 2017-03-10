using System;
using System.Xml;
using System.Xml.Serialization;

namespace GPS
{

    [XmlRoot("wpt")]
    public class WayPoint : GPS.GpxPoint, IXmlSerializable
    {

        public string Name;

        public DateTime Time;

        #region "Реализация IXmlSerializable"

        public new void ReadXml(XmlReader reader)
        {
            throw new System.NotSupportedException();
        }

        public new void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (!string.IsNullOrEmpty(Name))
                writer.WriteElementString("name", Name);
        }

        #endregion

    }

}
