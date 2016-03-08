using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженную в метрах высоту над уровнем земного геоида.
    /// </summary>
    public struct Altitude : IComparable<Altitude>, IEquatable<Altitude>
    {

        /// <summary>
        /// Высота (м).
        /// </summary>
        private double Value;
        
        /// <summary>
        /// Представляет нулевое значение высоты.
        /// </summary>
        public static readonly Altitude Zero = new Altitude(0);

        #region "Единицы измерения"

        private const double _convertMtToFt = 0.3048;
        
        /// <summary>
        /// Единицы измерения высоты.
        /// </summary>
        public enum Unit : int
        {
            Meters = 0,
            Feet = 1
        }

        /// <summary>
        /// Перевод метров в футы.
        /// </summary>
        /// <param name="meters">Значение в метрах</param>
        /// <returns>Значение в футах</returns>
        public static double MetersToFeet(double meters)
        {
            return meters / _convertMtToFt;
        }

        /// <summary>
        /// Перевод футов в метры.
        /// </summary>
        /// <param name="feet">Значение в футах</param>
        /// <returns>Значение в метрах</returns>
        public static double FeetToMeters(double feet)
        {
            return feet * _convertMtToFt;
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение высоты в метрах.
        /// </summary>
        public double Meters
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение высоты в футах.
        /// </summary>
        public double Feet
        {
            get { return MetersToFeet(this.Value); }
            set { this.Value = FeetToMeters(value); }
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
        /// Инициализирует структуру значением высоты в метрах.
        /// </summary>
        /// <param name="value">Значение высоты (м)</param>
        private Altitude(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру заданных значением в указанных единицах измерения.
        /// </summary>
        /// <param name="value">Значение высоты</param>
        /// <param name="unit">Единица измерения</param>
        public Altitude(double value, Altitude.Unit unit)
        {
            switch (unit)
            {
                case Unit.Meters:
                    this.Meters = value;
                    break;
                case Unit.Feet:
                    this.Feet = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество метров.
        /// </summary>
        /// <param name="value">Количество метров</param>
        public static Altitude FromMeters(double value)
        {
            return new Altitude(value);
        }

        #endregion

        #region "Операторы"

        public static Altitude operator +(Altitude altitude1, Altitude altitude2)
        {
            return new Altitude(altitude1.Value + altitude2.Value);
        }

        public static Altitude operator -(Altitude altitude1, Altitude altitude2)
        {
            return new Altitude(altitude1.Value - altitude2.Value);
        }

        public static Altitude operator *(Altitude altitude, double value)
        {
            return new Altitude(altitude.Value * value);
        }

        public static Altitude operator /(Altitude altitude, double value)
        {
            return new Altitude(altitude.Value / value);
        }

        public static Time operator /(Altitude altitude, Speed speed)
        {
            return new Time(altitude.Meters / speed.MetersPerSecond, Time.Unit.Second);
        }

        public static Speed operator /(Altitude altitude, Time time)
        {
            return new Speed(altitude.Meters / time.Seconds, Speed.Unit.MetersPerSecond);
        }

        public static bool operator ==(Altitude altitude1, Altitude altitude2)
        {
            return altitude1.Value == altitude2.Value;
        }

        public static bool operator !=(Altitude altitude1, Altitude altitude2)
        {
            return altitude1.Value != altitude2.Value;
        }

        public static bool operator <(Altitude altitude1, Altitude altitude2)
        {
            return altitude1.Value < altitude2.Value;
        }

        public static bool operator >(Altitude altitude1, Altitude altitude2)
        {
            return altitude1.Value > altitude2.Value;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Altitude altitude)
        {
            return this.Value.CompareTo(altitude.Value);
        }

        public bool Equals(Altitude altitude)
        {
            return this.Value.Equals(altitude.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Altitude)
            {
                return this.Equals((Altitude)obj);
            }
            else {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}m", this.Meters);
        }

        #endregion

        #region "Функции и процедуры"

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public static Altitude Abs(Altitude altitude)
        {
            return new Altitude(Math.Abs(altitude.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public Altitude Abs()
        {
            return Altitude.Abs(this);
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>
        public const double Exactitude = 1E-10;

        /// <summary>
        /// Возвращает истину, если переданное значение равно текущему с учетом погрешности типа данных.
        /// </summary>
        /// <param name="altitude">Высота</param>
        public bool AlmostEquals(Altitude altitude)
        {
            return Math.Abs(this.Value - altitude.Value) < Altitude.Exactitude;
        }

        #endregion

    }

}
