using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    /// <summary>
    /// Предоставляет методы для формирования текстового представления маршрута.
    /// </summary>
    static class RouteWriter
    {

        private static readonly IFormatProvider textFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        /// <summary>
        /// Формирует текстовое представление маршрута в заданном формате.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static string ToText(Route route, RouteFormat routeFormat)
        {
            switch (routeFormat)
            {
                case RouteFormat.Native:
                    return ToNative(route);
                case RouteFormat.YandexURL:
                    return ToYandexURL(route);
                case RouteFormat.GoogleURL:
                    return ToGoogleURL(route);
                case RouteFormat.RTE:
                    return ToRTE(route);
                default:
                    return null;
            }

        }

        /// <summary>
        /// Формирует текстовое представление маршрута в собственном формате.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static string ToNative(Route route)
        {
            string result = string.Empty;
            foreach (RoutePoint point in route.Points)
            {
                if (result.Length > 0)
                    result += Environment.NewLine;
                result += string.Format(textFormatProvider,
                                        "{0:f6},{1:f6},{2},{3}",
                                        point.LatitudeDeg,
                                        point.LongitudeDeg,
                                        point.Intermediate ? 1 : 0,
                                        point.Name);
            }
            return result;
        }

        /// <summary>
        /// Формирует текстовое представление маршрута в формате URL карт Яндекса.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static string ToYandexURL(Route route)
        {
            
            string result = "https://yandex.ru/maps/?mode=routes";

            // Добавляем координаты точек маршрута
            var rtextParam = new List<string>();
            foreach (RoutePoint point in route.Points)
            {
                rtextParam.Add(string.Format(textFormatProvider, "{0:f6}%2C{1:f6}", point.LatitudeDeg, point.LongitudeDeg));
            }
            result += "&rtext=" + string.Join("~", rtextParam) + "&rtt=auto";
            
            // Добавляем номера промежуточных точек
            var viaParam = new List<string>();
            for (int i = 0; i < route.Points.Count; i++)
            {
                if (route.Points[i].Intermediate)
                    viaParam.Add(string.Format("{0}", i));
            }
            if (viaParam.Count > 0)
                result += "&via=" + string.Join("~", viaParam);
            
            // Возвращаем результат
            return result;

        }

        /// <summary>
        /// Формирует текстовое представление маршрута в формате URL карт Google.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static string ToGoogleURL(Route route)
        {
            string result = "https://www.google.ru/maps/dir/";
            var mainPoints = new List<string>();
            var intrPoints = new List<string>();
            // Добавляем раздельно координаты основных и вспомогательныхточек
            foreach (RoutePoint point in route.Points)
            {
                if (point.Intermediate)
                    intrPoints.Add(string.Format(textFormatProvider, "!1d{0:f7}!2d{1:f7}", point.LongitudeDeg, point.LatitudeDeg));
                else
                    mainPoints.Add(string.Format(textFormatProvider, "{0:f7},{1:f7}", point.LatitudeDeg, point.LongitudeDeg));
            }
            result += string.Join("/", mainPoints);
            if (intrPoints.Count > 0)
                // ??? а вот это пока не даёт результата
                result += "/data=!4m9!4m8!1m5!3m4!1m2" + string.Join("", intrPoints);
            // Возвращаем результат
            return result;
        }

        /// <summary>
        /// Формирует текстовое представление маршрута в RTE (маршрут OziExplorer).
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static string ToRTE(Route route)
        {
            string result = string.Join(Environment.NewLine, new string[] { "OziExplorer Route File Version 1.0", "WGS 84", "Reserved 1", "Reserved 2", "", "R,0,,," });
            foreach (RoutePoint point in route.Points)
            {
                string[] pointString = new string[] { "W", "0", "", "", "", "", "" };
                pointString[4] = point.Name;
                pointString[5] = string.Format(textFormatProvider, "{0:f6}", point.LatitudeDeg);
                pointString[6] = string.Format(textFormatProvider, "{0:f6}", point.LongitudeDeg);
                result += Environment.NewLine + string.Join(",", pointString);
            }
            return result;
        }
    }
}
