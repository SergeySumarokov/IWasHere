using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Primitives;

namespace IWHTest
{
    class Program
    {

        static void Main(string[] args)
        {

            var stopwatch = new System.Diagnostics.Stopwatch();
            var IwhMap = new IWH.Map();

            //// Загружаем границы областей

            //Console.WriteLine("Загрузка границ области...");
            //stopwatch.Restart();
            //IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            //XmlDocument xml = new XmlDocument();
            //XmlNamespaceManager prefix;
            //// Питер в границах КАД
            //xml.Load(@"\Projects\IWasHere\Resources\RU-SPE_area.gpx");
            //prefix = new XmlNamespaceManager(xml.NameTable);
            //prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            //var areaSpb = new Area();
            //foreach (XmlNode n in xml.SelectNodes("//prfx:gpx/prfx:rte/prfx:rtept", prefix))
            //{
            //    double lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
            //    double lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
            //    areaSpb.Points.Add(new Coordinates(lat, lon, 0));
            //}
            //// Ленобласть
            //xml.Load(@"\Projects\IWasHere\Resources\RU-LEN_area.gpx");
            //prefix = new XmlNamespaceManager(xml.NameTable);
            //prefix.AddNamespace("prfx", xml.DocumentElement.NamespaceURI);
            //var areaLen = new Area();
            //foreach (XmlNode n in xml.SelectNodes("//prfx:gpx/prfx:rte/prfx:rtept", prefix))
            //{
            //    double lat = double.Parse(n.Attributes["lat"].Value, xmlFormatProvider);
            //    double lon = double.Parse(n.Attributes["lon"].Value, xmlFormatProvider);
            //    areaLen.Points.Add(new Coordinates(lat, lon, 0));
            //}
            //Console.WriteLine("Загрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("----------------");

            //// Формируем локальную базу

            //Console.WriteLine("Формирование базы по данным из OSM...");
            //stopwatch.Restart();
            //IwhMap.LoadFromOsm(@"\Temp\IWasHere\RU-LEN.osm", areaLen, areaSpb);
            //Console.WriteLine("Формирование выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            //Console.WriteLine("Длина {0}км", Math.Round(IwhMap.TotalLenght.Kilometers, 1));
            //Console.WriteLine("----------------");

            //// Удаляем НП с населением менее 2048

            //Console.WriteLine("Контроль населенных пунктов...");
            //Console.WriteLine("Узлов до удаления {0}", IwhMap.Nodes.Count);
            //stopwatch.Restart();
            //Int32 VillageCount = 0;
            //foreach (var node in IwhMap.Nodes.Values.ToList())
            //{
            //    if (node.Type == IWH.NodeType.Village)
            //    {
            //        if (node.Population < 2014)
            //            IwhMap.Nodes.Remove(node.Id);
            //        else
            //            VillageCount++;
            //    }
            //}
            //Console.WriteLine("Контроль выполнен за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Узлов после удаления {0}", IwhMap.Nodes.Count);
            //Console.WriteLine("из них нас. пунктов {0}", VillageCount);
            //Console.WriteLine("----------------");

            //// Записываем базу

            //Console.WriteLine("Сохранение базы данных...");
            //stopwatch.Restart();
            //IwhMap.WriteToXml(@"\Projects\IWasHere\Resources\IwhMap.xml");
            //Console.WriteLine("Сохранение выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("----------------");

            // Считываем базу

            Console.WriteLine("Загрузка базы данных...");
            stopwatch.Restart();
            IwhMap = IWH.Map.ReadFromXml(@"\Projects\IWasHere\Resources\IwhMap.xml");
            Console.WriteLine("Загрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            Console.WriteLine("Длина {0}км, Посещено {1}км ({2}%)",
                Math.Round(IwhMap.TotalLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers, 1),
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers / IwhMap.TotalLenght.Kilometers * 100, 2));
            Console.WriteLine("----------------");

            // Анализируем треки

            // ??? времянка начало
            var LegList = new List<IWH.Leg>();
            foreach (IWH.Way way in IwhMap.Ways.Values) foreach (IWH.Leg leg in way.Legs) LegList.Add(leg);
            // ??? времянка конец
            DirectoryInfo trackFolder = new DirectoryInfo(@"\Projects\IWasHere\Resources\Tracks");
            FileInfo[] trackFiles;
            trackFiles = trackFolder.GetFiles("*.gpx");
            // Обрабатываем треки
            stopwatch.Restart();
            foreach (FileInfo trackFile in trackFiles)
            {
                Console.WriteLine("Анализ файла {0}", trackFile.Name);
                GPS.Gpx gpxTrack = GPS.Gpx.FromXmlFile(trackFile.FullName);
                Distance cacheRange;
                Distance accupacy;
                if (trackFile.Name.Contains("GpsHome"))
                {
                    cacheRange = new Distance(4, Distance.Unit.Kilometers);
                    accupacy = new Distance(100, Distance.Unit.Meters);
                }
                else
                {
                    cacheRange = new Distance(4, Distance.Unit.Kilometers);
                    accupacy = new Distance(10, Distance.Unit.Meters);
                }
                AnalizeGpsTrack(LegList, gpxTrack.GetPointList(), cacheRange, accupacy);
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
            //IwhMap.WriteToXml(@"\Projects\IWasHere\Resources\IwhMap.xml");
            //Console.WriteLine("Сохранение выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("----------------");

            // Выгружаем треки

            Console.WriteLine("Выгрузка GPS-трека...");
            stopwatch.Restart();
            MapToGpx(WaysByType(IwhMap.Ways.Values.ToList(), new List<IWH.HighwayType>() { IWH.HighwayType.Motorway, IWH.HighwayType.Trunk }), false, @"\Projects\IWasHere\Resources\Way_Primary.gpx");
            MapToGpx(WaysByType(IwhMap.Ways.Values.ToList(), new List<IWH.HighwayType>() { IWH.HighwayType.Primary, IWH.HighwayType.Secondary }), false, @"\Projects\IWasHere\Resources\Way_Secondary.gpx");
            MapToGpx(WaysByType(IwhMap.Ways.Values.ToList(), new List<IWH.HighwayType>() { IWH.HighwayType.Tertiary }), false, @"\Projects\IWasHere\Resources\Way_Tertiary.gpx");
            MapToGpx(IwhMap.Ways.Values.ToList(), true, @"\Projects\IWasHere\Resources\Way_Visited.gpx");
            CrossroadsToGpx(IwhMap, @"\Projects\IWasHere\Resources\Way_Crossroads.gpx");
            stopwatch.Stop();
            Console.WriteLine("Выгрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("----------------");

            // Конец

            Console.WriteLine("Работа завершена. Нажмите [Enter] для выхода.");
            Console.ReadLine();
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
        static void AnalizeGpsTrack(List<IWH.Leg> iwhLegs, List<GPS.TrackPoint> gpsPoints, Distance cacheRange, Distance accuracy)
        {
            // Максимальный угол, при котором линия трека считается совпадающей с линией пути
            Angle MaximumVisibleDeviation = new Angle(8, Angle.Unit.Degrees);
            // Максимальное боковое уклонение точки трека от прямой на линии пути
            Distance MaximumVisibleOffset = new Distance(16, Distance.Unit.Meters);
            // Время (мин), при истечении котого точка считается посещенной повторно
            const int visitedCountInerval = 4;

            // Обходим все точки трека, кроме последней.
            var cacheLegs = new List<IWH.Leg>();
            var cacheCenter = new Coordinates();
            GPS.TrackPoint gpsPoint;
            for (int i = 0; i < gpsPoints.Count-1; i++ )
            {
                gpsPoint = gpsPoints[i];

                // Вычисляем направление и длину участка трека
                Angle trackDirection = gpsPoint.Coordinates.OrthodromicBearing(gpsPoints[i + 1].Coordinates);
                Distance trackLenght = gpsPoint.Coordinates.OrthodromicDistance(gpsPoints[i + 1].Coordinates);

                // Проверяем нахождение текущей точки трека в радиусе загруженного кеша участков
                if (cacheCenter.IsEmpty || cacheCenter.OrthodromicDistance(gpsPoint.Coordinates) + trackLenght + accuracy > cacheRange)
                {
                    // Устанавливаем центр кеша на текущую точку трека
                    cacheCenter = gpsPoint.Coordinates;
                    // Заново формируем кэш вокруг центральной точки
                    cacheLegs.Clear();
                    foreach (var leg in iwhLegs)
                    {
                        if (cacheCenter.OrthodromicDistance(leg.StartNode.Coordinates) <= cacheRange)
                            cacheLegs.Add(leg);
                    }
                }

                // Обсчитываем для каждой точки трека все находящиеся в кеше участки пути
                foreach (var leg in cacheLegs)
                {
                    // Проверяем удаление точки трека и начальной точки участка пути
                    Boolean legIsVisited = false;
                    Distance maxDistance = trackLenght + leg.Lenght; // ??? Дополнительно нужно учесть боковое отклонение
                    Distance factDistance = gpsPoint.Coordinates.OrthodromicDistance(leg.StartNode.Coordinates);
                    if (factDistance <= maxDistance + accuracy)
                    {
                        // Сравниваем направление участка пути с направление трека
                        Angle maxDeviation = MaximumVisibleDeviation; // ??? Дополнительно нужно учесть зависимость от длины участка трека
                        Angle factDeviation = (trackDirection - leg.Direction).Abs();
                        if ((factDeviation < maxDeviation) || (!leg.OneWay && ((Angle.Straight - factDeviation) < maxDeviation)))
                        {
                            // Проверяем боковое удаление точки трека от линии пути
                            Distance maxOffset = MaximumVisibleOffset; // ??? Дополнительно нужно учесть зависимость от длины участка трека
                            Angle factDirection = gpsPoint.Coordinates.OrthodromicBearing(leg.StartNode.Coordinates);
                            Distance factOffset = factDistance * Math.Abs((factDirection-leg.Direction).Sin());
                            if (factOffset < maxOffset)
                            {
                                 legIsVisited = true;
                            }
                        }
                    }
                    // Отмечаем участок как посещенный; обновляем время и количество посещений
                    if (legIsVisited)
                    {
                        leg.IsVisited = true;
                        if (gpsPoint.Time > leg.LastVisitedTime)
                        {
                            if ((gpsPoint.Time - leg.LastVisitedTime).TotalMinutes > visitedCountInerval)
                                leg.VisitedCount += 1;
                            leg.LastVisitedTime = gpsPoint.Time;
                        }
                    }

                } // leg
                
            } // i
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
                GPS.Track newTrack = new GPS.Track();
                // Определяем реквизиты трека
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.Id.ToString() + ")";
                if (way.OneWay)
                    newTrack.Name += " ONE_WAY ";
                if (way.Lanes > 0)
                    newTrack.Name += " L=" + way.Lanes.ToString();
                if (way.Surface > 0)
                    newTrack.Name += " " + way.Surface.ToString();
                if (way.Smoothness > 0)
                    newTrack.Name += " " + way.Smoothness.ToString();
                if (way.LastVisitedTime > DateTime.MinValue)
                    newTrack.Name += " " + way.LastVisitedTime.ToShortDateString();
                // Формируем сегменты
                GPS.TrackSegment newTrackSegment = new GPS.TrackSegment();
                for (int i = 0; i < legs.Count; i++)
                {
                    // Создаем новый сегмент для первого участка или
                    // если первая точка текущего участка не равна последней точке предыдущего
                    // Для первого участка добавляем обе точки, для остальных только конечную.
                    if (i == 0 || !legs[i].StartNode.Equals(legs[i-1].EndNode))
                    {
                        MapTpGps_PushSegment(newTrack, newTrackSegment);
                        newTrackSegment.Points.Add(new GPS.TrackPoint() { Coordinates = legs[i].StartNode.Coordinates });
                    }
                    newTrackSegment.Points.Add(new GPS.TrackPoint() { Coordinates = legs[i].EndNode.Coordinates });
                }
                //
                MapTpGps_PushSegment(newTrack, newTrackSegment);
                gpx.Tracks.Add(newTrack);
                legs.Clear();
            }
        }

        private static void MapTpGps_PushSegment(GPS.Track track, GPS.TrackSegment segment)
        {
            if (segment.Points.Count > 1)
                track.Segments.Add(segment);
            segment = new GPS.TrackSegment();
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
                if (node.UseCount > 2)
                {
                    newWayPoint = new GPS.WayPoint();
                    newWayPoint.Coordinates = node.Coordinates;
                    newWayPoint.Name = string.Format("{0} ({1})", node.Id, node.UseCount);
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

    }
}
