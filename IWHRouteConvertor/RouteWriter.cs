using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class RouteWriter
    {

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public static String ToText(Route route, RouteFormat routeFormat)
        {
            {
                switch (routeFormat)
                {
                    case RouteFormat.Native: return ToNative(route);
                    case RouteFormat.YandexURL: return ToYandexURL(route);
                    case RouteFormat.GoogleURL: return ToGoogleURL(route);
                    case RouteFormat.RTE: return ToRTE(route);
                    default: return null;
                }
            }

        }

        public static String ToNative(Route route)
        {
            string result = string.Empty;
            foreach (routePoint point in route.Points)
            {
                if (result.Length > 0) result += Environment.NewLine;
                result += string.Format(xmlFormatProvider,"{0:f6},{1:f6},{2},{3}", point.LatitudeDeg, point.LongitudeDeg, point.Intermediate.GetHashCode(), point.Name);
            }
            return result;
        }

        public static String ToYandexURL(Route route)
        {
            string result = "https://yandex.ru/maps/?mode=routes";
            var rtextParam = new List<string>();
            // Добавляем координаты точек маршрута
            foreach (routePoint point in route.Points)
            {
                rtextParam.Add(String.Format(xmlFormatProvider, "{0:f6}%2C{1:f6}", point.LatitudeDeg, point.LongitudeDeg));
            }
            result += "&rtext=" + String.Join("~", rtextParam) + "&rtt=auto";
            // Добавляем номера промежуточных точек
            var viaParam = new List<string>();
            for (int i = 0; i < route.Points.Count; i++)
            {
                if (route.Points[i].Intermediate)
                    viaParam.Add(String.Format("{0}", i));
            }
            if (viaParam.Count > 0)
                result += "&via=" + String.Join("~", viaParam);
            // Возвращаем результат
            return result;
        }

        public static String ToGoogleURL(Route route)
        {
            string result = "https://www.google.ru/maps/dir/";
            var mainPoints = new List<string>();
            var intrPoints = new List<string>();
            // Добавляем раздельно координаты основных и вспомогательныхточек
            foreach (routePoint point in route.Points)
            {
                if (point.Intermediate)
                    intrPoints.Add(String.Format(xmlFormatProvider, "!1d{0:f7}!2d{1:f7}", point.LongitudeDeg, point.LatitudeDeg));
                else
                    mainPoints.Add(String.Format(xmlFormatProvider, "{0:f7},{1:f7}", point.LatitudeDeg, point.LongitudeDeg));
            }
            result += String.Join("/", mainPoints);
            if (intrPoints.Count > 0)
                result += "/data=!4m9!4m8!1m5!3m4!1m2" + String.Join("", intrPoints);
            // Возвращаем результат
            return result;
        }

        public static String ToRTE(Route route)
        {
            String result = String.Join(Environment.NewLine, new String[] { "OziExplorer Route File Version 1.0", "WGS 84", "Reserved 1", "Reserved 2", "", "R,0,,," });
            foreach (routePoint point in route.Points)
            {
                var pointString = new String[] { "W", "0", "", "", "", "", "" };
                pointString[4] = point.Name;
                pointString[5] = String.Format(xmlFormatProvider, "{0:f6}", point.LatitudeDeg);
                pointString[6] = String.Format(xmlFormatProvider, "{0:f6}", point.LongitudeDeg);
                result += Environment.NewLine + String.Join(",", pointString);
            }
            return result;
        }
    }
}
