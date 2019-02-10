using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using Primitives;
using Geography;

namespace IWHTest
{
    class Program
    {

        static void Main(string[] args)
        {

            //bool calcSpbOnly = false;

            var stopwatch = new System.Diagnostics.Stopwatch();
            var IwhMap = new IWH.Map();

            string folderResources = @"\Projects\IWasHere\Resources";
            //if (calcSpbOnly) folderResources = @"\Projects\IWasHere\Resources\IWasHere78";

            // Загружаем границы областей

            Console.WriteLine("Загрузка границ области...");
            stopwatch.Restart();
            // Внешняя граница
            GeoArea areaInclude = AreaFromGpx(@"\Projects\IWasHere\Resources\RU-NW_area.gpx");
            //GeoArea areaInclude = null;
            // Внутреняя граница
            GeoArea areaExclude = AreaFromGpx(@"\Projects\IWasHere\Resources\RU-SPE_area.gpx");
            //GeoArea areaExclude = null;
            // Корректировка типа дорог
            //GeoArea areaFix1 = AreaFromGpx(@"\Projects\IWasHere\Resources\Fix_Tertiary1.gpx");
            //GeoArea areaFix2 = AreaFromGpx(@"\Projects\IWasHere\Resources\Fix_Tertiary2.gpx");
            GeoArea areaFix1 = null;
            GeoArea areaFix2 = null;
            //if (calcSpbOnly)
            //{
            //    // Питер в границах КАД
            //    areaInclude = AreaFromGpx(@"\Projects\IWasHere\Resources\RU-SPE_area.gpx");
            //    areaExclude = null;
            //    areaFix1 = null;
            //    areaFix2 = null;
            //}
            Console.WriteLine("Загрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("----------------");

            // -----

            //// Формируем локальную базу

            ////string[] osmFileNames =
            ////    {
            ////        @"\Temp\IWasHere\RU-KR.osm",
            ////        @"\Temp\IWasHere\RU-LEN.osm",
            ////        @"\Temp\IWasHere\RU-MUR.osm",
            ////        @"\Temp\IWasHere\RU-NGR.osm",
            ////        @"\Temp\IWasHere\RU-PSK.osm",
            ////        @"\Temp\IWasHere\RU-VLG.osm"
            ////    };
            //string[] osmFileNames =
            //    {
            //        @"\Temp\IWasHere\northwestern-fed-district-latest.osm"
            //    };

            //Console.WriteLine("Формирование базы по данным из OSM...");
            //stopwatch.Restart();
            ////if (calcSpbOnly)
            ////    IwhMap.LoadFromOsm(@"\Temp\IWasHere\RU-SPE.osm", areaInclude, areaExclude);
            ////else
            //    IwhMap.LoadFromOsm(osmFileNames, areaInclude, areaExclude);
            //Console.WriteLine("Формирование выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            //Console.WriteLine("Длина {0}км", Math.Round(IwhMap.TotalLenght.Kilometers, 1));
            //Console.WriteLine("----------------");

            ////// Удаляем НП с населением менее 2048

            ////Console.WriteLine("Контроль населенных пунктов...");
            ////Console.WriteLine("Узлов до удаления {0}", IwhMap.Nodes.Count);
            ////stopwatch.Restart();
            ////Int32 VillageCount = 0;
            ////foreach (var node in IwhMap.Nodes.Values.ToList())
            ////{
            ////    if (node.Type == IWH.NodeType.Village)
            ////    {
            ////        if (node.Population < 2014)
            ////            IwhMap.Nodes.Remove(node.OsmId);
            ////        else
            ////            VillageCount++;
            ////    }
            ////}
            ////Console.WriteLine("Контроль выполнен за {0} мсек", stopwatch.ElapsedMilliseconds);
            ////Console.WriteLine("Узлов после удаления {0}", IwhMap.Nodes.Count);
            ////Console.WriteLine("из них нас. пунктов {0}", VillageCount);
            ////Console.WriteLine("----------------");

            ////Записываем базу

            //Console.WriteLine("Сохранение базы данных...");
            //stopwatch.Restart();
            //IwhMap.WriteToXml(folderResources + @"\IwhMap.xml");
            //Console.WriteLine("Сохранение выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("----------------");

            // -----

            // Считываем базу

            Console.WriteLine("Загрузка базы данных...");
            stopwatch.Restart();
            IwhMap = IWH.Map.ReadFromXml(folderResources + @"\IwhMap.xml");
            IwhMap.Recalculate();
            Console.WriteLine("Загрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            Console.WriteLine("Длина {0}км, Посещено {1}км ({2}%)",
                Math.Round(IwhMap.TotalLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers / IwhMap.TotalLenght.Kilometers * 100, 2));
            Console.WriteLine("----------------");

            Console.WriteLine("Корректировка путей...");
            stopwatch.Restart();
            IwhMap.FixWayTypeInArea(areaFix1, IWH.HighwayType.Tertiary);
            IwhMap.FixWayTypeInArea(areaFix2, IWH.HighwayType.Tertiary);
            IwhMap.CombineShortWaysWithAdjacent();
            IwhMap.DivideWaysByCrossroads();
            IwhMap.DivideWaysByLenght();
            IwhMap.Recalculate();
            Console.WriteLine("Корректировка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            Console.WriteLine("Длина {0}км, Посещено {1}км ({2}%)",
                Math.Round(IwhMap.TotalLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers / IwhMap.TotalLenght.Kilometers * 100, 2));
            Console.WriteLine("----------------");

            // Анализируем треки

            DirectoryInfo trackFolder = new DirectoryInfo(@"\Temp\IWasHere\Tracks");
            FileInfo[] trackFiles;
            trackFiles = trackFolder.GetFiles("*.gpx");
            // Обрабатываем треки
            stopwatch.Restart();
            foreach (FileInfo trackFile in trackFiles)
            {
                Console.WriteLine("Анализ файла {0}", trackFile.Name);
                GPS.Gpx gpxTrack = GPS.Gpx.FromXmlFile(trackFile.FullName);
                Distance cacheRange = Distance.FromKilometers(4);
                //AnalizeGpsTrack(IwhMap.GetLegsList(), gpxTrack.GetPointList(), cacheRange, calcSpbOnly);
                AnalizeGpsTrack(IwhMap.GetLegsList(), gpxTrack.GetPointList(), cacheRange, false);
            }
            Console.WriteLine("Анализ выполнен за {0} мсек", stopwatch.ElapsedMilliseconds);
            // Пересчитываем
            stopwatch.Restart();
            IwhMap.Recalculate();
            Console.WriteLine("Пересчет выполнен за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Всего из {0}км посещено {1}км",
                Math.Round(IwhMap.TotalLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers, 1));
            Console.WriteLine("Из требуемых {0}км посещено {1}км ({2}%)",
                Math.Round(IwhMap.TargetLenght.Kilometers, 1),
                Math.Round(IwhMap.TargetVisitedLenght.Kilometers, 1),
                Math.Round(IwhMap.TargetVisitedLenght.Kilometers / IwhMap.TargetLenght.Kilometers * 100, 2));
            Console.WriteLine("----------------");

            //// Записываем базу

            //Console.WriteLine("Сохранение базы данных...");
            //stopwatch.Restart();
            //IwhMap.WriteToXml(folderResources + @"\IwhMap.xml");
            //Console.WriteLine("Сохранение выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("----------------");

            // Выгружаем треки

            Console.WriteLine("Выгрузка GPS-трека...");
            stopwatch.Restart();
            MapToGpx(WaysByType(IwhMap.Ways, new List<IWH.HighwayType>() { IWH.HighwayType.Motorway, IWH.HighwayType.Trunk }), false, folderResources + @"\Way_Primary.gpx");
            MapToGpx(WaysByType(IwhMap.Ways, new List<IWH.HighwayType>() { IWH.HighwayType.Primary, IWH.HighwayType.Secondary }), false, folderResources + @"\Way_Secondary.gpx");
            MapToGpx(WaysByType(IwhMap.Ways, new List<IWH.HighwayType>() { IWH.HighwayType.Tertiary }), false, folderResources + @"\Way_Tertiary.gpx");
            MapToGpx(IwhMap.Ways, true, folderResources + @"\Way_Visited.gpx");
            MapToGpx(WaysBySpeed(IwhMap.Ways, Speed.FromKilometersPerHour(5), Speed.FromKilometersPerHour(25)), true, folderResources + @"\Way_Visited_25kmh.gpx");
            MapToGpx(WaysBySpeed(IwhMap.Ways, Speed.FromKilometersPerHour(25), Speed.FromKilometersPerHour(50)), true, folderResources + @"\Way_Visited_50kmh.gpx");
            MapToGpx(WaysBySpeed(IwhMap.Ways, Speed.FromKilometersPerHour(50), Speed.FromKilometersPerHour(85)), true, folderResources + @"\Way_Visited_85kmh.gpx");
            MapToGpx(WaysBySpeed(IwhMap.Ways, Speed.FromKilometersPerHour(85), Speed.FromKilometersPerHour(200)), true, folderResources + @"\Way_Visited_99kmh.gpx");
            MapToGpx(WaysByTimes(IwhMap.Ways, 0, 1), true, folderResources + @"\Way_Times_1.gpx");
            MapToGpx(WaysByTimes(IwhMap.Ways, 1, 3), true, folderResources + @"\Way_Times_2.gpx");
            MapToGpx(WaysByTimes(IwhMap.Ways, 3, 99), true, folderResources + @"\Way_Times_4.gpx");
            CrossroadsToGpx(IwhMap, folderResources + @"\Way_Crossroads.gpx");
            stopwatch.Stop();
            Console.WriteLine("Выгрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("----------------");

            // Конец

            Console.WriteLine("Работа завершена. Нажмите [Enter] для выхода.");
            Console.ReadLine();
        }

        /// <summary>
        /// Возвращает географическую область, загруженную из gpx-файла
        /// </summary>
        /// <param name="gpxFileName"></param>
        /// <returns></returns>
        static Geography.GeoArea AreaFromGpx(string gpxFileName)
        {
            Geography.GeoArea result = new Geography.GeoArea();
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            XmlDocument xml = new XmlDocument();
            XmlNamespaceManager prefix;
            xml.Load(gpxFileName);
            prefix = new XmlNamespaceManager(xml.NameTable);
            prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            foreach (XmlNode n in xml.SelectNodes("//prfx:gpx/prfx:rte/prfx:rtept", prefix))
            {
                double lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
                double lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
                result.Points.Add(new GeoPoint(lat, lon, 0));
            }
            result.Recalculate();
            return result;
        }

        /// <summary>
        /// Анализирует список точек GPS и отмечает IsVisible у участков IWH.
        /// </summary>
        /// <remarks>
        /// Предполагалается, что точки в треке, расположены в порядке времени следования по маршруту.
        /// Участок пути считается посещенным, если точка трека находится на допустимом удалении
        /// и направления движения по треку соответствует направлению участка.
        /// Для оптимизации, работа проводится с участками пути, расположенными в заданном радиусе от точки трека.
        /// </remarks>
        static void AnalizeGpsTrack(List<IWH.Leg> mapLegs, List<GPS.TrackPoint> gpsPoints, Distance cacheRange, bool checkOneWay)
        {
            // Максимальный угол, при котором линия трека считается совпадающей с линией пути
            Angle MaximumVisitedDeviation = Angle.FromDegrees(16);
            // Максимальное боковое уклонение точки трека от прямой на линии пути
            Distance MaximumVisitedOffset = Distance.FromMeters(32);
            // Удаление, от точки трека, при котором участок развязки считается посещенным
            Distance LinksVisitedDistance = Distance.FromMeters(256);
            // Время (мин), при истечении котого точка считается посещенной повторно
            Time VisitedCountInerval = Time.FromMinutes(8);
            // Минимальный интервал времени, который требуется для расчета средней скорости
            Time MinimumSpeedInterval = Time.FromSeconds(5);
            // Минимальная средняя скорость, которую принимаем как реальную (не шум)
            Speed MinimumAverageSpeed = Speed.FromKilometersPerHour(5);

            // Создаём два уровня кэшей для участков
            LegsCache cache2 = new LegsCache(mapLegs, cacheRange * 8);
            LegsCache cache1 = new LegsCache(cache2.Legs, cacheRange);
            // Обходим все точки трека, кроме последней.
            GPS.TrackPoint gpsPoint;
            IWH.Leg gpsLeg;
            var avrSpeedCouner = new IWH.AverageSpeedCounter(MinimumSpeedInterval);
                        
            for (int i = 0; i < gpsPoints.Count-1; i++ )
            {
                gpsPoint = gpsPoints[i];
                // Вычисляем направление и длину участка трека
                gpsLeg = new IWH.Leg();
                gpsLeg.StartPoint = new IWH.Node() { Coordinates = gpsPoints[i].Coordinates };
                gpsLeg.EndPoint = new IWH.Node() { Coordinates = gpsPoints[i + 1].Coordinates };
                gpsLeg.Direction = gpsLeg.StartPoint.Coordinates.OrthodromicBearing(gpsLeg.EndPoint.Coordinates);
                gpsLeg.Lenght = gpsLeg.StartPoint.Coordinates.OrthodromicDistance(gpsLeg.EndPoint.Coordinates);
                // Вычисляем среднюю скорость движения
                avrSpeedCouner.Add(gpsPoints[i + 1].Time-gpsPoints[i].Time, gpsLeg.Lenght);
                gpsLeg.Speed = avrSpeedCouner.GetAverageSpeed();
                // Проверяем состояние кешей
                cache2.CheckCoordinates(gpsLeg.StartPoint.Coordinates, cacheRange);
                cache1.CheckCoordinates(gpsLeg.StartPoint.Coordinates, gpsLeg.Lenght);

                // Обсчитываем для точки трека все находящиеся в кеше участки пути
                foreach (var leg in cache1.Legs)
                {
                    // Проверяем удаление точки трека и начальной точки участка пути
                    Boolean legIsVisited = false;
                    Distance maxDistance = Distance.FromMeters(Math.Sqrt( Math.Pow(gpsLeg.Lenght.Meters+leg.Lenght.Meters,2)+Math.Pow(MaximumVisitedOffset.Meters,2) )); 
                    Distance factDistance = gpsPoint.Coordinates.MercatorDistance(leg.StartPoint.Coordinates);
                    // Для развязок простые правила
                    if (leg.Way.IsLink && factDistance < LinksVisitedDistance)
                    {
                        legIsVisited = true;
                    }
                    // Для соединений простое правила
                    else if (leg.Way.Legs.Count == 1 && leg.Lenght < MaximumVisitedOffset && factDistance < LinksVisitedDistance)
                    {
                        legIsVisited = true;
                    }
                    // Для остальных дорог правила посложнее
                    else if (factDistance <= maxDistance)
                    {
                        // Сравниваем направление участка пути с направление трека
                        // Добавляем к максимуму 200% на каждый километр длины трека 
                        Angle maxDeviation = MaximumVisitedDeviation + MaximumVisitedDeviation * (gpsLeg.Lenght.Kilometers*2);
                        Angle factDeviation = (gpsLeg.Direction - leg.Direction).Abs();
                        if ( (factDeviation < maxDeviation) || ( Angle.Straight - factDeviation < maxDeviation & (!checkOneWay | !leg.Way.OneWay)) )
                        {
                            // Проверяем боковое удаление точки трека от участка пути
                            // Добавляем к максимуму 200% на каждый километр длины трека
                            Distance maxOffset = MaximumVisitedOffset + MaximumVisitedOffset * (gpsLeg.Lenght.Kilometers*2);
                            Distance factOffset = leg.MinLegOffset(gpsLeg);
                            if (factOffset >= Distance.Zero && factOffset < maxOffset)
                            {
                                legIsVisited = true;
                            }
                        }
                    }
                    // Отмечаем участок как посещенный; обновляем скорость, время и количество посещений
                    if (legIsVisited)
                    {
                        leg.IsVisited = true;
                        if (gpsPoint.Time > leg.LastVisitedTime)
                        {
                            if (leg.VisitedCount == 0 || (gpsPoint.Time - leg.LastVisitedTime) > VisitedCountInerval)
                                leg.VisitedCount += 1;
                            leg.LastVisitedTime = gpsPoint.Time;
                        }
                        if (gpsLeg.Speed > MinimumAverageSpeed)
                        {
                            // Сначала считали среднюю скорость
                            if (leg.Speed.IsEmpty)
                                leg.Speed = gpsLeg.Speed;
                            else
                                leg.Speed = (leg.Speed + gpsLeg.Speed) / 2;
                            //Теперь считаем максимальную
                            //if (gpsLeg.Speed > leg.Speed)
                            //    leg.Speed = gpsLeg.Speed;
                        }
                    }

                } // leg
                
            } // i
        }

        private class LegsCache
        {
            private Coordinates _center;
            private Distance _range;
            private List<IWH.Leg> _legs;

            internal List<IWH.Leg> Legs { get; private set; }

            internal LegsCache(List<IWH.Leg> legsList, Distance cacheRange)
            {
                _legs = legsList;
                _range = cacheRange;
                Legs = new List<IWH.Leg>();
            }

            internal void CheckCoordinates(Coordinates coordinates, Distance minDistance)
            {
                // Проверяем нахождение текущей точки трека в радиусе загруженного кеша участков
                if (_center.IsEmpty || _center.MercatorDistance(coordinates) + minDistance > _range)
                {
                    // Устанавливаем центр кеша на текущую точку трека
                    _center = coordinates;
                    // Заново формируем кэш вокруг центральной точки
                    Legs.Clear();
                    foreach (var leg in _legs)
                    {
                        if (_center.MercatorDistance(leg.StartPoint.Coordinates) <= _range)
                            Legs.Add(leg);
                    }
                }
            }
        }

        /// <summary>
        /// Выгружает заданный список линий в файл GPS-трекинга
        /// </summary>
        /// <param name="wayList"></param>
        /// <param name="visitedOnly">Выгружать только посещенные участки линии</param>
        /// <param name="outputFileName"></param>
        static void MapToGpx(List<IWH.Way> wayList, Boolean visitedOnly, string outputFileName)
        {
            var gpx = new GPS.Gpx();
            var legs = new List<IWH.Leg>();
            // Подготавливаем линии для выгрузки
            foreach (IWH.Way way in wayList)
            {
                // Формируем выборку участков для выгрузки трека 
                foreach (IWH.Leg leg in way.Legs)
                {
                    if (!visitedOnly || leg.IsVisited)
                        legs.Add(leg);
                }
                // Выгружаем накопленный набор участков
                MapToGps_PushLegs(gpx, way, legs);
            }
            // Выгружаем в файл
            gpx.SaveToFile(outputFileName);
        }

        private static void MapToGps_PushLegs(GPS.Gpx gpx, IWH.Way way, List<IWH.Leg> legs)
        {
            if (legs.Count>0)
            {

                // ??? debug
                if (way.OsmId == 38179833)
                {
                    int f = 0;
                }

                GPS.Track newTrack = new GPS.Track();
                // Определяем реквизиты трека
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.OsmId.ToString() + ")";
                if (way.IsLink)
                    newTrack.Name += " Link";
                if (way.OneWay)
                    newTrack.Name += " ONE_WAY";
                if (way.Lanes > 0)
                    newTrack.Name += " L=" + way.Lanes.ToString();
                if (way.Surface > 0)
                    newTrack.Name += " " + way.Surface.ToString();
                if (way.Smoothness > 0)
                    newTrack.Name += " " + way.Smoothness.ToString();
                if (way.LastVisitedTime > DateTime.MinValue)
                    newTrack.Name += " " + way.LastVisitedTime.ToShortDateString();
                if (way.AverageSpeed > Speed.Zero)
                    newTrack.Name += " " + String.Format("{0:000}кмч", way.AverageSpeed.KilometersPerHour);
                if (way.AverageVisitedCount > 0.0)
                    newTrack.Name += " " + String.Format("{0:0.00}", way.AverageVisitedCount);
                // Формируем сегменты
                GPS.TrackSegment newTrackSegment = new GPS.TrackSegment();
                for (int i = 0; i < legs.Count; i++)
                {
                    // Создаем новый сегмент для первого участка или
                    // если первая точка текущего участка не равна последней точке предыдущего
                    // Для первого участка добавляем обе точки, для остальных только конечную.
                    if (i == 0 || !legs[i].StartPoint.Equals(legs[i-1].EndPoint))
                    {
                        newTrack.Segments.Add(newTrackSegment);
                        newTrackSegment = new GPS.TrackSegment();
                        newTrackSegment.Points.Add(new GPS.TrackPoint() { Coordinates = legs[i].StartPoint.Coordinates });
                    }
                    newTrackSegment.Points.Add(new GPS.TrackPoint() { Coordinates = legs[i].EndPoint.Coordinates });
                }
                //
                newTrack.Segments.Add(newTrackSegment);
                gpx.Tracks.Add(newTrack);
                legs.Clear();
            }
        }

        /// <summary>
        /// Выгружает перекрестки в файл GPS-трекинга
        /// </summary>
        /// <param name="outputFileName"></param>
        static void CrossroadsToGpx(IWH.Map map, string outputFileName)
        {
            GPS.Gpx gpx = new GPS.Gpx();
            GPS.WayPoint newWayPoint;
            // Подготавливаем точки для выгрузки
            var goodNodeTypeList = new List<IWH.NodeType> { IWH.NodeType.City, IWH.NodeType.Town, IWH.NodeType.Village };
            foreach (IWH.Node node in map.Nodes.Values)
            {
                if (node.Legs.Count > 2)
                {
                    newWayPoint = new GPS.WayPoint();
                    newWayPoint.Coordinates = node.Coordinates;
                    newWayPoint.Name = string.Format("{0} ({1})", node.OsmId, node.Legs.Count);
                    gpx.WayPoints.Add(newWayPoint);
                }
            }
            // Выгружаем в файл
            gpx.SaveToFile(outputFileName);
        }

        static List<IWH.Way> WaysByType(List<IWH.Way> wayList, List<IWH.HighwayType> typeList)
        {
            var result = new List<IWH.Way>();
            foreach (var way in wayList)
            {
                if (typeList.Contains(way.Type))
                    result.Add(way);
            }
            return result;
        }

        static List<IWH.Way> WaysBySpeed(List<IWH.Way> wayList, Speed minSpeed, Speed maxSpeed)
        {
            var result = new List<IWH.Way>();
            foreach (var way in wayList)
            {
                if (way.AverageSpeed >= minSpeed && way.AverageSpeed < maxSpeed)
                    result.Add(way);
            }
            return result;
        }

        static List<IWH.Way> WaysByTimes(List<IWH.Way> wayList, double minVisitedCount, double maxVisitedCount)
        {
            var result = new List<IWH.Way>();
            foreach (var way in wayList)
            {
                if (way.AverageVisitedCount > minVisitedCount && way.AverageVisitedCount <= maxVisitedCount)
                    result.Add(way);
            }
            return result;
        }

        // Это было нужно, чтобы выбрать из базы ОСМ все типы тегов по дорогам и наспунктам
        static void SelectOsmAttributes(string[] args)
        //static void Main(string[] args)
        {
            StreamWriter csv = new StreamWriter(new FileStream(@"\Projects\IWasHere\Resources\Attributes.csv", FileMode.Create));
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

            /// Первым проходом читаем линии и сохраняем только нужные
            using (XmlReader xml = XmlReader.Create(@"\Temp\IWasHere\RU-LEN.osm"))
            {
                while (xml.Read())
                {
                    if (xml.NodeType == XmlNodeType.Element && (xml.Name == "way" || xml.Name == "node"))
                    {
                        string type = xml.Name;
                        // Создаем новую линию
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml.ReadOuterXml());
                        XmlNode xmlWay = xmlDoc.SelectSingleNode("/" + type);
                        /// Загрузка тэгов
                        string tag = string.Empty;
                        string value = string.Empty;
                        string name = string.Empty;
                        string surface = string.Empty;
                        string smoothness = string.Empty;
                        string id = string.Empty;
                        foreach (XmlNode xmlTag in xmlDoc.SelectNodes("/" + type + "/tag"))
                        {
                            string key = xmlTag.Attributes["k"].Value;
                            if (key == "highway" || key == "place")
                            {
                                tag = key;
                                value = xmlTag.Attributes["v"].Value;
                            }
                            if (key == "name")
                                name = xmlTag.Attributes["v"].Value;
                            if (key == "surface")
                                surface = xmlTag.Attributes["v"].Value;
                            if (key == "smoothness")
                                smoothness = xmlTag.Attributes["v"].Value;
                        }
                        /// Линия отработана
                        if (tag == "highway" || tag == "place")
                            csv.WriteLine("{0};{1};{2};{3};{4};{5};{6}", type, tag, value, xmlWay.Attributes["id"].Value, name, surface, smoothness);
                    }
                }
            }
            csv.Close();

            Console.WriteLine("Done. Press [Enter] to exit.");
            Console.ReadLine();
        }

        // Это было нужно, чтобы собрать статистику по количеству точек и длине загруженных линий
        static void SelectWaysStat(List<IWH.Way> Ways)
        {
            StreamWriter csv = new StreamWriter(new FileStream(@"\Projects\IWasHere\Resources\WaysStat.csv", FileMode.Create));
            IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            foreach (IWH.Way way in Ways)
            {
                csv.WriteLine("{0};{1};{2};{3}",
                    way.OsmId, way.Name, way.Legs.Count, way.Lenght.Kilometers
                );

            }
            csv.Close();
        }

    }
}
