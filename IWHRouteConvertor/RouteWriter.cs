using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class RouteWriter
    {

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public static String PutToRTE(Route route)
        {
            String result = String.Join(Environment.NewLine, new String[] {"OziExplorer Route File Version 1.0","WGS 84","Reserved 1","Reserved 2","","R,0,,,"});
            foreach (RoutePoint routePoint in route.Points)
            {
                var pointString = new String[] { "W", "0", "", "", "", "", "" };
                pointString[4] = routePoint.Name;
                pointString[5] = String.Format(xmlFormatProvider, "{0:f6}", routePoint.LatitudeDeg);
                pointString[6] = String.Format(xmlFormatProvider, "{0:f6}", routePoint.LongitudeDeg);
                result += Environment.NewLine + String.Join(",", pointString);
            }
            return result;
        }
    
    }
}
