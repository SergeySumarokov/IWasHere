using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primitives;
using System.Xml;
using System.Xml.Serialization;

namespace GPS
{

    //[System.Serializable]
    [System.Serializable, XmlType("trk")]
    public class Track
    {
        public string Name;

        //[XmlArray("trk"), XmlArrayItem("trkseg")]
        [XmlElement("trkseg")]
        public List<TrackSegment> Segments;

        public Track() {Segments = new List<TrackSegment>();}
    }

    //[System.Serializable]
    [System.Serializable, XmlType("trkseg")]
    public class TrackSegment
    {
        //[XmlArray("trkseg"), XmlArrayItem("trkpt")]
        [XmlElement("trkpt")]
        public List<TrackPoint> Points;

        public TrackSegment() { Points = new List<TrackPoint>(); }
    }

    //[System.Serializable]
    [System.Serializable, XmlType("trkpt")]
    public class TrackPoint
    {
        [XmlIgnore]
        public Coordinates Coordinates;

        [XmlAttribute("Lat")]
        public double Lat
        {
            get { return Coordinates.Latitude.Degrees; }
            set { Coordinates.Latitude.Degrees = value; }
        }

        [XmlAttribute("Lon")]
        public double Lon
        {
            get { return Coordinates.Longitude.Degrees; }
            set { Coordinates.Longitude.Degrees = value; }
        }

        [XmlIgnore]
        //[XmlElement("Ele")]
        public double Ele
        {
            get { return Coordinates.Altitude.Meters; }
            set { Coordinates.Altitude.Meters = value; }
        }

        [XmlIgnore]
        //[XmlElement("Time")]
        public DateTime Time;
    }
}
