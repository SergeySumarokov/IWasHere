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
            //IwhMap.LoadFromOsm(@"\Temp\IWasHere\RU-LEN.osm");
            //Console.WriteLine("Формирование выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            //Console.WriteLine("Длина {0}км", Math.Round(IwhMap.TotalLenght.Kilometers, 1));
            //Console.WriteLine("----------------");

            //// Удаляем из локальной базы точки вне области

            //Console.WriteLine("Удаление узлов вне границ области...");
            //stopwatch.Restart();
            //Console.WriteLine("Узлов до удаления {0}", IwhMap.Nodes.Count);
            //foreach (IWH.Node node in IwhMap.Nodes.Values.ToList())
            //{
            //    if (areaSpb.HasPointInside(node.Coordinates) || !areaLen.HasPointInside(node.Coordinates))
            //        IwhMap.Nodes.Remove(node.Id);
            //}
            //IwhMap.PackNodes();
            //Console.WriteLine("Удаление выполнено за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Узлов после удаления {0}", IwhMap.Nodes.Count);
            //Console.WriteLine("----------------");

            //// Удаляем НП с населением менее 2048

            //Console.WriteLine("Контроль населенных пунктов...");
            //Console.WriteLine("Узлов до удаления {0}", IwhMap.Nodes.Count);
            //stopwatch.Restart();
            //foreach (var node in IwhMap.Nodes.Values.ToList())
            //{
            //    if (node.Type == IWH.NodeType.Village && node.Population < 2014)
            //        IwhMap.Nodes.Remove(node.Id);
            //}
            //Console.WriteLine("Контроль выполнен за {0} мсек", stopwatch.ElapsedMilliseconds);
            //Console.WriteLine("Узлов после удаления {0}", IwhMap.Nodes.Count);
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
            Console.WriteLine("Загрузка выполнена за {0} мсек",stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Линий {0}, Узлов {1}", IwhMap.Ways.Count, IwhMap.Nodes.Count);
            Console.WriteLine("Длина {0}км, Посещено {1}км ({2}%)", 
                Math.Round(IwhMap.TotalLenght.Kilometers,1), 
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers,1), 
                Math.Round(IwhMap.TotalVisitedLenght.Kilometers / IwhMap.TotalLenght.Kilometers * 100, 2));
            Console.WriteLine("----------------");

            // Анализируем треки

            DirectoryInfo trackFolder = new DirectoryInfo(@"\Projects\IWasHere\Resources\Tracks");
            FileInfo[] trackFiles;
            trackFiles = trackFolder.GetFiles("*.gpx");
            // Обрабатываем треки
            stopwatch.Restart();
            foreach (FileInfo trackFile in trackFiles)
            {
                Console.WriteLine("Анализ файла {0}", trackFile.Name);
                GPS.Gpx gpxTrack = GPS.Gpx.FromXmlFile(trackFile.FullName);
                AnalizeGpsTrack(IwhMap.Nodes.Values.ToList(), gpxTrack.GetPointList(), new Distance(4, Distance.Unit.Kilometers));
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
            MapToGpxByVisited(IwhMap, false, @"\Projects\IWasHere\Resources\Unvisited.gpx");
            MapToGpxByVisited(IwhMap, true, @"\Projects\IWasHere\Resources\Visited.gpx");
            stopwatch.Stop();
            Console.WriteLine("Выгрузка выполнена за {0} мсек", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("----------------");

            // Конец

            Console.WriteLine("Работа завершена. Нажмите [Enter] для выхода.");
            Console.ReadLine();
        }

        /// <summary>
        /// Анализирует список точек GPS и отмечает IsVisible у точек IWH.
        /// </summary>
        /// <remarks>
        /// Предполагалается, что точки в треке, расположены в порядке времени следования по маршруту.
        /// Точка на карте считается посещенной, если её удаление от отрезка, заданного двумя точками трека
        /// менее значения, установленного как "зона видимости" точки.
        /// Для оптимизации работа проводится с набором точек карты, расположенном в заданном радиусе от точки трека.
        /// </remarks>
        static void AnalizeGpsTrack(List<IWH.Node> iwhNodes, List<GPS.TrackPoint> gpsPoints, Distance cacheRange)
        {
            var cacheNodes = new List<IWH.Node>();
            var cacheCenter = new Coordinates();
            GPS.TrackPoint point;
            for (int i = 0; i < gpsPoints.Count-1; i++ )
            {
                point = gpsPoints[i];
                
                // Проверяем нахождение текущей точки трека в радиусе загруженного кеша точек
                if (cacheCenter.IsEmpty || cacheCenter.OrthodromicDistance(point.Coordinates) > cacheRange)
                {
                    // Устанавливаем центр кеша на текущую точку трека
                    cacheCenter = point.Coordinates;
                    // Заново формируем кэш вокруг центральной точки
                    cacheNodes.Clear();
                    foreach (var node in iwhNodes)
                    {
                        if (cacheCenter.OrthodromicDistance(node.Coordinates) <= cacheRange)
                            cacheNodes.Add(node);
                    }
                }

                // Вычисляем направление и длину участка трека
                Angle trackBearing = point.Coordinates.OrthodromicBearing(gpsPoints[i + 1].Coordinates);
                Distance trackDistance = point.Coordinates.OrthodromicDistance(gpsPoints[i + 1].Coordinates);
                // Проверяем удаление от участка трека каждой точки карты в кеше
                foreach (var node in cacheNodes)
                {
                    Boolean nodeIsVisited = false;

                    // Для каждой точки трека проверяем удаление точки карты от точки трека
                    Distance nodeDistance = point.Coordinates.OrthodromicDistance(node.Coordinates);
                    if (nodeDistance <= node.Range)
                        nodeIsVisited = true;

                    // Для всех точек трека, кроме последней, проверяем удаление точки карты от линии трека
                    if (!nodeIsVisited && i < gpsPoints.Count-1)
                    {
                        // Вычисляем азимут точки карты
                        Angle nodeBearing = point.Coordinates.OrthodromicBearing(node.Coordinates);
                        // Если разность углов меньше 90гр, значить проекция точки попадает на отрезок
                        Angle diffBearing = (nodeBearing - trackBearing).Abs();
                        if (diffBearing < Angle.Right)
                        {
                            // Если удаление точки пересечения проекции с отрезком меньше длины отрезка, то точка может подойти
                            Distance nodeProjection = nodeDistance * diffBearing.Cos();
                            if (nodeProjection < trackDistance)
                            {
                                // Если удаление точки от от отрезка пути меньше максимально-допустимой, то точка подходит
                                Distance nodeOffset = nodeDistance * diffBearing.Sin();
                                if (nodeOffset <= node.Range)
                                    nodeIsVisited = true;
                            }
                        }
                    }

                    // Отмечаем точку как посещенную; обновляем время и количество посещений
                    if (nodeIsVisited)
                    {
                        node.IsVisited = true;
                        if (point.Time > node.LastVisitedTime)
                        {
                            if ((point.Time - node.LastVisitedTime).TotalMinutes > 5)
                                node.VisitedCount += 1;
                            node.LastVisitedTime = point.Time;
                        }
                    }

                } // node
                
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
            var nodes = new List<IWH.Node>();
            // Подготавливаем линии для выгрузки
            foreach (IWH.Way way in wayList)
            {
                for (int i = 0; i < way.Nodes.Count; i++)
                {
                    IWH.Node node = way.Nodes[i];
                    if (!visitedOnly || node.IsVisited)
                    {
                        nodes.Add(node);
                    }
                    if ((visitedOnly && !node.IsVisited) || (i == way.Nodes.Count-1))
                    {
                        MapToGps_PushTrack(gpx, way, nodes);
                    }
                }
            }
            // Выгружаем в файл
            gpx.SaveToFile(outputFileName);
        }

        private static void MapToGps_PushTrack(GPS.Gpx gpx, IWH.Way way, List<IWH.Node> nodes)
        {
            if (nodes.Count>1)
            {
                GPS.Track newTrack = new GPS.Track();
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.Id.ToString() + ")";
                if (way.Surface > 0)
                    newTrack.Name += " " + way.Surface.ToString();
                if (way.Smoothness > 0)
                    newTrack.Name += " " + way.Smoothness.ToString();
                if (way.LastVisitedTime > DateTime.MinValue)
                    newTrack.Name += " " + way.LastVisitedTime.ToShortDateString();
                GPS.TrackSegment newTrackSegment = new GPS.TrackSegment();
                foreach (IWH.Node node in nodes)
                {
                    GPS.TrackPoint newTrackPoint = new GPS.TrackPoint();
                    newTrackPoint.Coordinates = node.Coordinates;
                    newTrackSegment.Points.Add(newTrackPoint);
                }
                newTrack.Segments.Add(newTrackSegment);
                gpx.Tracks.Add(newTrack);
            }
            nodes.Clear();
        }

        /// <summary>
        /// Выгружает заданный список линий в файл GPS-трекинга
        /// </summary>
        /// <param name="wayList"></param>
        /// <param name="visitedOnly">Выгружать только посещенные участки линии</param>
        /// <param name="outputFileName"></param>
        static void MapToGpxOld(List<IWH.Way> wayList, Boolean visitedOnly, string outputFileName)
        {
            GPS.Gpx gpx = new GPS.Gpx();
            GPS.Track newTrack;
            GPS.TrackSegment newTrackSegment;
            GPS.TrackPoint newTrackPoint;
            bool goodNode;
            // Подготавливаем линии для выгрузки
            foreach (IWH.Way way in wayList)
            {
                newTrack = new GPS.Track();
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.Id.ToString() + ")";
                if (way.Surface > 0)
                    newTrack.Name += " " + way.Surface.ToString();
                if (way.Smoothness > 0)
                    newTrack.Name += " " + way.Smoothness.ToString();
                if (way.LastVisitedTime > DateTime.MinValue)
                    newTrack.Name += " " + way.LastVisitedTime.ToShortDateString();
                newTrackSegment = new GPS.TrackSegment();
                for (int i = 0; i < way.Nodes.Count; i++)
                {
                    if (visitedOnly)
                        // Выгружаем посещенную точку только если следующая или предыдущая так-же посещена
                        goodNode = ((way.Nodes[i].IsVisited) && ((i == 0 || way.Nodes[i - 1].IsVisited) || (i == way.Nodes.Count - 1 || way.Nodes[i + 1].IsVisited)));
                    else
                        goodNode = true;
                    if (goodNode)
                    {
                        newTrackPoint = new GPS.TrackPoint();
                        newTrackPoint.Coordinates = way.Nodes[i].Coordinates;
                        newTrackSegment.Points.Add(newTrackPoint);
                    }
                }
                if (newTrackSegment.Points.Count >= 2)
                {
                    newTrack.Segments.Add(newTrackSegment);
                    gpx.Tracks.Add(newTrack);
                }
            }
            // Выгружаем в файл
            gpx.SaveToFile(outputFileName);
        }

        /// <summary>
        /// Выгружает линии и точки в файл GPS-трекинга
        /// </summary>
        /// <param name="visitedOnly">Выгружать только посещенные участки линии</param>
        /// <param name="outputFileName"></param>
        static void MapToGpxByVisited(IWH.Map map, Boolean visitedOnly, string outputFileName)
        {
            GPS.Gpx gpx = new GPS.Gpx();
            GPS.Track newTrack;
            GPS.TrackSegment newTrackSegment;
            GPS.TrackPoint newTrackPoint;
            GPS.WayPoint newWayPoint;
            bool goodNode;
            // Подготавливаем линии для выгрузки
            foreach (IWH.Way way in map.Ways.Values)
            {
                newTrack = new GPS.Track();
                newTrack.Name = way.Name + " (" + way.Type.ToString() + " " + way.Id.ToString() + ")";
                newTrackSegment = new GPS.TrackSegment();
                for (int i = 0; i < way.Nodes.Count; i++)
                {
                    // Выгружаем посещенную точку только если следующая или предыдущая так-же посещена
                    if (visitedOnly)
                        goodNode = ((way.Nodes[i].IsVisited) && ((i == 0 || way.Nodes[i - 1].IsVisited) || (i == way.Nodes.Count - 1 || way.Nodes[i + 1].IsVisited)));
                    else
                        goodNode = ((!way.Nodes[i].IsVisited) && ((i == 0 || !way.Nodes[i - 1].IsVisited) || (i == way.Nodes.Count - 1 || !way.Nodes[i + 1].IsVisited)));
                    if (goodNode)
                    {
                        newTrackPoint = new GPS.TrackPoint();
                        newTrackPoint.Coordinates = way.Nodes[i].Coordinates;
                        newTrackSegment.Points.Add(newTrackPoint);
                    }
                }
                if (newTrackSegment.Points.Count >= 2)
                {
                    newTrack.Segments.Add(newTrackSegment);
                    gpx.Tracks.Add(newTrack);
                }
            }
            // Подготавливаем точки для выгрузки
            var goodNodeTypeList = new List<IWH.NodeType> { IWH.NodeType.City, IWH.NodeType.Town, IWH.NodeType.Village };
            foreach (IWH.Node node in map.Nodes.Values)
            {
                if (goodNodeTypeList.Contains(node.Type))
                {
                    if (visitedOnly)
                        goodNode = node.IsVisited;
                    else
                        goodNode = !node.IsVisited;
                    if (goodNode)
                    {
                        newWayPoint = new GPS.WayPoint();
                        newWayPoint.Coordinates = node.Coordinates;
                        newWayPoint.Name = string.Format("{0} {1}K",node.Name,Math.Round(node.Population/1000.0));
                        gpx.WayPoints.Add(newWayPoint);
                    }
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
