using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primitives;

namespace IWH
{
    /// <summary>
    /// Накапливает передаваемую информацию о затраченном времени и пройденном пути, вычисляя среднюю скорость движения.
    /// </summary>
    public class AverageSpeedCounter
    {
        Time _timeInterval;
        List<Time> timeList = new List<Time>();
        List<Distance> distList = new List<Distance>();

        public Time TotalTime { get; private set; }
        public Distance TotalDistance { get; private set; }

        public AverageSpeedCounter(Time timeInterval)
        {
            _timeInterval = timeInterval;
            TotalTime = Time.Zero;
            TotalDistance = Distance.Zero;
        }

        /// <summary>
        /// Добавляет в массив информацию о времени и расстоянии прохождения очередного участка.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="distance"></param>
        public void Add(Time time, Distance distance)
        {
            if (time > Time.Zero && distance > Distance.Zero)
            {
                // Добавляем в массив расстояние и время
                timeList.Add(time);
                TotalTime += time;
                distList.Add(distance);
                TotalDistance += distance;
                // Уменьшаем размер массива до необходимого
                while (timeList.Count > 1 && TotalTime > _timeInterval)
                {
                    TotalTime -= timeList[0];
                    TotalDistance -= distList[0];
                    timeList.RemoveAt(0);
                    distList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Возвращает текущую среднюю скорость
        /// </summary>
        /// <returns></returns>
        public Speed GetAverageSpeed()
        {
            if (TotalTime > Time.Zero && TotalDistance > Distance.Zero)
                return TotalDistance / TotalTime;
            else
                return Speed.Zero;
        }

    }
}
