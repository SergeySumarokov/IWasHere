using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    /// <summary>
    /// Предоставляет методы для получения маршрута из текстового представления.
    /// </summary>
    static class RouteReader
    {

        private static readonly IFormatProvider TextFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        /// <summary>
        /// Получает маршрут из текста, самостоятельно определяя формат по содержимому.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static Route FromText(string routeText)
        {
            RouteFormat routeFormat = DetermineRouteFormat(routeText);
            switch (routeFormat)
            {
                case RouteFormat.Native:
                    return FromNative(routeText);
                case RouteFormat.YandexURL:
                    return FromYandexURL(routeText);
                default:
                    return null;
            }
        }

        private static RouteFormat DetermineRouteFormat(string routeString)
        {
            if (routeString.ToLower().StartsWith("https://yandex.ru/maps/"))
                return RouteFormat.YandexURL;
            else
                return RouteFormat.Unknown;
        }

        /// <summary>
        /// Получает маршрут из текста в собственном формате.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static Route FromNative(string routeText)
        {
            var result = new Route();
            string[] textLines = routeText.Split(new string[] { System.Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
            if (textLines.Length == 0)
                return null;
            // В каждой строке должно быть 4 значение с разделителем запятая
            foreach (string textLine in textLines)
            {
                string[] pointProps = textLine.Split(',');
                if (pointProps.Length == 4)
                    result.AddPoint(double.Parse(pointProps[0], TextFormatProvider),
                                    double.Parse(pointProps[1], TextFormatProvider),
                                    int.Parse(pointProps[2]) == 1,
                                    pointProps[3].Trim());
                else
                    return null;
            }
            return result;
        }

        /// <summary>
        /// Получает маршрут из текста URL карт Яндекса.
        /// </summary>
        /// <returns>Возвращает null при ошибке.</returns>
        public static Route FromYandexURL(string yandexURL)
        {
            
            // Проверяем начало строки на правильный URL
            yandexURL = yandexURL.ToLower();
            if (!yandexURL.StartsWith("https://yandex.ru/maps/"))
                return null;

            //Разбираем строку параметров и получаем словарь
            string paramString = yandexURL.Substring(yandexURL.IndexOf('?') + 1);
            Dictionary<string, string> paramDict = Helper.GetDictFromString(paramString, '&', '=');

            // Наполняем маршрут
            Route result = new Route();
            string[] pointTexts = paramDict["rtext"].Replace("%2c", ",").Split('~');
            foreach (string pointText in pointTexts)
            {
                string[] pointProps = pointText.Split(',');
                if (pointProps.Length == 2)
                    result.AddPoint(double.Parse(pointProps[0], TextFormatProvider),
                                    double.Parse(pointProps[1], TextFormatProvider),
                                    false,
                                    "");
                else
                    return null;
            }

            // Обозначаем промежуточные точки
            if (paramDict.ContainsKey("via"))
            {
                string[] pointIndexes = paramDict["via"].Split('~');
                foreach (string pointIndex in pointIndexes)
                {
                    result.Points[int.Parse(pointIndex)].Intermediate = true;
                }
            }

            return result;
        
        }

    }
}
