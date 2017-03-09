﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Geography;

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

    [XmlRoot("trkpt")]
    public class TrackPoint : GPS.GpxPoint, IXmlSerializable
    {
        public DateTime Time;

        #region "Реализация IXmlSerializable"

        public new void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            string timeString = reader.GetAttribute("time");
            if (!String.IsNullOrEmpty(timeString))
                Time = DateTime.Parse(timeString, xmlFormatProvider);
        }

        public new void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (Time != DateTime.MinValue)
                writer.WriteElementString("time", Time.ToString(xmlFormatProvider));
        }

        #endregion
    }
}
