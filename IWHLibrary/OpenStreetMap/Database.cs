using System;
using System.Collections.Generic;
using System.Xml;

namespace OSM
{
    
    /// <summary>
    /// Представляет фрагмент базы OpenStreetMap.
    /// </summary>
    public class Database
    {

        /// <summary>
        /// Список линий.
        /// </summary>
        public Dictionary<Int64, Node> Nodes { get; private set; }

        /// <summary>
        /// Список точек.
        /// </summary>
        public Dictionary<Int64, Way> Ways { get; private set; }

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Database()
        {
            Nodes = new Dictionary<Int64, Node>();
            Ways = new Dictionary<Int64, Way>();
        }

        /// <summary>
        /// Загружает в экземпляр класса данные из xml-файла OSM.
        /// </summary>
        /// <param name="FileName"></param>
        public void LoadFromXml(string osmFileName)
        {

            Nodes.Clear();
            Ways.Clear();
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
                        OSM.Way newWay = new OSM.Way();
                        newWay.Attributes = OSM.Attributes.FromXmlNode(xmlWay);
                        /// Загрузка тэгов
                        foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/way/tag"))
                        {
                            newWay.Tags.Add(xmlTag.Attributes["k"].Value, xmlTag.Attributes["v"].Value);
                        }
                        /// Сохраняем только нужные линии
                        if (newWay.Tags.ContainsKey("highway") && highwayList.Contains(newWay.Tags["highway"]))
                        {
                            Ways.Add(newWay.Id, newWay);
                            /// Загрузка идентификаторов узлов
                            OSM.Node newNode;
                            foreach (XmlNode xmlNd in xmlDoc.SelectNodes("/way/nd"))
                            {
                                Int64 id = Int64.Parse(xmlNd.Attributes["ref"].Value, xmlFormatProvider);
                                if (Nodes.ContainsKey(id))
                                {
                                    newNode = Nodes[id];
                                }
                                else
                                {
                                    newNode = new OSM.Node() { Id = id };
                                    Nodes.Add(newNode.Id, newNode);
                                }
                                newWay.Nodes.Add(newNode);
                            }
                        }
                        /// Линия отработана
                         
                    }
                }
            }
            
            /// Вторым проходом собираем точки
            using (XmlReader xml = XmlReader.Create(osmFileName))
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "node")
                    {

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode xmlNode = xmlDoc.SelectSingleNode("node");
                        Int64 Id = Int64.Parse(xmlNode.Attributes["id"].Value, xmlFormatProvider);
                        // Заполняем даннну только точек, участвующих в сохраненных линиях
                        if (Nodes.ContainsKey(Id))
                        {
                            OSM.Node node = Nodes[Id];
                            node.Attributes = OSM.Attributes.FromXmlNode(xmlNode);
                            node.Lat = double.Parse(xmlNode.Attributes["lat"].Value, xmlFormatProvider);
                            node.Lon = double.Parse(xmlNode.Attributes["lon"].Value, xmlFormatProvider);
                            node.Tags.Clear();
                            foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/node/tag"))
                            {
                                node.Tags.Add(xmlTag.Attributes["k"].Value, xmlTag.Attributes["v"].Value);
                            }
                        }
                        // Точка отработана

                    }
                }
            }

            //
            // Здесь должно идти удаление точек вне заданной области
            //

            // Это можно удалить при случае
            //
            // Удаляем из линий точки без координат
            //var wayList = ways.Values.ToList();
            //foreach (var way in wayList)
            //{
            //    var nodeList = way.Nodes.ToList();
            //    foreach (var node in nodeList)
            //    {
            //        if (node.Coordinates.IsEmpty)
            //        {
            //            way.Nodes.Remove(node);
            //        }
            //    }
            //    if (way.Nodes.Count < 2)
            //    {
            //        ways.Remove(way.Id);
            //    }
            //}

        }

    }

}
