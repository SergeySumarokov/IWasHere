using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Primitives;

namespace IWH

{
    /// <summary>
    /// Участок пути, заданный двумя точками с рассчитанным направлением пути и расстоянием.
    /// </summary>
    public class Leg
    {
        public Node StartNode;
        public Node EndNode;
        public Angle Direction;
        public Distance Lenght;
        public Boolean OneWay;
        
        /// <summary>
        /// Истина, исли участок был посещён.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Количество посещений участка
        /// </summary>
        public Int32 VisitedCount;

        /// <summary>
        /// Время последнего посещения участка.
        /// </summary>
        public DateTime LastVisitedTime;

        public Int64 WayId;
    }
}
