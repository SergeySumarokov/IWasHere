using System;
using System.Xml;
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
            //string osmFileName = "P:/Обменник/OSM/RU-SPE.osm";
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
                        /// Загрузка идентификаторов узлов
                        foreach (XmlNode nd in xmlDoc.SelectNodes("/way/nd"))
                        {
                            Int64 id = Int64.Parse(nd.Attributes["ref"].Value, xmlFormatProvider);
                            OSM.Node newNode = new OSM.Node();
                            newNode.Id = id;
                            newWay.Nodes.Add(newNode);
                        }
                        /// Загрузка тэгов
                        foreach (XmlNode tag in xmlDoc.SelectNodes("/way/tag"))
                        {
                            newWay.Tags.Add(tag.Attributes["k"].Value, tag.Attributes["v"].Value);
                        }
                        /// Сохраняем только нужные линии
                        if (newWay.Tags.ContainsKey("highway"))
                        {
                            string highway = newWay.Tags["highway"];
                            if (highway == "motorway" || highway == "trunk" || highway == "primary" || highway == "secondary")
                            {
                                ways.Add(newWay.Id, newWay);
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
            Console.ReadLine();
        }
    }
}
