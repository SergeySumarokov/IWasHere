using System;
using System.Collections.Generic;
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

    public class Way
    {

        /// <summary>
        /// Тип линии.
        /// </summary>
        public WayType Type { get; set; }

        /// <summary>
        /// Общая протяженность линии.
        /// </summary>
        public Distance Lenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков линии.
        /// </summary>
        public Distance VisitedLenght;

        /// <summary>
        /// Истина, исли вся линия была посещена.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения любой из точек линии.
        /// </summary>
        public DateTime LastVisitedTime;

        /// <summary>
        /// Оrdered list of nodes.
        /// </summary>
        /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist.</remarks>
        public List<Node> Nodes { get; private set; }

        public Int64 OsmId;

        public Int64 OsmVer;

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Way()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Выполняет пересчет длины линии
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
            }
            IsVisited = Lenght.AlmostEquals(VisitedLenght);
        }

    }

}
