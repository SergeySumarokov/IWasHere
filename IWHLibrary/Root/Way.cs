using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Primitives;

namespace IWH
{

    public enum WayType : int
    {
        Unknown = 0,
        Motorway = 1,
        Trunk = 2,
        Primary = 3,
        Secondary = 4
    }

    [System.Serializable, XmlType("way")]
    public class Way
    {

        /// <summary>
        /// Тип линии.
        /// </summary>
        [XmlAttribute("type")]
        public WayType Type { get; set; }

        /// <summary>
        /// Наименование линии.
        /// </summary>
        [XmlAttribute("name")]
        public String Name { get; set; }

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
        /// Истина, исли вся линия была посещена.
        /// </summary>
        [XmlIgnore]
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения любой из точек линии.
        /// </summary>
        [XmlIgnore]
        public DateTime LastVisitedTime;

        /// <summary>
        /// Оrdered list of nodes.
        /// </summary>
        /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist.</remarks>
        [XmlElement("node")]
        public List<Node> Nodes { get; private set; }

        [XmlAttribute("id")]
        public Int64 OsmId;

        [XmlAttribute("ver")]
        public Int64 OsmVer;

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Way()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Выполняет пересчет параметров линии.
        /// </summary>
        public void Recalculate()
        {
            Lenght = Distance.Zero;
            VisitedLenght = Distance.Zero;
            for (int i = 1; i <= Nodes.Count-1; i++)
            {
                Nodes[i].PartLenght = Nodes[i-1].Coordinates.OrthodromicDistance(Nodes[i].Coordinates);
                Nodes[i].PartDirection = Nodes[i-1].Coordinates.OrthodromicBearing(Nodes[i].Coordinates);
                Lenght += Nodes[i].PartLenght;
                if (Nodes[i].IsVisited && Nodes[i-1].IsVisited)
                    VisitedLenght += Nodes[i].PartLenght;
                if (Nodes[i].LastVisitedTime > LastVisitedTime)
                    LastVisitedTime = Nodes[i].LastVisitedTime;
            }
            IsVisited = Lenght.AlmostEquals(VisitedLenght);
        }

    }

}
