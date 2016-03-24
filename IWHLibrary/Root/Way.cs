using System;
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

    public class Way :OSM.Way
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

        public void Recalculate()
        {
            foreach (var n in Nodes)
            {

            }
        }

    }

}
