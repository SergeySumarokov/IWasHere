using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primitives;

namespace IWH
{

    public enum NodeType : int
    {
        Waypoint = 0,
        PopulatedPlace = 1,
        ShowPlace = 2
    }

    public class Node : OSM.Node
    {

        public NodeType Type { get; set; }

        /// <summary>
        /// Радиус окуржности, при входе в которую точка считается посещенной.
        /// </summary>
        public Distance Range;

        /// <summary>
        /// Длина участка пути (удаление от предыдущей точки линии).
        /// </summary>
        public Distance PartLenght;

        /// <summary>
        /// Направление участка пути (азимут от предыдущей точки линии).
        /// </summary>
        public Angle PartDirection;

        /// <summary>
        /// Истина, исли точка была посещена.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения точки.
        /// </summary>
        public DateTime LastVisitedTime;

    }

}
