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

            // Формируем базу OSM
            string osmFileName = @"\Temp\IWasHere\RU-SPE.osm";
            //string osmFileName = @"\Projects\IWasHere\Resources\ExampleOSM.xml";
            var OsmDb = new OSM.Database();
            //OsmDb.LoadFromXml(osmFileName);

            Console.WriteLine("Ways.count={0}", OsmDb.Ways.Count);
            Console.WriteLine("Nodes.count={0}", OsmDb.Nodes.Count);

            // Формируем локальную базу
            var IwhMap = new IWH.Map();
            //IwhMap.UpdateFromOsm(OsmDb);

            Console.WriteLine("Map.Lenght={0}", IwhMap.Lenght);

            //IwhMap.WriteToXml(@"\Projects\IWasHere\Resources\IwhMap.xml");

            Console.WriteLine("SaveToFile complete");

            IwhMap = IWH.Map.ReadFromXml(@"\Projects\IWasHere\Resources\IwhMap.xml");

            Console.WriteLine("LoadFromFile complete");

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
