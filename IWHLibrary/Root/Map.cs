using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Primitives;


namespace IWH
{

    /// <summary>
    /// Представляет данные приложения.
    /// </summary>
    [XmlRoot("map")]
    public class Map : IXmlSerializable
    {

        /// <summary>
        /// Список точек.
        /// </summary>
        public Dictionary<Int64, Node> Nodes { get; private set; }

        /// <summary>
        /// Список линий.
        /// </summary>
        public Dictionary<Int64, Way> Ways { get; private set; }

        /// <summary>
        /// Общая протяженность линии.
        /// </summary>
        public Distance Lenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков линии.
        /// </summary>
        public Distance VisitedLenght;

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Map()
        {
            Nodes = new Dictionary<Int64, Node>();
            Ways = new Dictionary<Int64, Way>();
        }

        /// <summary>
        /// Загружает данные из osm-файла.
        /// </summary>
        public void LoadFromOsm(string osmFileName)
        {
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            /// Первым проходом читаем линии и сохраняем только нужные
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                var highwayList = new List<string> { "motorway", "motorway_link", "trunk", "trunk_link", "primary", "primary_link", "secondary", "secondary_link" };
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "way")
                    {
                        // Создаем новую линию
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode xmlWay = xmlDoc.SelectSingleNode("way");
                        Way newWay = new Way();
                        // Загружаем тэги
                        var tags = new Dictionary<string, string>();
                        foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/way/tag"))
                            tags.Add(xmlTag.Attributes["k"].Value, xmlTag.Attributes["v"].Value);
                        // Сохраняем только нужные линии
                        if (tags.ContainsKey("highway") && highwayList.Contains(tags["highway"]))
                        {
                            // Определяем тип линии
                            string highwayValue = tags["highway"];
                            newWay.IsLink = highwayValue.EndsWith("_link");
                            switch (highwayValue)
                            {
                                case "motorway":
                                case "motorway_link":
                                    newWay.Type = WayType.Motorway;
                                    break;
                                case "trunk":
                                case "trunk_link":
                                    newWay.Type = WayType.Trunk;
                                    break;
                                case "primary":
                                case "primary_link":
                                    newWay.Type = WayType.Primary;
                                    break;
                                case "secondary":
                                case "secondary_link":
                                    newWay.Type = WayType.Secondary;
                                    break;
                            }
                            // Определяем другие реквизиты линии
                            newWay.Id = Int64.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                            if (tags.ContainsKey("name"))
                                newWay.Name = tags["name"];
                            // Загружаем идентификаторы узлов
                            Node newNode;
                            foreach (XmlNode xmlNd in xmlDoc.SelectNodes("/way/nd"))
                            {
                                Int64 id = Int64.Parse(xmlNd.Attributes["ref"].Value, xmlFormatProvider);
                                if (Nodes.ContainsKey(id))
                                {
                                    newNode = Nodes[id];
                                }
                                else
                                {
                                    newNode = new Node() { Id = id };
                                    Nodes.Add(newNode.Id, newNode);
                                }
                                newWay.Nodes.Add(newNode);
                            }
                            // Сохраняем нужную линию
                            Ways.Add(newWay.Id, newWay);
                        }
                        // Линия отработана
                    }
                }
            }

            // Вторым проходом собираем точки
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                var placeList = new List<string> { "city","town", "village" };
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "node")
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode xmlNode = xmlDoc.SelectSingleNode("node");
                        // Загружаем аттрибуты
                        Int64 id = Int64.Parse(xmlNode.Attributes["id"].Value, xmlFormatProvider);
                        // Загружаем тэги
                        var tags = new Dictionary<string, string>();
                        foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/node/tag"))
                            tags.Add(xmlTag.Attributes["k"].Value, xmlTag.Attributes["v"].Value);
                        // Нам нужны только точки, участвующие в сохраненных линиях, или точки заданного типа
                        Node newNode = null;
                        Boolean goodNode = false;
                        if (Nodes.ContainsKey(id))
                        {
                            // Эта точка входит в нужную линию
                            goodNode = true;
                            newNode = Nodes[id];
                            newNode.Type = NodeType.Waypoint;
                        }
                        if (tags.ContainsKey("place") && placeList.Contains(tags["place"]))
                        {
                            // Эта точка описывает населенный пункт
                            goodNode = true;
                            // Создаем новую и добавляем в список, если не была загружена ранее
                            if (newNode == null)
                            {
                                newNode = new Node() { Id = id };
                                Nodes.Add(id, newNode);
                            }
                            // Заполняем специфичные для населенного пункта данные
                            switch (tags["place"])
                            {
                                case "city":
                                    newNode.Type = NodeType.City;
                                    break;
                                case "town":
                                    newNode.Type = NodeType.Town;
                                    break;
                                case "village":
                                    newNode.Type = NodeType.Village;
                                    break;
                            }
                            if (tags.ContainsKey("name"))
                                newNode.Name = tags["name"];
                        }
                        // Заполняем общие данные для нужных точек
                        if (goodNode)
                        {
                            newNode.Coordinates.Latitude.Degrees = double.Parse(xmlNode.Attributes["lat"].Value, xmlFormatProvider);
                            newNode.Coordinates.Longitude.Degrees = double.Parse(xmlNode.Attributes["lon"].Value, xmlFormatProvider);
                        }
                        // Точка отработана
                    }
                }
            }

        }

        /// <summary>
        /// Удалает из Ways точки, не используемые в Nodes, затем удаляет пустые Ways.
        /// </summary>
        public void PackNodes()
        {
            foreach (Way way in Ways.Values.ToList())
            {
                foreach (Node node in way.Nodes.ToList())
                {
                    if (!Nodes.ContainsKey(node.Id))
                        way.Nodes.Remove(node);
                }
                if (way.Nodes.Count < 2)
                    Ways.Remove(way.Id);
            }
        }

        /// <summary>
        /// Выполняет пересчет параметров всех линий.
        /// </summary>
        public void Recalculate()
        {
            Lenght = Distance.Zero;
            VisitedLenght = Distance.Zero;
            foreach (Way way in Ways.Values)
            {
                // Пересчитываем линию
                way.Recalculate();
                Lenght += way.Lenght;
                VisitedLenght += way.VisitedLenght;
            }

        }

        /// <summary>
        /// Загружает содержимое экземпляра из xml-файла.
        /// </summary>
        /// <param name="fileName"></param>
        public static Map ReadFromXml(string fileName)
        {

            var serializer = new XmlSerializer(typeof(IWH.Map));
            using (var fileStream = new FileStream(fileName,FileMode.Open))
            {
                return (Map)serializer.Deserialize(fileStream);
            }

        }

        /// <summary>
        /// Выгружает содержимое экземпляра в xml-файл.
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteToXml(string fileName)
        {

            var serializer = new XmlSerializer(typeof(IWH.Map));
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(fileStream, this);
            }

        }

        #region "IXmlSerializable Members"

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

            Nodes.Clear();
            Ways.Clear();

            var nodeSerializer = new XmlSerializer(typeof(Node));
            var xmlDoc = new XmlDocument();

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name=="way")
                {
                    //Линии
                    xmlDoc.LoadXml(reader.ReadOuterXml());
                    XmlNode xmlWay = xmlDoc.SelectSingleNode("way");
                    var way = new Way();
                    // Аттрибуты
                    way.Name = xmlWay.Attributes["name"].Value;
                    way.Type = (WayType)Enum.Parse(typeof(WayType), xmlWay.Attributes["type"].Value);
                    way.IsLink = Boolean.Parse(xmlWay.Attributes["link"].Value);
                    way.Id = Int64.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                    // Точки
                    foreach (XmlNode xmlRef in xmlDoc.SelectNodes("/way/ref"))
                    {
                        Int64 id = Int64.Parse(xmlRef.Attributes["id"].Value, xmlFormatProvider);
                        way.Nodes.Add(Nodes[id]);
                        // Устанавливаем увеличенный радиус для точек, входящих в _link
                        if (way.IsLink)
                            Nodes[id].Range = new Distance(0.5, Distance.Unit.Kilometers);
                    }
                    Ways.Add(way.Id,way);
                    // Пересчитываем линию
                    way.Recalculate();
                    Lenght += way.Lenght;
                    VisitedLenght += way.VisitedLenght;
                }
                else
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "node")
                    {
                        // Точки
                        var node = (Node)nodeSerializer.Deserialize(reader);
                        if (node.Range.IsEmpty)
                            node.Range = new Distance(0.1, Distance.Unit.Kilometers);
                        Nodes.Add(node.Id, node);
                    }
                    reader.Read();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteAttributeString("version", "1.0");
            writer.WriteAttributeString("generator", "IWasHere application");

            var waySerializer = new XmlSerializer(typeof(Way));
            var nodeSerializer = new XmlSerializer(typeof(Node));
            // Точки
            foreach (Node node in Nodes.Values)
            {
                nodeSerializer.Serialize(writer, node);
            }
            // Линии
            foreach (Way way in Ways.Values)
            {
                waySerializer.Serialize(writer, way);
            }

        }

        #endregion


    }

}
