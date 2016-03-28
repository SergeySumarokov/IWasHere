using System;
using System.Xml;
using System.Xml.Serialization;
using Primitives;

namespace IWH
{

    public enum NodeType : int
    {
        Waypoint = 0,
        PopulatedPlace = 1,
        ShowPlace = 2
    }

    [System.Serializable, XmlType("node")]
    public class Node
    {

        /// <summary>
        /// Геодезические координаты точки.
        /// </summary>
        [XmlIgnore]
        public Coordinates Coordinates;

        [XmlAttribute("lat")]
        public double Lat
        {
            get { return System.Math.Round(Coordinates.Latitude.Degrees,6); }
            set { Coordinates.Latitude.Degrees = value; }
        }

        [XmlAttribute("lon")]
        public double Lon
        {
            get { return System.Math.Round(Coordinates.Longitude.Degrees,6); }
            set { Coordinates.Longitude.Degrees = value; }
        }

        /// <summary>
        /// Тип точки.
        /// </summary>
        [XmlAttribute("type")]
        public NodeType Type { get; set; }

        /// <summary>
        /// Длина участка пути (удаление от предыдущей точки линии).
        /// </summary>
        [XmlIgnore]
        public Distance PartLenght;

        /// <summary>
        /// Направление участка пути (азимут от предыдущей точки линии).
        /// </summary>
        [XmlIgnore]
        public Angle PartDirection;

        /// <summary>
        /// Радиус окружности, при входе в которую точка считается посещенной.
        /// </summary>
        [XmlIgnore]
        public Distance Range;

        /// <summary>
        /// Истина, исли точка была посещена.
        /// </summary>
        [XmlAttribute("visited")]
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения точки.
        /// </summary>
        [XmlAttribute("last")]
        public DateTime LastVisitedTime;

        /// <summary>
        /// Идентификатор OSM.
        /// </summary>
        [XmlAttribute("id")]
        public Int64 OsmId;

        /// <summary>
        /// Версия данных OSM.
        /// </summary>
        [XmlAttribute("ver")]
        public Int64 OsmVer;

    }

}
