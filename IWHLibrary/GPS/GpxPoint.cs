using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GPS
{

    public abstract class GpxPoint : Geography.Point, IXmlSerializable
    {

        #region "Реализация IXmlSerializable"

        protected static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            LatitudeDeg = double.Parse(reader.GetAttribute("lat"), xmlFormatProvider);
            LongitudeDeg = double.Parse(reader.GetAttribute("lon"), xmlFormatProvider);
            AltitudeMt = double.Parse(reader.GetAttribute("alt"), xmlFormatProvider);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("lat", LatitudeDeg.ToString("0.000000", xmlFormatProvider));
            writer.WriteAttributeString("lon", LongitudeDeg.ToString("0.000000", xmlFormatProvider));
            writer.WriteAttributeString("ele", AltitudeMt.ToString("0.000000", xmlFormatProvider));
        }

        #endregion

    }

}
