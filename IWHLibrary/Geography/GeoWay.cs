using System;
using System.Collections.Generic;

namespace Geography
{

    /// <summary>
    /// Представляет путь, заданный набором участков
    /// </summary>
    public class GeoWay
    {

        public List<GeoLeg> Legs { get; protected set; }

        public GeoWay()
        {
            Legs = new List<GeoLeg>();
        }

        /// <summary>
        /// Возвращает список всех точек пути
        /// </summary>
        /// <returns></returns>
        public List<GeoPoint> GetPoints()
        {
            var result = new List<GeoPoint>();
            if (Legs.Count>0)
            {
                result.Add(Legs[0].StartPoint);
                foreach (GeoLeg leg in Legs)
                {
                    result.Add(leg.EndPoint);
                }
            }
            return result;
        }

    }

}
