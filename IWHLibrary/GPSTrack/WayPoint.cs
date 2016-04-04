using System;
using System.Xml;
using System.Xml.Serialization;
using Primitives;

namespace GPS
{

    [System.Serializable, XmlType("wpt")]
    public class WayPoint
    {
        [XmlIgnore]
        public Coordinates Coordinates;

        [XmlAttribute("lat")]
        public double Lat
        {
            get { return Coordinates.Latitude.Degrees; }
            set { Coordinates.Latitude.Degrees = value; }
        }

        [XmlAttribute("lon")]
        public double Lon
        {
            get { return Coordinates.Longitude.Degrees; }
            set { Coordinates.Longitude.Degrees = value; }
        }

        [XmlIgnore, XmlElement("ele")]
        public double Ele
        {
            get { return Coordinates.Altitude.Meters; }
            set { Coordinates.Altitude.Meters = value; }
        }

        [XmlElement("name")]
        public string Name;

        [XmlIgnore, XmlElement("time")]
        public DateTime Time;

    }

}
