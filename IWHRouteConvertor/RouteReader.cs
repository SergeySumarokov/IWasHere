using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class RouteReader
    {

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public static Route GetRouteFromString(String routeString)
        {
            RouteFormat routeFormat = DetermineRouteFormat(routeString);
            switch (routeFormat)
            {
                case RouteFormat.YandexURL: return GetFromYandexURL(routeString);
                default: return null;
            }
        }

        public static RouteFormat DetermineRouteFormat(String RouteString)
        {
            if (RouteString.ToLower().StartsWith("https://yandex.ru/maps/")) return RouteFormat.YandexURL;
            else return RouteFormat.Unknown;
        }

        public static Route GetFromYandexURL(String yandexURL)
        {
            // Проверяем начало строки на правильный URL
            yandexURL = yandexURL.ToLower();
            if (!yandexURL.StartsWith("https://yandex.ru/maps/")) return null;

            //Разбираем строку параметров и получаем словарь
            String paramString = yandexURL.Substring(yandexURL.IndexOf('?')+1);
            Dictionary<String, String> paramDict = Helper.GetDictFromString(paramString, '&', '=');
            
            // Разбираем строку координат точек
            String pointString = paramDict["rtext"].Replace("%2c", ",");
            List<String> pointList = pointString.Split('~').ToList();
            
            // Наполняем маршрут
            Route route = new Route();
            foreach (String listVal in pointList)
            {
                String[] pointVal = listVal.Split(',');
                route.AddPoint(Double.Parse(pointVal[0],xmlFormatProvider),Double.Parse(pointVal[1],xmlFormatProvider),"");
            }

            // Обозначаем промежуточные точки
            if (paramDict.ContainsKey("via"))
            {
                String viaString = paramDict["via"];
                List<String> viaList = viaString.Split('~').ToList();
                foreach (String viaVal in viaList)
                {
                    route.Points[Int32.Parse(viaVal)].Intermediate = true;
                }
            }

            return route;
        }

    }
}
