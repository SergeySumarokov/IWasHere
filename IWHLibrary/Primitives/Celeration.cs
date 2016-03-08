using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует значение ускорения, выраженное в метрах в секунду в квадрате.
    /// </summary>
    public struct Celeration : IComparable<Celeration>, IEquatable<Celeration>
    {

        /// <summary>
        /// Ускорение (м/с2).
        /// </summary>
        private double Value;

        /// <summary>
        /// Представляет нулевое значение ускорения.
        /// </summary>
        public static readonly Celeration Zero = new Celeration(0);

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения ускорения.
        /// </summary>
        public enum Unit : int
        {
            MetersPerSecondSquared = 0,
            KilometersPerHourSquared = 1,
            FeetPerMinuteSquared = 2,
            KnotsSquared = 3
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
        /// Возвращает/устанавливает значение ускорения в метрах в секунду в квадрате.
        /// </summary>
        public double MetersPerSecondSquared
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение ускорения в километрах в час в квадрате.
        /// </summary>
        public double KilometersPerHourSquared
        {
            get { return MpsToKmh(this.Value); }
            set { this.Value = KmhToMps(value); }
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
        /// Инициализирует структуру значением ускорения.
        /// </summary>
        /// <param name="value">Значение ускорения (м/с2)</param>
        private Celeration(double value)
        {
            this.MetersPerSecondSquared = value;
        }

        /// <summary>
        /// Инициализирует структуру значением ускорения в заданных единицах измерения.
        /// </summary>
        /// <param name="value">Значение ускоренияч</param>
        /// <param name="unit">Единица измерения</param>
        public Celeration(double value, Unit unit)
        {
            switch (unit)
            {
                case Unit.MetersPerSecondSquared:
                    this.MetersPerSecondSquared = value;
                    break;
                case Unit.KilometersPerHourSquared:
                    this.KilometersPerHourSquared = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        #endregion

        #region "Операторы"

        public static Celeration operator +(Celeration acceleration1, Celeration acceleration2)
        {
            return new Celeration(acceleration1.Value + acceleration2.Value);
        }

        public static Celeration operator -(Celeration acceleration1, Celeration acceleration2)
        {
            return new Celeration(acceleration1.Value - acceleration2.Value);
        }

        public static Celeration operator *(Celeration acceleration, double value)
        {
            return new Celeration(acceleration.Value * value);
        }

        public static Speed operator *(Celeration acceleration, Time time)
        {
            return new Speed(acceleration.MetersPerSecondSquared * time.Seconds, Speed.Unit.MetersPerSecond);
        }

        public static Celeration operator /(Celeration acceleration, double value)
        {
            return new Celeration(acceleration.Value / value);
        }

        public static bool operator ==(Celeration acceleration1, Celeration acceleration2)
        {
            return (acceleration1.Value == acceleration2.Value);
        }

        public static bool operator !=(Celeration acceleration1, Celeration acceleration2)
        {
            return (acceleration1.Value != acceleration2.Value);
        }

        public static bool operator >(Celeration acceleration1, Celeration acceleration2)
        {
            return (acceleration1.Value > acceleration2.Value);
        }

        public static bool operator <(Celeration acceleration1, Celeration acceleration2)
        {
            return (acceleration1.Value < acceleration2.Value);
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Celeration acceleration)
        {
            return this.Value.CompareTo(acceleration.Value);
        }

        public bool Equals(Celeration acceleration)
        {
            return this.Value.Equals(acceleration.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Celeration)
            {
                return this.Equals((Celeration)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}mps2", this.MetersPerSecondSquared);
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
        public static Celeration Abs(Celeration acceleration)
        {
            return new Celeration(Math.Abs(acceleration.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public Celeration Abs()
        {
            return Celeration.Abs(this);
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>

        public const double Exactitude = 1E-10;
        /// <summary>
        /// Возвращает истину, если переданная скорость равна текущей с учетом погрешности типа данных.
        /// </summary>
        /// <param name="acceleration">Скорость</param>
        public bool AlmostEquals(Celeration acceleration)
        {
            return Math.Abs(this.Value - acceleration.Value) < Primitives.Celeration.Exactitude;
        }

        #endregion

        #region "Предметная область"

        #endregion

    }

}
