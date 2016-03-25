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

    public class Node
    {

        /// <summary>
        /// Геодезические координаты точки.
        /// </summary>
        public Coordinates Coordinates;

        /// <summary>
        /// Тип точки.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// Длина участка пути (удаление от предыдущей точки линии).
        /// </summary>
        public Distance PartLenght;

        /// <summary>
        /// Направление участка пути (азимут от предыдущей точки линии).
        /// </summary>
        public Angle PartDirection;

        /// <summary>
        /// Радиус окружности, при входе в которую точка считается посещенной.
        /// </summary>
        public Distance Range;

        /// <summary>
        /// Истина, исли точка была посещена.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения точки.
        /// </summary>
        public DateTime LastVisitedTime;

        public Int64 OsmId;

        public Int64 OsmVer;

    }

}
