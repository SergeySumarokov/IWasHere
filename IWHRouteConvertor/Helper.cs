using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWHRouteConvertor
{
    class Helper
    {

        public static List<String> GetListFromString(String sourceString, Char itemSeparator)
        {
            List<String> result = sourceString.Split(itemSeparator).ToList();
            return result;
        }

        public static Dictionary<String,String> GetDictFromString(String sourceString, Char itemSeparator, Char KeyValueSeparator)
        {
            List<String> sourceList = GetListFromString(sourceString, itemSeparator);
            var result = new Dictionary<String, String>();
            foreach (String listVal in sourceList)
            {
                String[] dictVal = listVal.Split(KeyValueSeparator);
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
            route.AddPoint(60, 30, "Первый");
            route.AddPoint(61, 29, "Второй");
            return route;
        }

        public static String GetDebugRouteString(RouteFormat routeFormat)
        {
            String routeString = String.Empty;
            
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
