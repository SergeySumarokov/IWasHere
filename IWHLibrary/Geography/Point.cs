using System;

namespace Geography
{

    /// <summary>
    /// Представляет точку, заданную географическими координатами
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Географические координаты точки
        /// </summary>
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Point() { }

        /// <summary>
        /// Инициализирует новый экземпляр заданными координатами.
        /// </summary>
        public Point(Coordinates coordinates)
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Инициализирует новый экземпляр координатами с заданными значениями широты и долготы, выраженными в градусах.
        /// </summary>
        /// <param name="latitudeDeg">Широта (гр)</param>
        /// <param name="longitudeDeg">Долгота (гр)</param>
        /// <param name="altitudeMt">Высота (м)</param>
        public Point(double latitudeDeg, double longitudeDeg, double altitudeMt)
        {
            Coordinates = new Coordinates(latitudeDeg, longitudeDeg, altitudeMt);
        }

    }

}
