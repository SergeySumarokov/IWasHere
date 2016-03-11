using System;
using System.Collections.Generic;
using System.Xml;

namespace OSM
{
    /// <summary>
    /// A way is an ordered list of nodes which normally also has at least one tag or is included within a Relation.
    /// </summary>
    /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist. A way can be open or closed. A closed way is one whose last node on the way is also the first on that way. A closed way may be interpreted either as a closed polyline, or an area, or both.</remarks>
    public class Way
    {

        /// <summary>
        /// Way id.
        /// </summary>
        public Int64 Id;

        /// <summary>
        /// Оrdered list of nodes.
        /// </summary>
        /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist.</remarks>
        public List<Node> Nodes { get; private set; }

        /// <summary>
        /// See Map Features for tagging guidelines.
        /// </summary>
        /// <remarks>A set of key/value pairs, with unique key.</remarks>
        public Dictionary<string, string> Tags { get; private set; }

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Way()
        {
            Nodes = new List<Node>();
            Tags = new Dictionary<string, string>();
        }

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Возвращает новый экземпляр класса с данными, загруженный из фрагмента xml-кода.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Way FromXml(string xmlString, Nodes globalNodes)
        {
            Way newWay = new Way();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            XmlNode n = xmlDoc.SelectSingleNode("way");
            newWay.Id = Int64.Parse(n.Attributes["id"].Value, xmlFormatProvider);
            /// Загрузка идетификаторов узлов
            foreach (XmlNode nd in xmlDoc.SelectNodes("/way/nd"))
            {
                Int64 id = Int64.Parse(nd.Attributes["ref"].Value, xmlFormatProvider);
                if (globalNodes.ContainId(id))
                {
                    newWay.Nodes.Add(globalNodes.GetById(id));
                }
                else
                {
                    Node newNode = new Node();
                    newNode.Id = id;
                    newWay.Nodes.Add(newNode);
                }
            }
            /// Загрузка тэгов
            foreach (XmlNode t in xmlDoc.SelectNodes("/way/tag"))
            {
                newWay.Tags.Add(t.Attributes["k"].Value, t.Attributes["v"].Value);
            }
            return newWay;
        }

    }

    public class Ways
    {

        Dictionary<Int64, Way> _ways;

        public Ways()
        {
            _ways = new Dictionary<Int64, Way>();
        }

        public int Count
        {
            get { return _ways.Count; }
        }

        public void Clear()
        {
            _ways.Clear();
        }

        public void Add(Way way)
        {
            _ways.Add(way.Id, way);
        }

        public void Remove(Way way)
        {
            _ways.Remove(way.Id);
        }

    }

}
