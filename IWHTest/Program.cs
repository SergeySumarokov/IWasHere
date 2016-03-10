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


            using (XmlReader xml = XmlReader.Create("ExampleOfOSM.xml"))
            {
                while (xml.Read())
                {

                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        if (xml.Name == "node")
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(xml.ReadOuterXml());
                            XmlNode n = xmlDoc.SelectSingleNode("node");
                            Console.WriteLine("ID: {0}", n.Attributes["id"].Value);
                            foreach (XmlNode t in xmlDoc.SelectNodes("/node/tag"))
                            {
                                Console.WriteLine("k: {0}", t.Attributes["k"].Value);
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
