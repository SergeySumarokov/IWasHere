using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace GPS
{
    
    /// <summary>
    /// Представляет объект для чтения-записи gpx-файла.
    /// </summary>
    [System.Serializable, XmlType("gpx")]
    public class Gpx
    {

        /// <summary>
        /// Список треков
        /// </summary>
        [XmlElement("trk")]
        public List<Track> Tracks { get; private set; }

        /// <summary>
        /// Список путевых точек
        /// </summary>
        [XmlElement("wpt")]
        public List<WayPoint> WayPoints { get; private set; }

        /// <summary>
        /// Инициализирует пустой экземпляр класса.
        /// </summary>
        public Gpx()
        {
            Tracks = new List<Track>();
            WayPoints = new List<WayPoint>();
        }

        /// <summary>
        /// Возвращает новый экземпляр с данными из xml-файла.
        /// </summary>
        /// <param name="fileName"></param>
        public static Gpx FromXmlFile(string fileName)
        {
            var gpx = new Gpx();
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            XmlDocument xml = new XmlDocument();
            XmlNamespaceManager prefix;
            xml.Load(fileName);
            prefix = new XmlNamespaceManager(xml.NameTable);
            prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            // Обходим треки
            foreach (XmlNode nodeTrk in xml.SelectNodes("//prfx:gpx/prfx:trk", prefix))
            {
                var trk = new Track();
                // Обходим сегменты трека
                foreach (XmlNode nodeTrkseg in nodeTrk.ChildNodes)
                {
                    if (nodeTrkseg.Name=="trkseg")
                    {
                        var seg = new TrackSegment();
                        // Обходим точки сегмента
                        foreach (XmlNode nodeTrkpt in nodeTrkseg.ChildNodes)
                        {
                            if (nodeTrkpt.Name == "trkpt")
                            {
                                var pt = new TrackPoint();
                                pt.LatitudeDeg = double.Parse(nodeTrkpt.Attributes["lat"].Value, xmlFormatProvider);
                                pt.LongitudeDeg = double.Parse(nodeTrkpt.Attributes["lon"].Value, xmlFormatProvider);
                                XmlElement timeElement = nodeTrkpt["time"];
                                if (timeElement != null)
                                    pt.Time = USDTPParse(timeElement.InnerText, xmlFormatProvider);
                                seg.Points.Add(pt);
                            }
                        }
                        trk.Segments.Add(seg);
                    }
                }
                gpx.Tracks.Add(trk);
            }
            return gpx;
        }

        private static DateTime USDTPParse(string text, IFormatProvider xmlFormatProvider) // USDTP = Universal Sortable Date Time Pattern
        {
            if (text.Length == 18) text = "20" + text; //17-04-06T10:51:05Z
            return DateTime.Parse(text, xmlFormatProvider);
        }

        public List<TrackPoint> GetPointList()
        {
            var result = new List<TrackPoint>();
            foreach (Track track in Tracks)
            {
                foreach (TrackSegment segment in track.Segments)
                {
                    foreach (TrackPoint point in segment.Points)
                    {
                        result.Add(point);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Выгружает содержимое экземпляра в xml-файл.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {
            var serializer = new XmlSerializer(typeof(GPS.Gpx));
            using (var fileStream = new  FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(fileStream, this);
            }
        }

    }
}
