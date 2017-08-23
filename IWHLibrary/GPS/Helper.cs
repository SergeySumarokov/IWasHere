using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS
{
    abstract class Helper
    {

        /// <summary>
        /// Преобразует загруженный из файла трек в пригодный для анализа набор путей
        /// </summary>
        /// <param name="gpx"></param>
        /// <returns></returns>
        public List<Geography.GeoWay> TracksToGeoWays (List<GPS.Track> tracks)
        {
            var result = new List<Geography.GeoWay>();
            foreach (GPS.Track track in tracks)
            {
                foreach (GPS.TrackSegment trackSegment in track.Segments)
                {
                    result.Add(this.SegmentToWay(trackSegment));
                }
            }
            return result;
        }

        public Geography.GeoWay SegmentToWay (GPS.TrackSegment trackSegment)
        {
            var result = new Geography.GeoWay();
            Geography.GeoLeg newGeoLeg;
            for (int i = 0; i < trackSegment.Points.Count - 1; i++)
            {
                newGeoLeg = new Geography.GeoLeg();
                newGeoLeg.StartPoint = new Geography.GeoPoint() { Coordinates = trackSegment.Points[i].Coordinates };
                newGeoLeg.EndPoint = new Geography.GeoPoint() { Coordinates = trackSegment.Points[i+1].Coordinates };
                newGeoLeg.Recalculate();
                result.Legs.Add(newGeoLeg);
            }
            return result;
        }

        public Track Normalize(Track sourceTrack)
        {
            foreach (TrackSegment seg in sourceTrack.Segments)
            {
                foreach (TrackPoint pt in seg.Points)
                {

                }
            }
            return new Track();
        }




    }

    /// <summary>
    /// Представляет точку стоянки/остановки
    /// </summary>
    public class StopPoint : WayPoint
    {
        /// <summary>
        /// Время остановки
        /// </summary>
        public DateTime Duration;
    }
}
