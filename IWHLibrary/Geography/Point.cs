using Primitives;

namespace Geography
{

    /// <summary>
    /// Представляет точку, заданную географическими координатами.
    /// </summary>
    public class Point
    {

        #region "Свойства"

        /// <summary>
        /// Географические координаты точки.
        /// </summary>
        public Coordinates Coordinates;

        /// <summary>
        ///  Географическая широта точки.
        /// </summary>
        public Angle Latitude
        {
            get { return Coordinates.Latitude; }
            set { Coordinates.Latitude = value; }
        }

        /// <summary>
        ///  Географическая долгота точки.
        /// </summary>
        public Angle Longitude
        {
            get { return Coordinates.Longitude; }
            set { Coordinates.Longitude = value; }
        }

        /// <summary>
        /// Высота точки.
        /// </summary>
        public Altitude Altitude
        {
            get { return Coordinates.Altitude; }
            set { Coordinates.Altitude = value; }
        }

        /// <summary>
        ///  Географическая широта точки в градусах.
        /// </summary>
        public double LatitudeDeg
        {
            get { return Coordinates.Latitude.Degrees; }
            set { Coordinates.Latitude.Degrees = value; }
        }

        /// <summary>
        ///  Географическая долгота точки в градусах.
        /// </summary>
        public double LongitudeDeg
        {
            get { return Coordinates.Longitude.Degrees; }
            set { Coordinates.Longitude.Degrees = value; }
        }

        /// <summary>
        ///  Высота точки в метрах.
        /// </summary>
        public double AltitudeMt
        {
            get { return Coordinates.Altitude.Meters; }
            set { Coordinates.Altitude.Meters = value; }
        }

        #endregion

        #region "Конструкторы"

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

        #endregion

    }

}
