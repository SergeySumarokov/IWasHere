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
    /// Представляет картографические данные приложения.
    /// </summary>
    [XmlRoot("map")]
    public class Map : IXmlSerializable
    {

        #region "Поля и свойства"

        /// <summary>
        /// Индексированный список узлов.
        /// </summary>
        public Dictionary<Int64, Node> Nodes { get; private set; }

        /// <summary>
        /// Индексированный список линий.
        /// </summary>
        public Dictionary<Int64, Way> Ways { get; private set; }

        /// <summary>
        /// Общая протяженность обязательных линий.
        /// </summary>
        public Distance TargetLenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков обязательных линии.
        /// </summary>
        public Distance TargetVisitedLenght;

        /// <summary>
        /// Общая протяженность всех линий.
        /// </summary>
        public Distance TotalLenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков всех линии.
        /// </summary>
        public Distance TotalVisitedLenght;

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Map()
        {
            Nodes = new Dictionary<Int64, Node>();
            Ways = new Dictionary<Int64, Way>();
        }

        #endregion

        #region "Методы и функции"

        /// <summary>
        /// Выполняет пересчет параметров всех линий.
        /// </summary>
        public void Recalculate()
        {
            var targetWayTypes = new List<IWH.HighwayType>() { IWH.HighwayType.Motorway, IWH.HighwayType.Trunk, IWH.HighwayType.Primary, IWH.HighwayType.Secondary };
            TotalLenght = Distance.Zero;
            TotalVisitedLenght = Distance.Zero;
            TargetLenght = Distance.Zero;
            TargetVisitedLenght = Distance.Zero;
            foreach (Way way in Ways.Values)
            {
                // Пересчитываем линию
                way.Recalculate();
                if (targetWayTypes.Contains(way.Type))
                {
                    TargetLenght += way.Lenght;
                    TargetVisitedLenght += way.VisitedLenght;
                }
                TotalLenght += way.Lenght;
                TotalVisitedLenght += way.VisitedLenght;
            }

        }

        /// <summary>
        /// Возвращает новый экземпляр с данными, загруженными из xml-файла.
        /// </summary>
        /// <param name="fileName"></param>
        public static Map ReadFromXml(string fileName)
        {

            var serializer = new XmlSerializer(typeof(IWH.Map));
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                return (Map)serializer.Deserialize(fileStream);
            }

        }

        /// <summary>
        /// Выгружает данные экземпляра в xml-файл.
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

        #endregion

        #region "Реализация IXmlSerializable"

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
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "way")
                {
                    //Линии
                    xmlDoc.LoadXml(reader.ReadOuterXml());
                    XmlNode xmlWay = xmlDoc.SelectSingleNode("way");
                    var way = new Way();
                    // Аттрибуты
                    way.Name = xmlWay.Attributes["name"].Value;
                    way.Type = (HighwayType)Enum.Parse(typeof(HighwayType), xmlWay.Attributes["type"].Value);
                    way.IsLink = Boolean.Parse(xmlWay.Attributes["link"].Value);
                    way.Surface = (HighwaySurface)Enum.Parse(typeof(HighwaySurface), xmlWay.Attributes["surface"].Value);
                    way.Smoothness = (HighwaySmoothness)Enum.Parse(typeof(HighwaySmoothness), xmlWay.Attributes["smoothness"].Value);
                    way.Lighting = Boolean.Parse(xmlWay.Attributes["lighting"].Value);
                    way.Lanes = Byte.Parse(xmlWay.Attributes["lanes"].Value);
                    way.OneWay = Boolean.Parse(xmlWay.Attributes["oneway"].Value);
                    way.IsVisited = Boolean.Parse(xmlWay.Attributes["visited"].Value);
                    way.LastVisitedTime = DateTime.Parse(xmlWay.Attributes["last"].Value, xmlFormatProvider);
                    way.Id = Int64.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                    // Точки
                    XmlNodeList xmlRefs = xmlDoc.SelectNodes("/way/ref");
                    for (int i = 0; i < xmlRefs.Count; i++)
                    {
                        XmlNode xmlRef = xmlRefs[i];
                        Int64 id = Int64.Parse(xmlRef.Attributes["id"].Value, xmlFormatProvider);
                        Node node = Nodes[id]; // Должна существовать
                        way.Nodes.Add(node);
                        // Увеличиваем счетчик использования существующих точек, для не первой и не последней - еще раз
                        Nodes[node.Id].UseCount++;
                        if (!(i == 0 || i == xmlRefs.Count - 1))
                            Nodes[node.Id].UseCount++;
                        // Устанавливаем увеличенный радиус для точек, входящих в _link
                        if (way.IsLink)
                            node.Range = new Distance(0.2, Distance.Unit.Kilometers);
                    }
                    Ways.Add(way.Id, way);
                }
                else
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "node")
                    {
                        // Точки
                        var node = (Node)nodeSerializer.Deserialize(reader);
                        if (node.Range.IsEmpty)
                            node.Range = new Distance(0.05, Distance.Unit.Kilometers);
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

        #region "Загрузка из OSM"

        /// <summary>
        /// Загружает в экземпляр данные из osm-файла.
        /// </summary>
        public void LoadFromOsm(string osmFileName, Area IncludedArea, Area ExcludedArea)
        {

            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

            /// Первым проходом читаем линии и сохраняем только нужные
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                LoadWaysFromOsm(xml, xmlFormatProvider);
            }

            // Вторым проходом собираем точки
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                LoadNodesFromOsm(xml, xmlFormatProvider);
            }

            // Корректируем типы линий ??? - это очень некрасивая времянка
            FixLenRouteType();

            // Удаляем точки вне заданных границ
            RemoveNodesOutsideArea(IncludedArea, ExcludedArea);

            // Пересчитываем
            Recalculate();

        }

        /// <summary>
        /// Загружает из OSM линии дорог
        /// </summary>
        private void LoadWaysFromOsm(XmlReader xml, IFormatProvider xmlFormatProvider)
        {
            var highwayList = new List<string> { "motorway", "motorway_link", "trunk", "trunk_link", "primary", "primary_link", "secondary", "secondary_link", "tertiary", "tertiary_link" };
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
                                newWay.Type = HighwayType.Motorway;
                                break;
                            case "trunk":
                            case "trunk_link":
                                newWay.Type = HighwayType.Trunk;
                                break;
                            case "primary":
                            case "primary_link":
                                newWay.Type = HighwayType.Primary;
                                break;
                            case "secondary":
                            case "secondary_link":
                                newWay.Type = HighwayType.Secondary;
                                break;
                            case "tertiary":
                            case "tertiary_link":
                                newWay.Type = HighwayType.Tertiary;
                                break;
                        }
                        // Определяем другие реквизиты линии
                        newWay.Id = Int64.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                        if (tags.ContainsKey("name"))
                            newWay.Name = tags["name"];
                        if (tags.ContainsKey("lit"))
                            newWay.Lighting = (tags["lit"] == "yes");
                        if (tags.ContainsKey("lanes"))
                            try { newWay.Lanes = Byte.Parse(tags["lanes"]); } catch { }
                        if (tags.ContainsKey("oneway"))
                            newWay.OneWay = (tags["oneway"] == "yes");
                        if (tags.ContainsKey("surface"))
                        {
                            string surfaceValue = tags["surface"];
                            switch (surfaceValue)
                            {
                                case "asphalt":
                                    newWay.Surface = HighwaySurface.Asphalt;
                                    break;
                                case "concrete":
                                case "concrete:lanes":
                                case "concrete:plates":
                                    newWay.Surface = HighwaySurface.Concrete;
                                    break;
                            }
                        }
                        if (tags.ContainsKey("smoothness"))
                        {
                            string smoothnessValue = tags["smoothness"];
                            switch (smoothnessValue)
                            {
                                case "excellent":
                                    newWay.Smoothness = HighwaySmoothness.Excellent;
                                    break;
                                case "good":
                                    newWay.Smoothness = HighwaySmoothness.Good;
                                    break;
                                case "intermediate":
                                    newWay.Smoothness = HighwaySmoothness.Intermediate;
                                    break;
                                case "bad":
                                case "very_bad":
                                    newWay.Smoothness = HighwaySmoothness.Bad;
                                    break;
                                case "horrible":
                                case "very_horrible":
                                    newWay.Smoothness = HighwaySmoothness.Horrible;
                                    break;
                            }
                        }
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

        /// <summary>
        /// Загружает из OSM точки дорог и населенные пункты
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="xmlFormatProvider"></param>
        private void LoadNodesFromOsm(XmlReader xml, IFormatProvider xmlFormatProvider)
        {
            var placeList = new List<string> { "city", "town", "village" };
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
                        // Создаем новую и добавляем в список, если не была загружена ранее как узел линии
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
                        if (tags.ContainsKey("population"))
                            try { newNode.Population = Int32.Parse(tags["population"], xmlFormatProvider); } catch { }
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

        /// <summary>
        /// Удаляет точки, лежащие вне заданной области и упаковывает карту
        /// </summary>
        private void RemoveNodesOutsideArea(Area IncludedArea, Area ExcludedArea)
        {
            // Удаляем точки вне заданной области из общего массива точек
            foreach (Node node in Nodes.Values.ToList())
            {
                if (ExcludedArea.HasPointInside(node.Coordinates) || !IncludedArea.HasPointInside(node.Coordinates))
                    Nodes.Remove(node.Id);
            }
            // Удаляем у линий точки, отсутствующие в общем массиве
            // и удаляем из общего массива саму линию, если в ней мешьше двух точек
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
        /// Корректировка ненормальных типов линиц в восточнее Лодейного поля
        /// </summary>
        private void FixLenRouteType()
        {
            Int64[] FixIDs = 
            {
                145393611,
                150898900,
                150898894,
                227455595,
                102952820,
                102952836,
                227455596,
                102970752,
                102970770,
                409850284,
                409850285,
                199884323,
                93895824,
                107851192,
                107851203,
                227455594,
                107851196,
                107851208,
                107851200,
                107851131
            };
            foreach (Int64 Id in FixIDs)
            {
                Ways[Id].Type = HighwayType.Tertiary;
            }
        }

        #endregion

    }

}
