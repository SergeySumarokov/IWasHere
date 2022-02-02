using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class RouteReader
    {

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public static Route FromText(string routeString)
        {
            RouteFormat routeFormat = DetermineRouteFormat(routeString);
            switch (routeFormat)
            {
                case RouteFormat.Native: return FromNative(routeString);
                case RouteFormat.YandexURL: return FromYandexURL(routeString);
                default: return null;
            }
        }

        private static RouteFormat DetermineRouteFormat(string routeString)
        {
            if (routeString.ToLower().StartsWith("https://yandex.ru/maps/")) return RouteFormat.YandexURL;
            else return RouteFormat.Unknown;
        }

        public static Route FromNative(string routeString)
        {
            Route result = new Route();
            string[] routeLines = routeString.Split(new string[] { System.Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
            if (routeLines.Length == 0)
                return null;
            foreach (string routeLine in routeLines)
            {
                string[] pointParams = routeLine.Split(',');
                if (pointParams.Length == 4)
                    result.AddPoint(
                        double.Parse(pointParams[0], xmlFormatProvider),
                        double.Parse(pointParams[1], xmlFormatProvider),
                        int.Parse(pointParams[2]) == 1,
                        pointParams[3].Trim()
                        );
                else
                    return null;
            }
            return result;
        }

        public static Route FromYandexURL(string yandexURL)
        {
            // Проверяем начало строки на правильный URL
            yandexURL = yandexURL.ToLower();
            if (!yandexURL.StartsWith("https://yandex.ru/maps/")) return null;

            //Разбираем строку параметров и получаем словарь
            string paramString = yandexURL.Substring(yandexURL.IndexOf('?')+1);
            Dictionary<string, string> paramDict = Helper.GetDictFromString(paramString, '&', '=');

            // Разбираем строку координат точек
            string pointString = paramDict["rtext"].Replace("%2c", ",");
            List<string> pointList = pointString.Split('~').ToList();
            
            // Наполняем маршрут
            Route route = new Route();
            foreach (string listVal in pointList)
            {
                string[] pointVal = listVal.Split(',');
                route.AddPoint(double.Parse(pointVal[0],xmlFormatProvider), double.Parse(pointVal[1],xmlFormatProvider),false,"");
            }

            // Обозначаем промежуточные точки
            if (paramDict.ContainsKey("via"))
            {
                string viaString = paramDict["via"];
                List<string> viaList = viaString.Split('~').ToList();
                foreach (string viaVal in viaList)
                {
                    route.Points[int.Parse(viaVal)].Intermediate = true;
                }
            }

            return route;
        }

    }
}
