using System.Collections.Generic;
using Geography;

namespace IWHRouteConvertor
{
 
    /// <summary>
    /// Указывает формат представления маршрута.
    /// </summary>
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
    /// Представляет точку маршрута.
    /// </summary>
    public class RoutePoint : GeoPoint
    {
        
        /// <summary>
        /// Наименование точки маршрута.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Значение true указывает на промежуточную точку маршрута.
        /// </summary>
        public bool Intermediate;
    
    }

    /// <summary>
    /// Представляет маршрут, определённый упорядоченным списком точек.
    /// </summary>
    public class Route
    {
        
        /// <summary>
        /// Представляет упорядоченный список точек, определяющих маршрут.
        /// </summary>
        public List<RoutePoint> Points { get; private set; } = new List<RoutePoint>();

        /// <summary>
        /// Добавляет точку в конец списка.
        /// </summary>
        public void AddPoint(double latitudeDeg, double longitudeDeg, bool intermediate, string name)
        {
            var point = new RoutePoint
            {
                LatitudeDeg = latitudeDeg,
                LongitudeDeg = longitudeDeg,
                Intermediate = intermediate,
                Name = name
            };
            Points.Add(point);
        }

    }
}
