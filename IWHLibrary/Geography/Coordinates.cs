using System;
using Primitives;

namespace Geography
{

    /// <summary>
    /// Геодезические координаты.
    /// </summary>
    public struct Coordinates : IEquatable<Coordinates>
    {

        /// <summary>
        /// Широта.
        /// </summary>
        public Angle Latitude;

        /// <summary>
        /// Долгота.
        /// </summary>
        public Angle Longitude;

        /// <summary>
        /// Высота.
        /// </summary>
        /// И пусть по ГОСТ 22268-76 положено использовать Height, но в авиации именно Altitude означает высоту над уровнем моря.
        public Altitude Altitude;
        
        /// <summary>
        /// Инициализирует структуру значениями широты, долготы и высоты.
        /// </summary>
        /// <param name="latitude">Широта</param>
        /// <param name="longitude">Долгота</param>
        /// <param name="altitude">Высота</param>
        /// <remarks></remarks>
        public Coordinates(Angle latitude, Angle longitude, Altitude altitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
        }

        /// <summary>
        /// Инициализирует структуру значениями широты и долготы, выраженными в градусах.
        /// </summary>
        /// <param name="latitudeDeg">Широта (гр)</param>
        /// <param name="longitudeDeg">Долгота (гр)</param>
        /// <param name="altitudeMt">Высота (м)</param>
        public Coordinates(double latitudeDeg, double longitudeDeg, double altitudeMt)
        {
            this.Latitude = Angle.FromDegrees(latitudeDeg);
            this.Longitude = Angle.FromDegrees(longitudeDeg);
            this.Altitude = Altitude.FromMeters(altitudeMt);
        }

        /// <summary>
        /// Возвращает Истину, если значение структуры не задано.
        /// </summary>
        /// <returns>Истина, если все поля структуры имеют значения по умолчанию.</returns>
        public bool IsEmpty
        {
            get { return (this.Latitude.IsEmpty && this.Longitude.IsEmpty && this.Altitude.IsEmpty); }
        }

        #region "Операторы"

        public static bool operator ==(Coordinates coordinates1, Coordinates coordinates2)
        {
            return coordinates1.Latitude == coordinates2.Latitude && coordinates1.Longitude == coordinates2.Longitude && coordinates1.Altitude == coordinates2.Altitude;
        }

        public static bool operator !=(Coordinates coordinates1, Coordinates coordinates2)
        {
            return coordinates1.Latitude != coordinates2.Latitude || coordinates1.Longitude != coordinates2.Longitude || coordinates1.Altitude != coordinates2.Altitude;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        /// <summary>
        /// Определяет, равны ли текущие координаты заданным.
        /// </summary>
        /// <param name="coordinates">Заданная координаты</param>
        /// <returns>Истина, если заданные координаты равны текущим</returns>
        public bool Equals(Coordinates coordinates)
        {
            return this.Latitude.Equals(coordinates.Latitude) && this.Longitude.Equals(coordinates.Longitude) && this.Altitude.Equals(coordinates.Altitude);
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinates)
            {
                return this.Equals((Coordinates)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}{1} {2}{3} {4}m", (this.Latitude.IsNegative ? "S" : "N"), Math.Abs(this.Latitude.Degrees), (this.Longitude.IsNegative ? "W" : "E"), Math.Abs(this.Longitude.Degrees), this.Altitude.Meters);
        }

        public override int GetHashCode()
        {
            return this.Longitude.GetHashCode() ^ this.Longitude.GetHashCode() ^ this.Altitude.GetHashCode();
        }

        #endregion

        #region "Расчеты для проекции Меркатора"

        /// <summary>
        /// Возвращает прямой пеленг заданной точки для проекции Меркатора.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Angle MercatorBearing(Coordinates coordinates)
        {
            return GeodesyCalculator.MercatorBearing(this, coordinates);
        }

        /// <summary>
        /// Возвращает удаление заданной точки для проекции Меркатора.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Distance MercatorDistance(Coordinates coordinates)
        {
            return GeodesyCalculator.MercatorDistance(this, coordinates);
        }

        #endregion

        #region "Расчеты по ортодромии"

        /// <summary>
        /// Возвращает прямой пеленг заданной точки по ортодромии.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Angle OrthodromicBearing(Coordinates coordinates)
        {
            return GeodesyCalculator.OrthodromicBearing(this, coordinates);
        }

        /// <summary>
        /// Возвращает удаление заданной точки по ортодромии.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Distance OrthodromicDistance(Coordinates coordinates)
        {
            return GeodesyCalculator.OrthodromicDistance(this, coordinates);
        }

        /// <summary>
        /// Возвращает конечную точку, заданную пеленгом и удалением по ортодромии. 
        /// </summary>
        /// <param name="bearing">Пеленг</param>
        /// <param name="distance">Удаление</param>
        public Coordinates OrthodromicDestination(Angle bearing, Distance distance)
        {
            return GeodesyCalculator.OrthodromicDestination(this, bearing, distance);
        }

        #endregion

        #region "Расчеты по локсодромии"

        /// <summary>
        /// Возвращает прямой пеленг заданной точки по локсодромии.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Angle LoxodromicBearing(Coordinates coordinates)
        {
            return GeodesyCalculator.LoxodromicBearing(this, coordinates);
        }

        /// <summary>
        /// Возвращает удаление заданной точки по локсодромии.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Distance LoxodromicDistance(Coordinates coordinates)
        {
            return GeodesyCalculator.LoxodromicDistance(this, coordinates);
        }

        /// <summary>
        /// Возвращает конечную точку, заданную пеленгом и удалением по локсодромии. 
        /// </summary>
        /// <param name="bearing">Пеленг</param>
        /// <param name="distance">Удаление</param>
        public Coordinates LoxodromicDestination(Angle bearing, Distance distance)
        {
            return GeodesyCalculator.LoxodromicDestination(this, bearing, distance);
        }

        #endregion

        #region "Расчеты с учетом высоты"

        /// <summary>
        /// Возвращает угол места заданной точки.
        /// </summary>
        /// <param name="coordinates">Заданная точка</param>
        public Angle Elevation(Coordinates coordinates)
        {
            return GeodesyCalculator.Elevation(this, coordinates);
        }

        #endregion

    }

    /// <summary>
    /// Расчеты на земном сфероиде.
    /// </summary>
    /// <remarks>Игнорирование эллипсоидальной формы Земли дает максимальную погрешность в расчетах порядка 0.3%</remarks>
    public static class GeodesyCalculator
    {

        // Для расчетов, помимо прочего, использованы алгоритмы, опубликованные Ed Williams и Chris Veness
        // http://williams.best.vwh.net/avform.htm
        // http://www.movable-type.co.uk/scripts/latlong.html

        #region "Расчеты для проекции Меркатора"

        /// <summary>
        /// Возвращает пеленг точки2 относительно точки1 по прямой для проекции Меркатора.
        /// </summary>
        /// <remarks>Для расчета обратного пеленга следует просто поменять параметры местами.</remarks>
        public static Angle MercatorBearing(Coordinates coordinates1, Coordinates coordinates2)
        {
            return
                new Angle(
                    Math.Atan2(
                        (coordinates2.Longitude.Radians - coordinates1.Longitude.Radians) * Math.Cos((coordinates1.Latitude.Radians + coordinates2.Latitude.Radians) / 2)
                        ,
                        coordinates2.Latitude.Radians - coordinates1.Latitude.Radians
                    )
                , Angle.Unit.Radians);
        }

        /// <summary>
        /// Возвращает удаление точки2 от точки1 по прямой для проекции Меркатора.
        /// </summary>
        public static Distance MercatorDistance(Coordinates coordinates1, Coordinates coordinates2)
        {
            return 
                new Distance(
                    Math.Sqrt(
                        Math.Pow(
                            (coordinates2.Longitude.Radians - coordinates1.Longitude.Radians)
                            *
                            Math.Cos((coordinates1.Latitude.Radians + coordinates2.Latitude.Radians) / 2)
                        , 2)
                        +
                        Math.Pow(
                            coordinates2.Latitude.Radians - coordinates1.Latitude.Radians
                        , 2)
                    )
                , Distance.Unit.Radians);
        }

        #endregion

        #region "Расчеты по ортодромии"

        /// <summary>
        /// Возвращает прямой пеленг точки2 относительно точки1 по ортодромии.
        /// </summary>
        /// <remarks>Для расчета обратного пеленга следует просто поменять параметры местами.</remarks>
        public static Angle OrthodromicBearing(Coordinates coordinates1, Coordinates coordinates2)
        {
            double lat1 = coordinates1.Latitude.Radians;
            double lon1 = coordinates1.Longitude.Radians;
            double lat2 = coordinates2.Latitude.Radians;
            double lon2 = coordinates2.Longitude.Radians;
            double dLon = lon2 - lon1;
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double b = Math.Atan2(y, x);
            return new Angle(b, Angle.Unit.Radians);
        }

        /// <summary>
        /// Возвращает удаление точки2 от точки1 по ортодромии.
        /// </summary>
        public static Distance OrthodromicDistance(Coordinates coordinates1, Coordinates coordinates2)
        {
            double lat1 = coordinates1.Latitude.Radians;
            double lon1 = coordinates1.Longitude.Radians;
            double lat2 = coordinates2.Latitude.Radians;
            double lon2 = coordinates2.Longitude.Radians;
            double a = Math.Pow(Math.Sin((lat2 - lat1) / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon2 - lon1) / 2), 2);
            double d = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return new Distance(d, Distance.Unit.Radians);
        }

        /// <summary>
        /// Возвращает конечную точку от опорной точки, прямого пеленга и удаления по ортодромии.
        /// </summary>
        public static Coordinates OrthodromicDestination(Coordinates coordinates, Angle bearing, Distance distance)
        {
            double lat1 = coordinates.Latitude.Radians;
            double lon1 = coordinates.Longitude.Radians;
            double dist = distance.Radians;
            double brng = bearing.Radians;
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dist) + Math.Cos(lat1) * Math.Sin(dist) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dist) * Math.Cos(lat1), Math.Cos(dist) - Math.Sin(lat1) * Math.Sin(lat2));
            return new Coordinates(Angle.FromRadians(lat2), Angle.FromRadians(lon2), coordinates.Altitude);
        }

        /// <summary>
        /// Возвращает точку в середине пути между двумя точками по ортодромии.
        /// </summary>
        /// <remarks>Высота середины пути осредняется.</remarks>
        public static Coordinates OrthodromicMidPoint(Coordinates coordinates1, Coordinates coordinates2)
        {
            double lat1 = coordinates1.Latitude.Radians;
            double lon1 = coordinates1.Longitude.Radians;
            double lat2 = coordinates2.Latitude.Radians;
            double lon2 = coordinates2.Longitude.Radians;
            double dLon = (lon2 - lon1);
            double Bx = Math.Cos(lat2) * Math.Cos(dLon);
            double By = Math.Cos(lat2) * Math.Sin(dLon);
            double lat3 = Math.Atan2(Math.Sin(lat1) + Math.Sin(lat2), Math.Sqrt((Math.Cos(lat1) + Bx) * (Math.Cos(lat1) + Bx) + By * By));
            double lon3 = lon1 + Math.Atan2(By, Math.Cos(lat1) + Bx);
            double alt = (coordinates1.Altitude.Meters + coordinates2.Altitude.Meters) / 2.0;
            return new Coordinates(Angle.FromRadians(lat3), Angle.FromRadians(lon3), Altitude.FromMeters(alt));
        }

        #endregion

        #region "Расчеты по локсодромии"

        /// <summary>
        /// Возвращает прямой пеленг точки2 относительно точки1 по локсодромии.
        /// </summary>
        /// <remarks>Для расчета обратного пеленга следует отнять/прибавить 180 градусов.</remarks>
        public static Angle LoxodromicBearing(Coordinates coordinates1, Coordinates coordinates2)
        {
            double lat1 = coordinates1.Latitude.Radians;
            double lon1 = coordinates1.Longitude.Radians;
            double lat2 = coordinates2.Latitude.Radians;
            double lon2 = coordinates2.Longitude.Radians;
            double dLon = lon2 - lon1;
            double dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
            {
                if (dLon > 0)
                {
                    dLon = -(2 * Math.PI - dLon);
                }
                else {
                    dLon = +(2 * Math.PI + dLon);
                }
            }
            double b = Math.Atan2(dLon, dPhi);
            return Angle.FromRadians(b);
        }

        /// <summary>
        /// Возвращает удаление точки2 от точки1 по локсодромии.
        /// </summary>
        public static Distance LoxodromicDistance(Coordinates coordinates1, Coordinates coordinates2)
        {
            double lat1 = coordinates1.Latitude.Radians;
            double lon1 = coordinates1.Longitude.Radians;
            double lat2 = coordinates2.Latitude.Radians;
            double lon2 = coordinates2.Longitude.Radians;
            double dLat = lat2 - lat1;
            double dLon = Math.Abs(lon2 - lon1);
            double dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            double q = 0;
            if (dPhi == 0)
            {
                q = Math.Cos(lat1);
            }
            else {
                q = dLat / dPhi;
            }
            if (dLon > Math.PI)
                dLon = 2 * Math.PI - dLon;
            double d = Math.Sqrt(dLat * dLat + q * q * dLon * dLon);
            return new Distance(d, Distance.Unit.Radians);
        }

        /// <summary>
        /// Возвращает конечную точку от опорной точки, прямого пеленга и удаления по локсодромии.
        /// </summary>
        public static Coordinates LoxodromicDestination(Coordinates coordinates, Angle bearing, Distance distance)
        {
            double lat1 = coordinates.Latitude.Radians;
            double lon1 = coordinates.Longitude.Radians;
            double brng = bearing.Radians;
            double dist = distance.Radians;
            double lat2 = lat1 + dist * Math.Cos(brng);
            double dLat = lat2 - lat1;
            double dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            double q = 0;
            if (dPhi == 0)
            {
                q = Math.Cos(lat1);
            }
            else {
                q = dLat / dPhi;
            }
            double dLon = dist * Math.Sin(brng) / q;
            if (Math.Abs(lat2) > Math.PI / 2)
            {
                if (lat2 > 0)
                {
                    lat2 = +(Math.PI - lat2);
                }
                else {
                    lat2 = -(Math.PI - lat2);
                }
            }
            double lon2 = (lon1 + dLon + Math.PI) % (2 * Math.PI) - Math.PI;
            return new Coordinates(Angle.FromRadians(lat2), Angle.FromRadians(lon2), coordinates.Altitude);
        }

        #endregion

        #region "Расчеты с учетом высоты"

        /// <summary>
        /// Возвращает удаление точки2 от точки1 по прямой.
        /// </summary>
        public static Distance DirectDistance(Coordinates coordinates1, Coordinates coordinates2)
        {
            // Радиус земли (км)
            double r = Distance.GetEarthRadius(Distance.Unit.Kilometers);
            // Удаление точек от центра земли (км)
            double h1 = r + coordinates1.Altitude.Meters / 1000.0;
            double h2 = r + coordinates2.Altitude.Meters / 1000.0;
            // Угол между отрезками от цента земли на точки (рад)
            double b = OrthodromicDistance(coordinates1, coordinates2).Radians;
            // Расстояние между точками по прямой (км)
            double d = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) - 2 * h1 * h2 * Math.Cos(b));
            // Результат
            if (double.IsNaN(d))
            {
                d = 0;
            }
            return Distance.FromKilometers(d);
        }

        /// <summary>
        /// Возвращает угол места точки2 для точки1.
        /// </summary>
        public static Angle Elevation(Coordinates coordinates1, Coordinates coordinates2)
        {
            // Радиус земли (км)
            double r = Distance.GetEarthRadius(Distance.Unit.Kilometers);
            // Удаление точек от центра земли (км)
            double h1 = r + coordinates1.Altitude.Meters / 1000.0;
            double h2 = r + coordinates2.Altitude.Meters / 1000.0;
            // Угол между отрезками от цента земли на точки (рад)
            double b = OrthodromicDistance(coordinates1, coordinates2).Radians;
            // Расстояние между точками по прямой (км)
            double d = Math.Sqrt(Math.Pow(h1, 2) + Math.Pow(h2, 2) - 2 * h1 * h2 * Math.Cos(b));
            // Угол места точки2 (рад)
            double result = Math.Acos(h2 * Math.Sin(b) / d);
            // Точки слишком близко
            if (double.IsNaN(result))
            {
                result = Math.Sign(coordinates2.Altitude.Meters - coordinates1.Altitude.Meters) * Math.PI / 2.0;
                // Точки достаточно далеко
            }
            else {
                if (b >= Math.PI / 2 || h2 < h1 / Math.Cos(b))
                {
                    // Угол места отрицательный (т.е. точка 2 ниже горизонтали в точке1)
                    result *= (-1);
                }
            }
            return Angle.FromRadians(result);
        }

        #endregion

    }

}
