using System;
using System.Collections.Generic;
using Primitives;

namespace Geography
{

    /// <summary>
    /// Представляет географическую область как полигон, заданный списком точек.
    /// </summary>
    public class Area
    {

        // Минимальные и максимальные значения широты и долготы для оптимизации алгоритмов
        private Coordinates _minPoint;
        private Coordinates _maxPoint;

        /// <summary>
        /// Упорядоченный список точек, описывающий границы географической области.
        /// </summary>
        public List<Point> Points { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Area()
        {
            Points = new List<Point>();
        }

        /// <summary>
        /// Пересчитывает значения минимальных и максимальных широты и долготы
        /// </summary>
        public void Recalculate()
        {
            _minPoint = Points[0].Coordinates;
            _maxPoint = Points[0].Coordinates;
            foreach (Point point in Points)
            {
                if (point.Coordinates.Latitude < _minPoint.Latitude) { _minPoint.Latitude = point.Coordinates.Latitude; }
                if (point.Coordinates.Latitude > _maxPoint.Latitude) { _maxPoint.Latitude = point.Coordinates.Latitude; }
                if (point.Coordinates.Longitude < _minPoint.Longitude) { _minPoint.Longitude = point.Coordinates.Longitude; }
                if (point.Coordinates.Longitude > _maxPoint.Longitude) { _maxPoint.Longitude = point.Coordinates.Longitude; }
            }
        }

        #region "Расчеты c областями"

        /// <summary>
        /// Возвращает Истину если заданная точка находится внутри области или на ее границе.
        /// </summary>
        /// <param name="pointCoordinates"></param>
        /// <returns></returns>
        public bool HasPointInside(Point point)
        {
            if (point.Coordinates.Latitude < _minPoint.Latitude ||
                point.Coordinates.Latitude > _maxPoint.Latitude ||
                point.Coordinates.Longitude < _minPoint.Longitude ||
                point.Coordinates.Longitude > _maxPoint.Longitude)
            { return false; }
            else
            { return IsPointInArea(Points, point); }
        }

        /// <summary>
        /// Возвращает истину, если заданная точка находится внутри заданной области.
        /// </summary>
        /// <returns>Не будет работать, если область пересекается мередианом 180.</returns>
        public static bool IsPointInArea(List<Point> areaPoints, Point testPoint)
        {

            bool isInside = false;
            for (int i = 0, j = areaPoints.Count - 1; i < areaPoints.Count; j = i++)
            {
                if (((areaPoints[i].Coordinates.Latitude > testPoint.Coordinates.Latitude) != (areaPoints[j].Coordinates.Latitude > testPoint.Coordinates.Latitude)) &&
                    (testPoint.Coordinates.Longitude.Radians <
                        (areaPoints[j].Coordinates.Longitude.Radians - areaPoints[i].Coordinates.Longitude.Radians)
                        * (testPoint.Coordinates.Latitude.Radians - areaPoints[i].Coordinates.Latitude.Radians)
                        / (areaPoints[j].Coordinates.Latitude.Radians - areaPoints[i].Coordinates.Latitude.Radians)
                        + areaPoints[i].Coordinates.Longitude.Radians))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        #endregion

    }
}
