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
    public class routePoint : GeoPoint
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
        public List<routePoint> Points { get; private set; } = new List<routePoint>();
        
        /// <summary>
        /// Загружает маршрут из текстового представления заданного формата
        /// </summary>
        public void Read(RouteFormat routeFormat)
        {
            
        }
        
        public void Write()
        {

        }

        public void AddPoint (Double latitudeDeg, Double longitudeDeg, Boolean intermediate, String name)
        {
            routePoint point = new routePoint();
            point.LatitudeDeg = latitudeDeg;
            point.LongitudeDeg = longitudeDeg;
            point.Intermediate = intermediate;
            point.Name = name;
            Points.Add(point);
        }

        public String ToText()
        {
            return RouteWriter.ToNative(this);
        }

    }
}
