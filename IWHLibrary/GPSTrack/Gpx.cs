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

        public void LoadFromFile(string fileName)
        {

        }

        /// <summary>
        /// Выгружает содержимое экземпляра в xml-файл.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {

            var serializer = new XmlSerializer(typeof(GPS.Gpx));
            using (var fileStream = new  FileStream("/Projects/IWasHere/Resources/Track_out.gpx", FileMode.Create))
            {
                serializer.Serialize(fileStream, this);
            }

        }

    }
}
