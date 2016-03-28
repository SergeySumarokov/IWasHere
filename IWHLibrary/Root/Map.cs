using System;
using System.IO;
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
        [XmlIgnore]
        public Distance Lenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков линии.
        /// </summary>
        [XmlIgnore]
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
                    VisitedLenght += VisitedLenght;
                }
                // Линия обновлена
            }

        }

        /// <summary>
        /// Удалает из Nodes точки, не используемые в Ways.
        /// </summary>
        private void PackNodes()
        {
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
        public void ReadFromXml(string fileName)
        {

            var serializer = new XmlSerializer(typeof(IWH.Map));
            using (var fileStream = new FileStream(fileName,FileMode.Open))
            {
                var map = (Map)serializer.Deserialize(fileStream);
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


        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

            var waySerializer = new XmlSerializer(typeof(Way));

            reader.Read();
            if (reader.IsEmptyElement) return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                var way = (Way)waySerializer.Deserialize(reader);
            }
            reader.ReadEndElement();

        }

        public void WriteXml(XmlWriter writer)
        {

            var waySerializer = new XmlSerializer(typeof(Way));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            foreach (Way way in Ways.Values)
            {
                waySerializer.Serialize(writer, way, namespaces);
            }

        }

        #endregion


    }

}
