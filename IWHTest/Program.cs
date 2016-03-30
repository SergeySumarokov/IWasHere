using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Primitives;

namespace IWHTest
{
    class Program
    {
        static void Main(string[] args)
        {

            // Загружаем границы областей

            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            XmlDocument xml = new XmlDocument();
            XmlNamespaceManager prefix;
            // Питер в границах КАД
            xml.Load(@"\Projects\IWasHere\Resources\RU-SPE_area.gpx");
            prefix = new XmlNamespaceManager(xml.NameTable);
            prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            var areaSpb = new Area();
            foreach (XmlNode n in xml.SelectNodes("//prfx:gpx/prfx:rte/prfx:rtept", prefix))
            {
                double lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
                double lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
                areaSpb.Points.Add(new Coordinates(lat, lon, 0));
            }
            // Ленобласть
            xml.Load(@"\Projects\IWasHere\Resources\RU-LEN_area.gpx");
            prefix = new XmlNamespaceManager(xml.NameTable);
            prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            var areaLen = new Area();
            foreach (XmlNode n in xml.SelectNodes("//prfx:gpx/prfx:rte/prfx:rtept", prefix))
            {
                double lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
                double lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
                areaLen.Points.Add(new Coordinates(lat, lon, 0));
            }

            // Формируем базу OSM

            string osmFileName = @"\Temp\IWasHere\RU-LEN.osm";
            //string osmFileName = @"\Projects\IWasHere\Resources\ExampleOSM.xml";
            var OsmDb = new OSM.Database();
            //OsmDb.LoadFromXml(osmFileName);

            Console.WriteLine("Ways {0}, Nodes {1}", OsmDb.Ways.Count, OsmDb.Nodes.Count);

            // Формируем локальную базу
            
            var IwhMap = new IWH.Map();
            //IwhMap.UpdateFromOsm(OsmDb);

            Console.WriteLine("Map.Lenght={0}", IwhMap.Lenght);

            // Удаляем из локальной базы точки вне области

            Console.WriteLine("Nodes before = {0}", IwhMap.Nodes.Count);
            foreach (IWH.Node node in IwhMap.Nodes.Values.ToList())
            {
                if (areaSpb.HasPointInside(node.Coordinates) || !areaLen.HasPointInside(node.Coordinates))
                    IwhMap.Nodes.Remove(node.OsmId);
            }
            IwhMap.PackNodes();
            Console.WriteLine("Nodes after = {0}", IwhMap.Nodes.Count);

            // Записываем и считываем базу

            //IwhMap.WriteToXml(@"\Projects\IWasHere\Resources\IwhMap.xml");

            Console.WriteLine("SaveToFile complete");

            IwhMap = IWH.Map.ReadFromXml(@"\Projects\IWasHere\Resources\IwhMap.xml");

            Console.WriteLine("LoadFromFile complete");
            Console.WriteLine("Ways {0}, Nodes {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            Console.WriteLine("Map.Lenght={0}", IwhMap.Lenght);

            // Готовим массив для записи трека

            GPS.Gpx gpx = new GPS.Gpx();
            GPS.Track newTrack;
            GPS.TrackSegment newTrackSegment;
            GPS.TrackPoint newTrackPoint;
            foreach (IWH.Way way in IwhMap.Ways.Values)
            {
                newTrack = new GPS.Track();
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.OsmId.ToString() + ")";
                newTrackSegment = new GPS.TrackSegment();
                foreach (IWH.Node node in way.Nodes)
                {
                    newTrackPoint = new GPS.TrackPoint();
                    newTrackPoint.Coordinates = node.Coordinates;
                    newTrackSegment.Points.Add(newTrackPoint);
                }
                newTrack.Segments.Add(newTrackSegment);
                gpx.Tracks.Add(newTrack);
            }
            // Выгружаем в файл
            gpx.SaveToFile(@"\Projects\IWasHere\Resources\Track.gpx");

            // Конец

            Console.WriteLine( "Done. Press [Enter] to exit.");
            Console.ReadLine();
        }
    }
}
