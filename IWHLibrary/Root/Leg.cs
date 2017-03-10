﻿using System;
using Primitives;

namespace IWH

{

    /// <summary>
    /// Участок пути, заданный двумя точками с рассчитанным направлением пути и расстоянием.
    /// </summary>
    public class Leg : Geography.Leg
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
        /// Направление участка
        /// </summary>
        //public Angle Direction;

        /// <summary>
        /// Протяженность участка
        /// </summary>
        //public Distance Lenght;

        /// <summary>
        /// Ссылка на путь, которому принадлежит участок
        /// </summary>
        public Way Way;

        /// <summary>
        /// Порядковый номер участка в пути
        /// </summary>
        //public Int32 Number;

        /// <summary>
        /// Скорость движения на участке
        /// </summary>

        public Speed Speed;
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

    }

}
