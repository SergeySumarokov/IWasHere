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
        /// 
        /// </summary>
        [XmlElement("trk")]
        public List<Track> Tracks { get; private set; }

        /// <summary>
        /// Инициализирует пустой экземпляр класса.
        /// </summary>
        public Gpx()
        {
            Tracks = new List<Track>();
        }

        /// <summary>
        /// Возвращает экземпляр данными из xml-файла
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
            foreach (XmlNode nodeTrk in xml.SelectNodes("//prfx:gpx/prfx:trk", prefix))
            {
                var trk = new Track();
                //trk.Name = nodeTrk.  ["name"].Value;
                foreach (XmlNode nodeTrkseg in nodeTrk.SelectNodes("//prfx:trkseg", prefix))
                {
                    var seg = new TrackSegment();
                    foreach (XmlNode nodeTrkpt in nodeTrk.SelectNodes("//prfx:trkpt", prefix))
                    {
                        var pt = new TrackPoint();
                        pt.Lat = double.Parse(nodeTrkpt.Attributes["lat"].Value, xmlFormatProvider);
                        pt.Lon = double.Parse(nodeTrkpt.Attributes["lon"].Value, xmlFormatProvider);
                        seg.Points.Add(pt);
                    }
                    trk.Segments.Add(seg);

                }
                gpx.Tracks.Add(trk);
            }
            return gpx;
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
