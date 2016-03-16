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
            //string osmFileName = "/Projects/IWasHere/Resources/RU-SPE.osm";
            string osmFileName = "/Projects/IWasHere/Resources/ExampleOSM.xml";
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            OSM.Ways ways = new OSM.Ways();
            OSM.Nodes nodes = new OSM.Nodes();

            /// Первым проходом читаем линии и сохраняем только нужные
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "way")
                    {
                        // Создаем новую строку
                        OSM.Way newWay = new OSM.Way();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode n = xmlDoc.SelectSingleNode("way");
                        newWay.Id = Int64.Parse(n.Attributes["id"].Value, xmlFormatProvider);
                        newWay.Attributes = OSM.Attributes.FromXmlNode(n);
                        /// Загрузка тэгов
                        foreach (XmlNode tag in xmlDoc.SelectNodes("/way/tag"))
                        {
                            newWay.Tags.Add(tag.Attributes["k"].Value, tag.Attributes["v"].Value);
                        }
                        /// Сохраняем только нужные линии
                        var highwayList = new List<string> { "motorway","motorway_link","trunk","trunk_link","primary","primary_link","secondary","secondary_link"};
                        if (newWay.Tags.ContainsKey("highway"))
                        {
                            if (highwayList.Contains(newWay.Tags["highway"]))
                            {
                                ways.Add(newWay.Id, newWay);
                                /// Загрузка идентификаторов узлов
                                foreach (XmlNode nd in xmlDoc.SelectNodes("/way/nd"))
                                {
                                    Int64 id = Int64.Parse(nd.Attributes["ref"].Value, xmlFormatProvider);
                                    OSM.Node newNode;
                                    if (nodes.ContainsKey(id))
                                    {
                                        newNode = nodes[id];
                                    }
                                    else
                                    {
                                        newNode = new OSM.Node() { Id = id };
                                        nodes.Add(newNode.Id, newNode);
                                    }
                                    newWay.Nodes.Add(newNode);
                                }
                                foreach (OSM.Node node in newWay.Nodes)
                                {
                                    if (!nodes.ContainsKey(node.Id))
                                    {
                                        nodes.Add(node.Id, node);
                                    }
                                }
                            }
                        }
                        /// Линия отработана
                    }
                }
            }
            Console.WriteLine("Ways.count={0}", ways.Count);

            /// Вторым проходом собираем точки
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "node")
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode n = xmlDoc.SelectSingleNode("node");
                        Int64 Id = Int64.Parse(n.Attributes["id"].Value, xmlFormatProvider);
                        // Заполняем данный только точек, участвующих в сохраненных линиях
                        if (nodes.ContainsKey(Id))
                        {
                            OSM.Node node = nodes[Id];
                            node.Id = Id;
                            node.Attributes = OSM.Attributes.FromXmlNode(n);
                            node.Lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
                            node.Lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
                            node.Tags.Clear();
                            foreach (XmlNode tag in xmlDoc.SelectNodes("/node/tag"))
                            {
                                node.Tags.Add(tag.Attributes["k"].Value, tag.Attributes["v"].Value);
                            }
                        }
                        // Точка отработана
                    }
                }
                Console.WriteLine("Nodes.count={0}", nodes.Count);
            }

            // Удаляем из линий точки без координат
            var wayList = ways.Values.ToList();
            foreach (var way in wayList)
            {
                var nodeList = way.Nodes.ToList();
                foreach (var node in nodeList)
                {
                    if (node.Coordinates.IsEmpty)
                    {
                        way.Nodes.Remove(node);
                    }
                }
                if (way.Nodes.Count<2)
                {
                    ways.Remove(way.Id);
                }

            }

            // Готовим массив для записи трека
            GPS.Gpx gpx = new GPS.Gpx();
            GPS.Track newTrack;
            GPS.TrackSegment newTrackSegment;
            GPS.TrackPoint newTrackPoint;
            foreach (OSM.Way way in ways.Values)
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
