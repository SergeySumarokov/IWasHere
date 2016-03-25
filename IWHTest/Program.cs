using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWHTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Формируем базу OSM
            string osmFileName = "/Projects/IWasHere/Resources/RU-SPE.osm";
            //string osmFileName = "/Projects/IWasHere/Resources/ExampleOSM.xml";
            var OsmDb = new OSM.Database();
            OsmDb.LoadFromXml(osmFileName);

            // Готовим массив для записи трека
            GPS.Gpx gpx = new GPS.Gpx();
            GPS.Track newTrack;
            GPS.TrackSegment newTrackSegment;
            GPS.TrackPoint newTrackPoint;
            foreach (OSM.Way way in OsmDb.Ways.Values)
            {
                newTrack = new GPS.Track();
                if (way.Tags.ContainsKey("name"))
                {newTrack.Name = way.Tags["name"];}
                else
                {newTrack.Name = "<noname>"; }
                newTrack.Name += " (" + way.Tags["highway"] + " " + way.Id.ToString() + ")";
                newTrackSegment = new GPS.TrackSegment();
                foreach (OSM.Node node in way.Nodes)
                {
                    newTrackPoint = new GPS.TrackPoint();
                    newTrackPoint.Coordinates = node.Coordinates;
                    newTrackSegment.Points.Add(newTrackPoint);
                }
                newTrack.Segments.Add(newTrackSegment);
                gpx.Tracks.Add(newTrack);
            }

            // Выгружаем
            XmlSerializer formatter = new XmlSerializer(typeof(GPS.Gpx));
            using (FileStream fs = new FileStream("/Projects/IWasHere/Resources/Track_out.gpx", FileMode.Create))
            {
                formatter.Serialize(fs, gpx);
            }

            // Конец
            Console.WriteLine( "Done. Press [Enter] to exit.");
            Console.ReadLine();
        }
    }
}
