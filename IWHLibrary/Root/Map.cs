using System;
using System.Collections.Generic;

namespace IWH
{

    /// <summary>
    /// Представляет данные приложения.
    /// </summary>
    public class Map
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

            foreach (OSM.Way osmWay in osmDb.Ways.Values)
            {
                Way way;
                // Выбираем существующую линию или создаем новую
                if (Ways.ContainsKey(osmWay.Id))
                    way = Ways[osmWay.Id];
                else
                    way = new Way();
                // Заполняем новую линию или обновляем существующую, если её версия меньше версии OSM
                if (way.OsmVer == 0 || osmWay.Attributes.Version>way.OsmVer)
                {
                    // Заполняем поля
                    way.OsmId = osmWay.Attributes.Id;
                    way.OsmVer = osmWay.Attributes.Version;
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
                    // Заполняем точки
                    way.Nodes.Clear();
                    foreach (OSM.Node osmNode in osmWay.Nodes)
                        way.Nodes.Add(NodeFromOsmNode(osmNode));
                    // Пересчитываем точки
                    way.Recalculate();
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

    }

}
