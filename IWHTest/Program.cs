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

            OSM.Ways ways = new OSM.Ways();
            OSM.Nodes nodes = new OSM.Nodes();

            /// Первым проходом читаем линии и сохраняем только нужные
            using (XmlReader xml = XmlReader.Create("D:/Projects/IWasHere/Resources/ExampleOSM.xml"))
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        if (xml.Name == "way")
                        {
                            OSM.Way osmWay = OSM.Way.FromXml(xml.ReadOuterXml(),nodes);
                            if (osmWay.Tags.ContainsKey("highway"))
                            {
                                string highway = osmWay.Tags["highway"];
                                if (highway == "motorway" || highway == "trunk" || highway == "primary" || highway == "secondary")
                                {
                                    Console.WriteLine("{0}={1}", osmWay.Id, highway);
                                    Console.WriteLine("------------------------------------------");
                                    ways.Add(osmWay);
                                }
                            }
                        }
                    }

                }
            }

            /// Вторым проходом собираем точки
            using (XmlReader xml = XmlReader.Create("D:/Projects/IWasHere/Resources/ExampleOSM.xml"))
            { 

                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        if (xml.Name == "node")
                        {
                            Int64 Id = Int64.Parse(n.Attributes["id"].Value);

                            OSM.Node osmNode = OSM.Node.FromXml(xml.ReadOuterXml());

                            Console.WriteLine("Node {0}, {1}, {2}", osmNode.Id, osmNode.Lat, osmNode.Lon);
                            foreach (KeyValuePair<string, string> t in osmNode.Tags)
                            {
                                Console.WriteLine("{0}={1}", t.Key, t.Value);
                            }
                            Console.WriteLine("------------------------------------------");
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
