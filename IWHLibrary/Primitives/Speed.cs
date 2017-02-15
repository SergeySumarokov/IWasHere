using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует значение скорости, выраженное в метрах в секунду.
    /// </summary>
    public struct Speed : IComparable<Speed>, IEquatable<Speed>
    {

        /// <summary>
        /// Скорость (м/с).
        /// </summary>
        private double Value;

        /// <summary>
        /// Представляет нулевое значение скорости.
        /// </summary>
                public static readonly Speed Zero = new Speed(0);
        
        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения скорости.
        /// </summary>
        public enum Unit : int
        {
            MetersPerSecond = 0,
            KilometersPerHour = 1,
            FeetPerMinute = 2,
            Knots = 3
        }

        private const double _convertToKmh = 3.6;
        private const double _convertToFpm = 196.8504;
        private const double _convertToKnt = 1.943844;
        private const double _convertKntToKmh = 1.852;

        public static double MpsToKmh(double value)
        {
            return value * _convertToKmh;
        }

        public static double KmhToMps(double value)
        {
            return value / _convertToKmh;
        }

        public static double MpsToFpm(double value)
        {
            return value * _convertToFpm;
        }

        public static double FpmToMps(double value)
        {
            return value / _convertToFpm;
        }

        public static double MpsToKnt(double value)
        {
            return value * _convertToKnt;
        }

        public static double KntToMps(double value)
        {
            return value / _convertToKnt;
        }

        public static double KntToKmh(double value)
        {
            return value * _convertKntToKmh;
        }

        public static double KmhToKnt(double value)
        {
            return value / _convertKntToKmh;
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение скорости в метрах в секунду.
        /// </summary>
        public double MetersPerSecond
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение скорости в километрах в час.
        /// </summary>
        public double KilometersPerHour
        {
            get { return MpsToKmh(this.Value); }
            set { this.Value = KmhToMps(value); }
        }

        /// <summary>
        /// Возвращает/устанавливает значение скорости в узлах.
        /// </summary>
        public double Knots
        {
            get { return MpsToKnt(this.Value); }
            set { this.Value = KntToMps(value); }
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
        /// Инициализирует структуру значением скорости.
        /// </summary>
        /// <param name="value">Значение скорости (м/с)</param>
        private Speed(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру значением скорости в заданных единицах измерения.
        /// </summary>
        /// <param name="value">Значение скорости</param>
        /// <param name="unit">Единица измерения</param>
        public Speed(double value, Unit unit)
        {
            this.Value = 0.0;
            switch (unit)
            {
                case Unit.MetersPerSecond:
                    this.MetersPerSecond = value;
                    break;
                case Unit.KilometersPerHour:
                    this.KilometersPerHour = value;
                    break;
                case Unit.Knots:
                    this.Knots = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное значение метров в секунду.
        /// </summary>
        /// <param name="value">Метры в секунду</param>
        public static Speed FromMetersPerSecond(double value)
        {
            return new Speed(value, Unit.MetersPerSecond);
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное значение километров в час.
        /// </summary>
        /// <param name="value">Километры в час</param>
        public static Speed FromKilometersPerHour(double value)
        {
            return new Speed(value, Unit.KilometersPerHour);
        }

        #endregion

        #region "Операторы"

        public static Speed operator +(Speed speed1, Speed speed2)
        {
            return new Speed(speed1.Value + speed2.Value);
        }

        public static Speed operator -(Speed speed1, Speed speed2)
        {
            return new Speed(speed1.Value - speed2.Value);
        }

        public static Speed operator *(Speed speed, double value)
        {
            return new Speed(speed.Value * value);
        }

        public static Distance operator *(Speed speed, Time time)
        {
            return new Distance(speed.MetersPerSecond * time.Seconds, Distance.Unit.Meters);
        }

        public static Speed operator /(Speed speed, double value)
        {
            return new Speed(speed.Value / value);
        }

        public static Time operator /(Speed speed, Celeration acceleration)
        {
            return new Time(speed.MetersPerSecond / acceleration.MetersPerSecondSquared, Time.Unit.Second);
        }

        public static bool operator ==(Speed speed1, Speed speed2)
        {
            return speed1.Value == speed2.Value;
        }

        public static bool operator !=(Speed speed1, Speed speed2)
        {
            return speed1.Value != speed2.Value;
        }

        public static bool operator <(Speed speed1, Speed speed2)
        {
            return speed1.Value < speed2.Value;
        }

        public static bool operator <=(Speed speed1, Speed speed2)
        {
            return speed1.Value <= speed2.Value;
        }

        public static bool operator >(Speed speed1, Speed speed2)
        {
            return speed1.Value > speed2.Value;
        }
        public static bool operator >=(Speed speed1, Speed speed2)
        {
            return speed1.Value >= speed2.Value;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Speed speed)
        {
            return this.Value.CompareTo(speed.Value);
        }

        public bool Equals(Speed speed)
        {
            return this.Value.Equals(speed.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Speed)
            {
                return this.Equals((Speed)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}mps", this.MetersPerSecond);
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
        public static Speed Abs(Speed speed)
        {
            return new Speed(Math.Abs(speed.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public Speed Abs()
        {
            return Speed.Abs(this);
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую максимальное значение.
        /// </summary>
        public static Speed Max(Speed speed1, Speed speed2)
        {
            return new Speed(Math.Max(speed1.Value, speed2.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую минимальное значение.
        /// </summary>
        public static Speed Min(Speed speed1, Speed speed2)
        {
            return new Speed(Math.Min(speed1.Value, speed2.Value));
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>

        public const double Exactitude = 1E-10;
        /// <summary>
        /// Возвращает истину, если переданная скорость равна текущей с учетом погрешности типа данных.
        /// </summary>
        /// <param name="speed">Скорость</param>
        public bool AlmostEquals(Speed speed)
        {
            return Math.Abs(this.Value - speed.Value) < Speed.Exactitude;
        }

        #endregion

        #region "Предметная область"

        /// <summary>
        /// Возвращает истинную скорость по Высоте полета и Приборной скорости
        /// </summary>
        /// <param name="altitude">Высота</param>
        /// <param name="airSpeed">Приборная скорость</param>
        /// <returns>Истинная скорость</returns>
        public static Speed GetTrueByIndicated(Altitude altitude, Speed airSpeed)
        {
            return new Speed(airSpeed.KilometersPerHour * (1 + (altitude.Meters / 15240)), Unit.KilometersPerHour);
        }

        #endregion

    }

}
