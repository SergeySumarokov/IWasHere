using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class Helper
    {

        public static List<string> GetListFromString(string sourceString, char itemSeparator)
        {
            List<string> result = sourceString.Split(itemSeparator).ToList();
            return result;
        }

        public static Dictionary<string, string> GetDictFromString(string sourceString, char itemSeparator, char KeyValueSeparator)
        {
            List<string> sourceList = GetListFromString(sourceString, itemSeparator);
            var result = new Dictionary<string, string>();
            foreach (string listVal in sourceList)
            {
                string[] dictVal = listVal.Split(KeyValueSeparator);
                if (dictVal.Count() == 2)
                {
                    result.Add(dictVal[0], dictVal[1]);
                }
            }
            return result;
        }

        public static Route GetDebugRoute()
        {
            Route route = new Route();
            route.AddPoint(60, 30, false, "Первый");
            route.AddPoint(61, 29, false, "Второй");
            return route;
        }

        public static string GetDebugRouteString(RouteFormat routeFormat)
        {
            string routeString = string.Empty;
            
            switch (routeFormat)
            {
                case RouteFormat.YandexURL:
                    {
                        routeString = "https://yandex.ru/maps/2/saint-petersburg/?ll=30.315635%2C59.938951&mode=routes&rtext=60.010537%2C30.136083~60.058071%2C30.160828~60.057759%2C30.217632~60.035281%2C30.295385&rtt=auto&ruri=~~~&via=1~2&z=11";
                        break;
                    }
                    

            }
            return routeString;
        }


    }
}
