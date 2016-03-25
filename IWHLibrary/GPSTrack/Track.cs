using System;
using System.Collections.Generic;
using Primitives;
using System.Xml;
using System.Xml.Serialization;

namespace GPS
{

    [System.Serializable, XmlType("trk")]
    public class Track
    {
        [XmlElement("name")]
        public string Name;

        [XmlElement("trkseg")]
        public List<TrackSegment> Segments;

        public Track() {Segments = new List<TrackSegment>();}
    }

    [System.Serializable, XmlType("trkseg")]
    public class TrackSegment
    {
        [XmlElement("trkpt")]
        public List<TrackPoint> Points;

        public TrackSegment() { Points = new List<TrackPoint>(); }
    }

    [System.Serializable, XmlType("trkpt")]
    public class TrackPoint
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

        [XmlIgnore, XmlElement("time")]
        public DateTime Time;
    }
}
