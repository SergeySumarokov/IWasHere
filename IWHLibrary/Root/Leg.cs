using System;
using Primitives;

namespace IWH

{

    /// <summary>
    /// Участок пути, заданный двумя точками с рассчитанным направлением пути и расстоянием.
    /// </summary>
    public class Leg : Geography.GeoLeg
    {

        /// <summary>
        /// Начальная точка участка
        /// </summary>
        public new Node StartPoint
        {
            get { return (Node)base.StartPoint; }
            set { base.StartPoint = value; }
        }

        /// <summary>
        /// Конечная точка участка
        /// </summary>
        public new Node EndPoint
        {
            get { return (Node)base.EndPoint; }
            set { base.EndPoint = value; }
        }

        /// <summary>
        /// Ссылка на путь, которому принадлежит участок
        /// </summary>
        public Way Way;

        /// <summary>
        /// Скорость движения на участке
        /// </summary>
        public Speed Speed;

        /// <summary>
        /// Истина, исли участок был посещён.
        /// </summary>
        public bool IsVisited;

        /// <summary>
        /// Количество посещений участка
        /// </summary>
        public int VisitedCount;

        /// <summary>
        /// Время последнего посещения участка.
        /// </summary>
        public DateTime LastVisitedTime;

    }

}
