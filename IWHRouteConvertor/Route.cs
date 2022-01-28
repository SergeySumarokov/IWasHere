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
        Native = 0,
        YandexURL = 1,
        GoogleURL = 2,
        RTE = 3,
        KML = 4
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
        public Boolean Hidden;
    }

    /// <summary>
    /// Представляет маршрут, заданный набором промежуточных точек.
    /// </summary>
    public class Route
    {
       
        /// <summary>
        /// Упорядоченный список точек, описывающий маршрут.
        /// </summary>
        public List<RoutePoint> Points { get; private set; }
        
        /// <summary>
        /// Загружает маршрут из текстового представления заданного формата
        /// </summary>
        public void Read(RouteFormat routeFormat)
        {
            
        }
        
        public void Write()
        {

        }

    }
}
