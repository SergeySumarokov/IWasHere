using System;
using System.Collections.Generic;

namespace Geography
{

    /// <summary>
    /// Представляет путь, заданный набором участков
    /// </summary>
    public class Way
    {

        public List<Leg> Legs { get; private set; }


        public Way()
        {
            Legs = new List<Leg>();
        }

        /// <summary>
        /// Возвращает список всех точек пути
        /// </summary>
        /// <returns></returns>
        public List<Point> GetPoints()
        {
            var result = new List<Point>();
            if (Legs.Count>0)
            {
                result.Add(Legs[0].StartPoint);
                foreach (Leg leg in Legs)
                {
                    result.Add(leg.EndPoint);
                }
            }
            return result;
        }

    }

}
