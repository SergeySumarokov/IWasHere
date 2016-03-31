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
        /// Обновляет данные приложения данными OSM
        /// </summary>
        /// <remarks>На начальном этапе обновление не реазовано, выполняется полная перезагрузка.</remarks>
        public void UpdateFromOsm(OSM.Database osmDb )
        {

            Lenght = Distance.Zero;
            VisitedLenght = Distance.Zero;

            foreach (OSM.Way osmWay in osmDb.Ways.Values)
            {
                Way way;
                // Выбираем существующую линию или создаем новую
                if (Ways.ContainsKey(osmWay.Id))
                    way = Ways[osmWay.Id];
                else
                {
                    way = new Way();
                    Ways.Add(osmWay.Id, way);
                }
                // Заполняем новую линию или обновляем существующую, если её версия меньше версии OSM
                if (way.OsmVer == 0 || osmWay.Attributes.Version>way.OsmVer)
                {
                    // Заполняем поля
                    way.OsmId = osmWay.Attributes.Id;
                    way.OsmVer = osmWay.Attributes.Version;
                    // Тип
                    way.IsLink = osmWay.Tags["highway"].EndsWith("_link");
                    switch (osmWay.Tags["highway"])
                    {
                        case "motorway":
                        case "motorway_link":
                            way.Type = WayType.Motorway;
                            break;
                        case "trunk":
                        case "trunk_link":
                            way.Type = WayType.Trunk;
                            break;
                        case "primary":
                        case "primary_link":
                            way.Type = WayType.Primary;
                            break;
                        case "secondary":
                        case "secondary_link":
                            way.Type = WayType.Secondary;
                            break;
                    }
                    // Наименование
                    if (osmWay.Tags.ContainsKey("name"))
                        way.Name = osmWay.Tags["name"];
                    // Заполняем точки
                    way.Nodes.Clear();
                    foreach (OSM.Node osmNode in osmWay.Nodes)
                        way.Nodes.Add(NodeFromOsmNode(osmNode));
                    // Пересчитываем точки
                    way.Recalculate();
                    Lenght += way.Lenght;
                    VisitedLenght += way.VisitedLenght;
                }
                // Линия обновлена
            }

        }

        /// <summary>
        /// Удалает из Nodes точки, не используемые в Ways.
        /// </summary>
        public void PackNodes()
        {
            foreach (Way way in Ways.Values.ToList())
            {
                foreach (Node node in way.Nodes.ToList())
                {
                    if (!Nodes.ContainsKey(node.OsmId))
                        way.Nodes.Remove(node);
                }
                if (way.Nodes.Count < 2)
                    Ways.Remove(way.OsmId);
            }
        }

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
        /// Возвращает экземпляр Node с данными из экземпляра osmNode, зарегистрированный в Map.Nodes
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private Node NodeFromOsmNode(OSM.Node osmNode)
        {
            Node node;
            // Выбираем существующую точку или создаем новую
            if (Nodes.ContainsKey(osmNode.Id))
                node = Nodes[osmNode.Id];
            else
            {
                node = new Node() { OsmId = osmNode.Id };
                Nodes.Add(node.OsmId, node);
            }
            // Заполняем новую точку или обновляем существующую, если её версия меньше версии OSM
            if (node.OsmVer == 0 || osmNode.Attributes.Version > node.OsmVer)
            {
                // Заполняем поля
                node.Coordinates = osmNode.Coordinates;
                node.Type = NodeType.Waypoint;
                node.OsmVer = osmNode.Attributes.Version;
            }
            return node;
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
                    way.OsmId = Int64.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                    way.OsmVer = Int64.Parse(xmlWay.Attributes["ver"].Value, xmlFormatProvider);
                    // Точки
                    foreach (XmlNode xmlRef in xmlDoc.SelectNodes("/way/ref"))
                    {
                        Int64 id = Int64.Parse(xmlRef.Attributes["id"].Value, xmlFormatProvider);
                        way.Nodes.Add(Nodes[id]);
                        // Устанавливаем увеличенный радиус для точек, входящих в _link
                        if (way.IsLink)
                            Nodes[id].Range = new Distance(0.5, Distance.Unit.Kilometers);
                    }
                    Ways.Add(way.OsmId,way);
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
                        Nodes.Add(node.OsmId, node);
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
