using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженный в секундах интервал времени.
    /// </summary>
    public struct Time : IComparable<Time>, IEquatable<Time>
    {

        /// <summary>
        /// Интервал времени (с).
        /// </summary>
        private double Value;
        /// <summary>
        /// Представляет нулевое значение времени.
        /// </summary>

        public static readonly Time Zero = new Time(0);

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения времени.
        /// </summary>
        public enum Unit : int
        {
            Milliseconds = -1,
            Second = 0,
            Minute = 1,
            Hour = 2
        }

        private const double _convertToMilliseconds = 0.001;
        private const double _convertToMinutes = 60.0;
        private const double _convertToHours = 3600.0;

        public static double SecondsToMilliseconds(double value)
        {
            return value / _convertToMilliseconds;
        }

        public static double MillisecondsToSeconds(double value)
        {
            return value * _convertToMilliseconds;
        }

        public static double SecondsToMinutes(double value)
        {
            return value / _convertToMinutes;
        }

        public static double MinutesToSeconds(double value)
        {
            return value * _convertToMinutes;
        }

        public static double SecondsToHours(double value)
        {
            return value / _convertToHours;
        }

        public static double HoursToSeconds(double value)
        {
            return value * _convertToHours;
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение интервала времени в миллисекундах.
        /// </summary>
        public double Milliseconds
        {
            get { return Time.SecondsToMilliseconds(this.Value); }
            set { this.Value = Time.MillisecondsToSeconds(value); }
        }

        /// <summary>
        /// Возвращает значение интервала времени в миллисекундах как 32-битное целое.
        /// </summary>
        /// <remarks>
        /// Возвращает Int32.MinValue или Int32.MaxValue если текущее значение интервала времени вне допустимого для Int32.
        /// </remarks>
        public int MillisecondsInt32
        {
            get
            {
                dynamic milliseconds = this.Milliseconds;
                if (milliseconds < int.MinValue)
                {
                    return int.MinValue;
                }
                else if (milliseconds > int.MaxValue)
                {
                    return int.MaxValue;
                }
                else {
                    return Convert.ToInt32(this.Milliseconds);
                }
            }
        }

        /// <summary>
        /// Возвращает/устанавливает значение интервала времени в секундах.
        /// </summary>
        public double Seconds
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение интервала времени в минутах.
        /// </summary>
        public double Minutes
        {
            get { return Time.SecondsToMinutes(this.Value); }
            set { this.Value = Time.MinutesToSeconds(value); }
        }

        /// <summary>
        /// Возвращает/устанавливает значение интервала времени в часах.
        /// </summary>
        public double Hours
        {
            get { return Time.SecondsToHours(this.Value); }
            set { this.Value = Time.HoursToSeconds(value); }
        }

        /// <summary>
        /// Возвращает Истину, если значение не задано.
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.Value == 0); }
        }

        /// <summary>
        /// Возвращает Истину, если значение больше нуля.
        /// </summary>
        public bool IsPositive
        {
            get { return (this.Value > 0); }
        }

        /// <summary>
        /// Возвращает Истину, если значение меньше нуля.
        /// </summary>
        public bool IsNegative
        {
            get { return (this.Value < 0); }
        }

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует структуру значением интервала времени в секундах.
        /// </summary>
        /// <param name="value">Количество секунд</param>
        private Time(double value)
        {
            this.Seconds = value;
        }

        /// <summary>
        /// Инициализирует структуру значением интервала времени в заданных единицах измерения.
        /// </summary>
        /// <param name="value">Интервал времени</param>
        /// <param name="unit">Единица измерения</param>
        public Time(double value, Unit unit)
        {
            switch (unit)
            {
                case Unit.Milliseconds:
                    this.Milliseconds = value;
                    break;
                case Unit.Second:
                    this.Seconds = value;
                    break;
                case Unit.Minute:
                    this.Minutes = value;
                    break;
                case Unit.Hour:
                    this.Hours = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество минут.
        /// </summary>
        /// <param name="value">Количество минут</param>
        public static Time FromMinutes(double value)
        {
            return new Time(value, Unit.Minute);
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество часов.
        /// </summary>
        /// <param name="value">Количество часов</param>
        public static Time FromHours(double value)
        {
            return new Time(value, Unit.Hour);
        }

        #endregion

        #region "Операторы"

        public static Time operator +(Time time1, Time time2)
        {
            return new Time(time1.Value + time2.Value);
        }

        public static Time operator -(Time time1, Time time2)
        {
            return new Time(time1.Value - time2.Value);
        }

        public static Time operator *(Time time, double value)
        {
            return new Time(time.Value * value);
        }

        public static Distance operator *(Time time, Speed speed)
        {
            return new Distance(time.Seconds * speed.MetersPerSecond, Distance.Unit.Meters);
        }

        public static Time operator /(Time time, double value)
        {
            return new Time(time.Value / value);
        }

        public static bool operator ==(Time time1, Time time2)
        {
            return time1.Value == time2.Value;
        }

        public static bool operator !=(Time time1, Time time2)
        {
            return time1.Value != time2.Value;
        }

        public static bool operator <(Time time1, Time time2)
        {
            return time1.Value < time2.Value;
        }

        public static bool operator >(Time time1, Time time2)
        {
            return time1.Value > time2.Value;
        }

        public static explicit operator TimeSpan(Time time)
        {
            if (time.Value < TimeSpan.MinValue.TotalSeconds || time.Value > TimeSpan.MaxValue.TotalSeconds)
            {
                throw new InvalidCastException();
            }
            else {
                return TimeSpan.FromSeconds(time.Value);
            }
        }

        public static implicit operator Time(TimeSpan timeSpan)
        {
            return new Time(timeSpan.TotalSeconds);
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Time time)
        {
            return this.Value.CompareTo(time.Value);
        }

        public bool Equals(Time time)
        {
            return this.Value.Equals(time.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Time)
            {
                return this.Equals((Time)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}s", this.Seconds);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

        #region "Функции и процедуры"

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public static Time Abs(Time time)
        {
            return new Time(Math.Abs(time.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public Time Abs()
        {
            return Time.Abs(this);
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>

        public const double Exactitude = 1E-10;
        /// <summary>
        /// Возвращает истину, если переданный интервал времени равен текущему с учетом погрешности типа данных.
        /// </summary>
        /// <param name="time">Интервала времени</param>
        public bool AlmostEquals(Time time)
        {
            return Math.Abs(this.Value - time.Value) < Time.Exactitude;
        }

        #endregion

    }

    /// <summary>
    /// Инкапсулирует число тактов временного механизма.
    /// </summary>
    public struct TimeStamp
    {

        /// <summary>
        /// Значение.
        /// </summary>
        public long Value;

        /// <summary>
        /// Устанавливает для значения структуры текущее число тактов временного механизма.
        /// </summary>
        public void SetNow()
        {
            this.Value = System.Diagnostics.Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Возвращает новую структуру со значением текущего число тактов временного механизма.
        /// </summary>
        public static TimeStamp GetNow()
        {
            TimeStamp result = new TimeStamp();
            result.SetNow();
            return result;
        }

        private static double GetTimeStampDiff(TimeStamp timeStart, TimeStamp timeStop)
        {
            return (timeStop.Value - timeStart.Value) / System.Diagnostics.Stopwatch.Frequency;
        }

        /// <summary>
        /// Возвращает разницу между двумя значениями тактов временного механизма в миллисекундах.
        /// </summary>
        /// <param name="timeStart">Начальное значение тактов</param>
        /// <param name="timeStop">Конечное значение тактов</param>
        /// <returns>Разницу между текущим и заданным значениями тактов</returns>
        public static int GetDiff(TimeStamp timeStart, TimeStamp timeStop)
        {
            double diff = GetTimeStampDiff(timeStart, timeStop) * 1000;
            if (diff > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (diff < int.MinValue)
            {
                return int.MinValue;
            }
            else {
                return Convert.ToInt32(diff);
            }
        }

        /// <summary>
        /// Возвращает разницу между хранимым и заданным значениями тактов временного механизма в миллисекундах.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetDiff(TimeStamp timeStamp)
        {
            return TimeStamp.GetDiff(this, timeStamp);
        }

        /// <summary>
        /// Возвращает разницу между хранимым и текущим значениями тактов временного механизма в миллисекундах.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetDiff()
        {
            return this.GetDiff(TimeStamp.GetNow());
        }

        /// <summary>
        /// Возвращает время между хранимым и текущим значениями тактов временного механизма.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Time GetDiffTime()
        {
            return new Time(TimeStamp.GetTimeStampDiff(this, TimeStamp.GetNow()), Time.Unit.Second);
        }

        #region "Интерфейсы и Переопределения"

        public override string ToString()
        {
            return string.Format("{0}", this.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

    }

}
