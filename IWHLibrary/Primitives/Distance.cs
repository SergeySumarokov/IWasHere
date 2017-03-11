using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженное в радианах расстояние на поверхности земного сфероида.
    /// </summary>
    public struct Distance : IComparable<Distance>, IEquatable<Distance>
    {

        /// <summary>
        /// Значение расстояния (рад).
        /// </summary>
        private double Value;

        /// <summary>
        /// Представляет нулевое значение расстояния.
        /// </summary>
        public static readonly Distance Zero = new Distance(0);

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения расстояния.
        /// </summary>
        /// <remarks></remarks>
        public enum Unit : int
        {
            Radians = 0,
            Meters = 1,
            Kilometers = 2,
            NauticalMiles = 3,
            StatuteMiles = 4
        }

        private static double[] _earthRadius = {
            1.0,
            6378137.0,
            6378.137,
            3443.918,
            3964.038

        };

        /// <summary>
        /// Возвращает радиус земли в указанных единицах измерения.
        /// </summary>
        /// <param name="unit">Единица измерения</param>
        static internal double GetEarthRadius(Unit unit)
        {
            return _earthRadius[(int)unit];
        }

        /// <summary>
        /// Возвращает расстояние в указанных единицах измерения.
        /// </summary>
        /// <param name="unit">Единица измерения</param>
        private double GetValue(Unit unit)
        {
            return this.Value * _earthRadius[(int)unit];
        }

        /// <summary>
        /// Устанавливает расстояние в указанных единицах измерения.
        /// </summary>
        /// <param name="value">Значение расстояния</param>
        /// <param name="unit">Единица измерения</param>
        private void SetValue(double value, Unit unit)
        {
            this.Value = value / _earthRadius[(int)unit];
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает угловое расстояние в радианах.
        /// </summary>
        public double Radians
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает расстояние в метрах.
        /// </summary>
        public double Meters
        {
            get { return this.GetValue(Unit.Meters); }
            set { this.SetValue(value, Unit.Meters); }
        }

        /// <summary>
        /// Возвращает/устанавливает расстояние в километрах.
        /// </summary>
        public double Kilometers
        {
            get { return this.GetValue(Unit.Kilometers); }
            set { this.SetValue(value, Unit.Kilometers); }
        }

        /// <summary>
        /// Возвращает/устанавливает расстояние в морских милях.
        /// </summary>
        public double NauticalMiles
        {
            get { return this.GetValue(Unit.NauticalMiles); }
            set { this.SetValue(value, Unit.NauticalMiles); }
        }

        /// <summary>
        /// Возвращает/устанавливает расстояние в сухопутных милях.
        /// </summary>
        public double StatuteMiles
        {
            get { return this.GetValue(Unit.StatuteMiles); }
            set { this.SetValue(value, Unit.StatuteMiles); }
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
        /// Инициализирует структуру значением расстояния в радианах.
        /// </summary>
        /// <param name="value">Значение расстояния (рад)</param>
        private Distance(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру заданных значением в указанных единицах измерения.
        /// </summary>
        /// <param name="value">Значение расстояния</param>
        /// <param name="unit">Единица измерения</param>
        public Distance(double value, Unit unit)
        {
            this.Value = 0.0;
            this.SetValue(value, unit);
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество метров.
        /// </summary>
        /// <param name="value">Количество метров</param>
        public static Distance FromMeters(double value)
        {
            return new Distance(value, Unit.Meters);
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество километров.
        /// </summary>
        /// <param name="value">Количество километров</param>
        public static Distance FromKilometers(double value)
        {
            return new Distance(value, Unit.Kilometers);
        }

        #endregion

        #region "Операторы"

        public static Distance operator +(Distance distance1, Distance distance2)
        {
            return new Distance(distance1.Value + distance2.Value);
        }

        public static Distance operator -(Distance distance1, Distance distance2)
        {
            return new Distance(distance1.Value - distance2.Value);
        }

        public static Distance operator *(Distance distance, double value)
        {
            return new Distance(distance.Value * value);
        }

        public static double operator /(Distance distance1, Distance distance2)
        {
            return (distance1.Value / distance2.Value);
        }

        public static Distance operator /(Distance distance, double value)
        {
            return new Distance(distance.Value / value);
        }

        public static Time operator /(Distance distance, Speed speed)
        {
            return new Time(distance.Meters / speed.MetersPerSecond, Time.Unit.Second);
        }

        public static Speed operator /(Distance distance, Time time)
        {
            return new Speed(distance.Meters / time.Seconds, Speed.Unit.MetersPerSecond);
        }

        public static bool operator ==(Distance distance1, Distance distance2)
        {
            return distance1.Value == distance2.Value;
        }

        public static bool operator !=(Distance distance1, Distance distance2)
        {
            return distance1.Value != distance2.Value;
        }

        public static bool operator >(Distance distance1, Distance distance2)
        {
            return distance1.Value > distance2.Value;
        }

        public static bool operator <(Distance distance1, Distance distance2)
        {
            return distance1.Value < distance2.Value;
        }

        public static bool operator >=(Distance distance1, Distance distance2)
        {
            return distance1.Value >= distance2.Value;
        }

        public static bool operator <=(Distance distance1, Distance distance2)
        {
            return distance1.Value <= distance2.Value;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Distance distance)
        {
            return this.Value.CompareTo(distance.Value);
        }

        public bool Equals(Distance distance)
        {
            return this.Value.Equals(distance.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Distance)
            {
                return this.Equals((Distance)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}km", this.Kilometers);
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
        public Distance Abs()
        {
            return new Distance(Math.Abs(this.Value));
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>

        public const double Exactitude = 1E-10;
        /// <summary>
        /// Возвращает истину, если переданное расстояние равно текущему с учетом погрешности типа данных.
        /// </summary>
        /// <param name="distance">Расстояние</param>
        public bool AlmostEquals(Distance distance)
        {
            return Math.Abs(this.Value - distance.Value) < Distance.Exactitude;
        }

        #endregion

    }

}
