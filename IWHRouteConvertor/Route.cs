using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primitives;
using Geography;

namespace IWHRouteConvertor
{
    public enum RouteFormat : int
    {
        Unknown = 0,
        Native = 1,
        YandexURL = 2,
        GoogleURL = 3,
        RTE = 4,
        KML = 5
    }
    
    /// <summary>
    /// Представляет точку маршрута/
    /// </summary>
    public class RoutePoint : GeoPoint
    {
        /// <summary>
        /// Имя точки маршрута.
        /// </summary>
        public string Name;
        /// <summary>
        /// Значение Истина указывает на промежуточную точку маршрута
        /// </summary>
        public Boolean Intermediate;
    }

    /// <summary>
    /// Представляет маршрут, заданный набором промежуточных точек.
    /// </summary>
    public class Route
    {

        /// <summary>
        /// Упорядоченный список точек, описывающий маршрут.
        /// </summary>
        public List<RoutePoint> Points { get; private set; } = new List<RoutePoint>();
        
        /// <summary>
        /// Загружает маршрут из текстового представления заданного формата
        /// </summary>
        public void Read(RouteFormat routeFormat)
        {
            
        }
        
        public void Write()
        {

        }

        public void AddPoint (double LatitudeDeg, double LongitudeDeg, string Name)
        {
            RoutePoint point = new RoutePoint();
            point.LatitudeDeg = LatitudeDeg;
            point.LongitudeDeg = LongitudeDeg;
            point.Name = Name;
            Points.Add(point);
        }

        public string ToText()
        {
            string text = string.Empty;
            foreach (RoutePoint point in this.Points)
            {
                if (text.Length > 0) text += Environment.NewLine;
                text += string.Format("{0:f6},{1:f6},{2},{3}", point.LatitudeDeg, point.LongitudeDeg,point.Intermediate.GetHashCode(), point.Name);
            }
            return text;
        }

    }
}
