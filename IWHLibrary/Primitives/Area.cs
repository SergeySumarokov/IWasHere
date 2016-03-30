using System;
using System.Collections.Generic;

namespace Primitives
{
    /// <summary>
    /// Представляет географическую область как полигон, заданный списком геодезических координат.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Упорядоченный список точек, описывающий границы области.
        /// </summary>
        public List<Coordinates> Points { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Area()
        {
            Points = new List<Coordinates>();
        }

        /// <summary>
        /// Возвращает Истину если заданная точка находится внутри области или на ее границе.
        /// </summary>
        /// <param name="pointCoordinates"></param>
        /// <returns></returns>
        public bool HasPointInside(Coordinates pointCoordinates)
        {
            return IsPointInArea(Points, pointCoordinates);
        }

        #region "Расчеты c областями"

        /// <summary>
        /// Возвращает истину, если точка находится внутри области, заданной массивом точек.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="point"></param>
        /// <returns>Не будет работать, если область пересекается меридином 180.</returns>
        public static bool IsPointInArea(List<Coordinates> area, Coordinates point)
        {

            bool isInside = false;
            for (int i = 0, j = area.Count - 1; i < area.Count; j = i++)
            {
                if (((area[i].Latitude > point.Latitude) != (area[j].Latitude > point.Latitude)) &&
                    (point.Longitude.Radians <
                        (area[j].Longitude.Radians - area[i].Longitude.Radians)
                        * (point.Latitude.Radians - area[i].Latitude.Radians)
                        / (area[j].Latitude.Radians - area[i].Latitude.Radians)
                        + area[i].Longitude.Radians))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        //public static bool IsPointInPolygon4(PointF[] polygon, PointF testPoint)
        //{
        //    bool result = false;
        //    int j = polygon.Count() - 1;
        //    for (int i = 0; i < polygon.Count(); i++)
        //    {
        //        if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
        //        {
        //            if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
        //            {
        //                result = !result;
        //            }
        //        }
        //        j = i;
        //    }
        //    return result;
        //}

        #endregion

    }
}
