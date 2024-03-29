﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Primitives;
using Geography;


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
        public Dictionary<long, Node> Nodes { get; private set; }

        /// <summary>
        /// Cписок линий.
        /// </summary>
        public List<Way> Ways { get; private set; }

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
            Nodes = new Dictionary<long, Node>();
            Ways = new List<Way>();
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
            foreach (Way way in Ways)
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
        /// Формирует список всех всех участков пути на карте.
        /// </summary>
        /// <returns></returns>
        public List<Leg> GetLegsList()
        {
            List<Leg> result = new List<Leg>();
            foreach (Way way in Ways)
            {
                foreach (Leg leg in way.Legs)
                {
                    result.Add(leg);
                }
            }
            return result;
        }

        /// <summary>
        /// Разделяет существующие пути по точкам, являющимся перекрестками
        /// </summary>
        public void DivideWaysByCrossroads()
        {
            // Работаем с копией списка, потому что оригинал будем править
            foreach (Way way in Ways.ToList())
            {
                int legIndex = int.MaxValue;
                while (legIndex > 0)
                {
                    legIndex = GetFirstLegAfterCrossroad(way);
                    if (legIndex > 0)
                    {
                        Way newWay = way.CutLegs(legIndex);
                        Ways.Add(newWay);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает индекс первого участка пути за перекрестком или -1, если перекрестков нет
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private int GetFirstLegAfterCrossroad(Way way)
        {
            int result = -1;
            // Проверяем со второго участка, потому что начинать путь с перекрестка - это нормально
            for (int i=1; i<way.Legs.Count; i++)
            {
                // Если начальная точка участка участвует больше чем в двух участках - это участок за перекрестком
                if (way.Legs[i].StartPoint.Legs.Count > 2)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Разделяет существующие пути максимальной длине
        /// </summary>
        public void DivideWaysByLenght()
        {
            Distance maxLenght = Distance.FromKilometers(4);
            int minLegsCount = 2;
            // Работаем с копией списка, потому что оригинал будем править
            foreach (Way way in Ways.ToList())
            {
                Distance maxThisWaylenght = way.Lenght / (Math.Floor(way.Lenght / maxLenght)+1);
                while (way.Lenght > maxThisWaylenght && way.Legs.Count >= minLegsCount)
                {
                    int legIndex = GetFirstLegByLenght(way, maxThisWaylenght);
                    // Не допускаем разделения по последней точке
                    if (legIndex == way.Legs.Count-1)
                        legIndex -= 1;
                    // Делим если можем
                    if (legIndex >= 0)
                    {
                        Way newWay = way.CutLegs(legIndex+1);
                        Ways.Add(newWay);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает индекс первого участка пути, на котором путь превышает заданную длину
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private int GetFirstLegByLenght(Way way, Distance maxLenght)
        {
            Distance segmentLenght = Distance.Zero;
            int result = -1;
            for (int i = 0; i < way.Legs.Count; i++)
            {
                segmentLenght += way.Legs[i].Lenght;
                if (segmentLenght > maxLenght)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Объединяет слишком короткии пути со смежными
        /// </summary>
        public void CombineShortWaysWithAdjacent()
        {
            Distance maxWayLenght = Distance.FromKilometers(1);
            int maxLegsCount = 2;
            // Объединяем в случае совпадения параметров и направлений
            foreach (Way way in Ways.ToList())
            {
                if (way.Legs.Count <= maxLegsCount && way.Lenght <= maxWayLenght)
                {
                    // Сначала проверяем первую точку пути
                    Leg goodLeg = CheckPointForPossibilityCombining(way.FirstPoint, way.FirstLeg);
                    if (goodLeg != null)
                    {
                        goodLeg.Way.CombineLegs(way);
                        Ways.Remove(way);
                    }
                    // Потом, если не получилось, проверяем последнюю точку пути
                    if (goodLeg == null)
                    {
                        goodLeg = CheckPointForPossibilityCombining(way.LastPoint, way.LastLeg);
                        if (goodLeg != null)
                        {
                            goodLeg.Way.CombineLegs(way);
                            Ways.Remove(way);
                        }
                    }

                }
            }

        }

        /// <summary>
        /// Возвращает участок, к которому возможно присоединить заданный участок или null если такого нет
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sourceLeg"></param>
        /// <returns></returns>
        private Leg CheckPointForPossibilityCombining(Node node, Leg sourceLeg)
        {
            Angle maxLegDirectionDeviation = Angle.FromDegrees(5);
            Leg result = null;
            if (node.Legs.Count > 1)
            {
                foreach (Leg leg in node.Legs)
                {
                    if (
                        !leg.Equals(sourceLeg)
                        && (leg.Direction-sourceLeg.Direction).Cos() > maxLegDirectionDeviation.Cos()
                        && leg.Way.Type == sourceLeg.Way.Type
                        && leg.Way.OneWay == sourceLeg.Way.OneWay
                        && leg.Way.Name == sourceLeg.Way.Name
                        )
                    {
                        result = leg;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Изменяет тип пути на заданный для всех путей, начальная и конечная точки которых лежат в заданной области
        /// </summary>
        /// <param name="area"></param>
        /// <param name="targetType"></param>
        public void FixWayTypeInArea(Geography.GeoArea area, IWH.HighwayType targetType)
        {
            if (area == null)
                return;
            foreach (Way way in Ways)
            {
                if (area.HasPointInside(way.FirstPoint) && area.HasPointInside(way.LastPoint))
                    way.Type = targetType;
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
                    way.OsmId = long.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                    way.Name = xmlWay.Attributes["name"].Value;
                    way.Type = (HighwayType)Enum.Parse(typeof(HighwayType), xmlWay.Attributes["type"].Value);
                    way.IsLink = bool.Parse(xmlWay.Attributes["link"].Value);
                    way.Surface = (HighwaySurface)Enum.Parse(typeof(HighwaySurface), xmlWay.Attributes["surface"].Value);
                    way.Smoothness = (HighwaySmoothness)Enum.Parse(typeof(HighwaySmoothness), xmlWay.Attributes["smoothness"].Value);
                    way.Lighting = bool.Parse(xmlWay.Attributes["lighting"].Value);
                    way.Lanes = byte.Parse(xmlWay.Attributes["lanes"].Value);
                    way.OneWay = bool.Parse(xmlWay.Attributes["oneway"].Value);
                    way.IsVisited = bool.Parse(xmlWay.Attributes["visited"].Value);
                    way.LastVisitedTime = DateTime.Parse(xmlWay.Attributes["last"].Value, xmlFormatProvider);
                    // Точки
                    Node point;
                    Leg newLeg = null;
                    XmlNodeList xmlRefs = xmlDoc.SelectNodes("/way/ref");
                    for (int i = 0; i < xmlRefs.Count; i++)
                    {
                        XmlNode xmlRef = xmlRefs[i];
                        point = Nodes[long.Parse(xmlRef.Attributes["id"].Value, xmlFormatProvider)];
                        // Для всех точек кроме последней создаём новые участки
                        if (i < xmlRefs.Count-1)
                        {
                            newLeg = new Leg();
                            newLeg.Way = way;
                            newLeg.StartPoint = point;
                            newLeg.StartPoint.Legs.Add(newLeg);
                            newLeg.IsVisited = bool.Parse(xmlRef.Attributes["visited"].Value);
                            newLeg.VisitedCount = int.Parse(xmlRef.Attributes["count"].Value, xmlFormatProvider);
                            newLeg.LastVisitedTime = DateTime.Parse(xmlRef.Attributes["last"].Value, xmlFormatProvider);
                            way.Legs.Add(newLeg);
                        }
                        // Для всех точек кроме первой задаём её концом предыдущего участка
                        if (i > 0)
                        {
                            way.Legs[i-1].EndPoint = point;
                            way.Legs[i-1].EndPoint.Legs.Add(way.Legs[i-1]);
                            way.Legs[i-1].Recalculate();
                        }
                    }
                    Ways.Add(way);
                }
                else
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "node")
                    {
                        // Точки
                        var node = (Node)nodeSerializer.Deserialize(reader);
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
            foreach (Way way in Ways)
            {
                waySerializer.Serialize(writer, way);
            }

        }

        #endregion

        #region "Загрузка из OSM"

        /// <summary>
        /// Загружает в экземпляр данные из osm-файла.
        /// </summary>
        /// <remarks>Требуется последущий пересчет участков и линий.</remarks>
        public void LoadFromOsm(string[] osmFileNames, GeoArea IncludedArea, GeoArea ExcludedArea)
        {

            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            // Обходим все файлы
            foreach (string osmFileName in osmFileNames)
            {
                /// Первым проходом читаем линии и сохраняем только нужные
                using (XmlReader xml = XmlReader.Create(osmFileName))
                    LoadWaysFromOsm(xml, xmlFormatProvider);

                // Вторым проходом собираем точки
                using (XmlReader xml = XmlReader.Create(osmFileName))
                    LoadNodesFromOsm(xml, xmlFormatProvider);
            }
            // Удаляем точки вне заданных границ
            RemoveNodesOutsideArea(IncludedArea, ExcludedArea);

        }

        /// <summary>
        /// Загружает из OSM линии дорог
        /// </summary>
        private void LoadWaysFromOsm(XmlReader xml, IFormatProvider xmlFormatProvider)
        {
            // Формируем список идентификаторов существующих линий
            var waysDict = new Dictionary<long, Way>();
            foreach (Way way in Ways)
            {
                waysDict.Add(way.OsmId, way);
            }
            //
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
                        newWay.OsmId = long.Parse(xmlWay.Attributes["id"].Value, xmlFormatProvider);
                        if (tags.ContainsKey("name"))
                            newWay.Name = tags["name"];
                        if (tags.ContainsKey("lit"))
                            newWay.Lighting = (tags["lit"] == "yes");
                        if (tags.ContainsKey("lanes"))
                            try { newWay.Lanes = byte.Parse(tags["lanes"]); } catch { }
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
                        List<Node> wayNodes = new List<Node>();
                        foreach (XmlNode xmlNd in xmlDoc.SelectNodes("/way/nd"))
                        {
                            long id = long.Parse(xmlNd.Attributes["ref"].Value, xmlFormatProvider);
                            if (Nodes.ContainsKey(id))
                            {
                                newNode = Nodes[id];
                            }
                            else
                            {
                                newNode = new Node() { OsmId = id };
                                Nodes.Add(newNode.OsmId, newNode);
                            }
                            wayNodes.Add(newNode);
                        }
                        // Создаём участки
                        Leg newLeg;
                        for (int i = 0; i < wayNodes.Count-1; i++)
                        {
                            newLeg = new Leg();
                            newLeg.StartPoint = wayNodes[i];
                            newLeg.EndPoint = wayNodes[i + 1];
                            newWay.Legs.Add(newLeg);
                        }
                        // Сохраняем нужную линию
                        if (!waysDict.ContainsKey(newWay.OsmId))
                        {
                            waysDict.Add(newWay.OsmId, newWay);
                            Ways.Add( newWay);
                        }
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
                    long id = long.Parse(xmlNode.Attributes["id"].Value, xmlFormatProvider);
                    // Загружаем тэги
                    var tags = new Dictionary<string, string>();
                    foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/node/tag"))
                        tags.Add(xmlTag.Attributes["k"].Value, xmlTag.Attributes["v"].Value);
                    // Нам нужны только точки, участвующие в сохраненных линиях, или точки заданного типа
                    Node newNode = null;
                    bool goodNode = false;
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
                            newNode = new Node() { OsmId = id };
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
                            try { newNode.Population = int.Parse(tags["population"], xmlFormatProvider); } catch { }
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
        private void RemoveNodesOutsideArea(GeoArea IncludedArea, GeoArea ExcludedArea)
        {
            // Удаляем точки вне заданной области из общего массива точек
            foreach (Node node in Nodes.Values.ToList())
            {

                //
                //if (node.OsmId == 1875009848) throw new Exception("???");

                if (node.Coordinates.IsEmpty || IncludedArea != null && !IncludedArea.HasPointInside(node) || (ExcludedArea != null && ExcludedArea.HasPointInside(node)))
                    Nodes.Remove(node.OsmId);
            }
            // Удаляем у линий участки, точки которых отсутствующие в общем массиве
            // и удаляем из общего массива саму линию, если в ней нет участков
            foreach (Way way in Ways.ToList())
            {
                foreach (Leg leg in way.Legs.ToList())
                {

                    //
                    //if (leg.StartPoint.OsmId == 1875009848 || leg.EndPoint.OsmId == 1875009848) throw new Exception("???");

                    if (!Nodes.ContainsKey(leg.StartPoint.OsmId) || !Nodes.ContainsKey(leg.EndPoint.OsmId))
                        way.Legs.Remove(leg);
                }
                if (way.Legs.Count == 0)
                    Ways.Remove(way);
            }

        }

        #endregion

    }

}
