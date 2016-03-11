using System;
using System.Collections.Generic;
using System.Xml;
using Primitives;

namespace OSM
{
    /// <summary>
    /// A node represents a specific point on the earth's surface defined by its latitude and longitude.
    /// </summary>
    /// <remarks>64-bit integer number ≥ 1</remarks>
    public class Node
    {

        /// <summary>
        /// Node ids are unique between nodes.
        /// </summary>
        /// <remarks>64-bit integer number ≥ 1. 
        /// Editors may temporarily save node ids as negative to denote ids that haven't yet been saved to the server. Node ids on the server are persistent, meaning that the assigned id of an existing node will remain unchanged each time data are added or corrected. Deleted node ids must not be reused, unless a former node is now undeleted.</remarks>
        public Int64 Id;

        /// <summary>
        /// 
        /// </summary>
        public Coordinates Coordinates;

        /// <summary>
        /// Latitude coordinate in degrees (North of equator is positive) using the standard WGS84 projection.
        /// </summary>
        /// <remarks>decimal number ≥ −90.0000000 and ≤ 90.0000000 with 7 decimal places.
        /// Some applications may not accept latitudes above/below ±85 degrees for some projections.</remarks>
        public double Lat
        {
            get { return Coordinates.Latitude.Degrees; }
            set { Coordinates.Latitude.Degrees = value; }
        }

        /// <summary>
        /// Longitude coordinate in degrees (East of Greenwich is positive) using the standard WGS84 projection.
        /// </summary>
        /// <remarks>decimal number ≥ −180.0000000 and ≤ 180.0000000 with 7 decimal places.
        /// Note that the geographic poles will be exactly at latitude ±90 degrees but in that case the longitude will be set to an arbitrary value within this range.</remarks>
        public double Lon
        {
            get { return Coordinates.Longitude.Degrees; }
            set { Coordinates.Longitude.Degrees = value; }
        }

        /// <summary>
        /// See Map Features for tagging guidelines.
        /// </summary>
        /// <remarks>A set of key/value pairs, with unique key.</remarks>
        public Dictionary<string, string> Tags { get; private set; }

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Node()
        {
            Tags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса, определяя поле Id.
        /// </summary>
        public Node(Int64 Id):this()
        {
            this.Id = Id;
        }

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Загружает данные в поля экземпляра класса из фрагмента xml-кода.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public void LoadXml(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            XmlNode n = xmlDoc.SelectSingleNode("node");
            Id = Int64.Parse(n.Attributes["id"].Value, xmlFormatProvider);
            Lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
            Lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
            Tags.Clear();
            foreach (XmlNode t in xmlDoc.SelectNodes("/node/tag"))
            {
                Tags.Add(t.Attributes["k"].Value, t.Attributes["v"].Value);
            }
        }

        /// <summary>
        /// Возвращает новый экземпляр класса с данными, загруженными из фрагмента xml-кода.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static Node FromXml(string xmlString)
        {
            Node newNode = new Node();
            newNode.LoadXml(xmlString);
            return newNode;
        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class Nodes
    {

        Dictionary<Int64, Node> _nodes;

        public Nodes()
        {
            _nodes = new Dictionary<Int64, Node>();
        }

        public int Count
        {
            get { return _nodes.Count; }
        }

        public bool ContainId(Int64 Id)
        {
            return _nodes.ContainsKey(Id);
        }

        public Node GetById(Int64 Id)
        {
            return _nodes[Id];
        }
        
        public void Clear()
        {
            _nodes.Clear();
        }

        public void Add(Node node)
        {
            _nodes.Add(node.Id, node);
        }

        public void Remove(Node node)
        {
            _nodes.Remove(node.Id);
        }

    }
        
}
