﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class RouteWriter
    {

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

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

        public static String ToRTE(Route route)
        {
            String result = String.Join(Environment.NewLine, new String[] {"OziExplorer Route File Version 1.0","WGS 84","Reserved 1","Reserved 2","","R,0,,,"});
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
    
        public static String ToYandexURL(Route route)
        {
            string result = "https://yandex.ru/maps/?mode=routes";
            var rtextParam = new List<string>();
            // Добавляем оординаты точек маршрута
            foreach (routePoint point in route.Points)
            {
                rtextParam.Add(String.Format(xmlFormatProvider, "{0:f6}%2C{1:f6}", point.LatitudeDeg, point.LongitudeDeg));
            }
            result += "&rtext=" + String.Join("~", rtextParam) + "&rtt=auto";
            // Добавляем номера промежуточных точек
            var viaParam = new List<string>();
            for (int i = 0; i < route.Points.Count; i++)
            {
                if (route.Points[i].Intermediate) viaParam.Add(String.Format("{0}", i));
            }
            if (viaParam.Count > 0)
            {
                result += "&via=" + String.Join("~", viaParam);
            }
            // Возвращаем результат
            return result;
        }

    }
}
