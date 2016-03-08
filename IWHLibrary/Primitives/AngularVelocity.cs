using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженную в радианах в секунду угловую скорость вращения.
    /// </summary>
    public struct AngularVelocity : IComparable<AngularVelocity>, IEquatable<AngularVelocity>
    {

        /// <summary>
        /// Значение угловой скорости (рад/с).
        /// </summary>
        private double Value;

        /// <summary>
        /// Представляет нулевое значение угловой скорости.
        /// </summary>
        public static readonly AngularVelocity Zero = new AngularVelocity(0);

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения угловой скорости.
        /// </summary>
        public enum Unit : int
        {
            RadiansPerSecond = 0,
            DegreesPerSecond = 1
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение угловой скорости вращения в радианах в секунду.
        /// </summary>
        public double RadiansPerSecond
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение угловой скорости вращения в градусах в секунду.
        /// </summary>
        public double DegreesPerSecond
        {
            get { return Angle.RadToDeg(this.Value); }
            set { this.Value = Angle.DegToRad(value); }
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
        /// Инициализирует структуру значением угловой скорости.
        /// </summary>
        /// <param name="value">Значение угловой скорости (рад/с)</param>
        private AngularVelocity(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру заданных значением в указанных единицах измерения.
        /// </summary>
        /// <param name="value">Значение угловой скорости</param>
        /// <param name="unit">Единица измерения</param>
        public AngularVelocity(double value, AngularVelocity.Unit unit)
        {
            this.Value = 0.0;
            switch (unit)
            {
                case Unit.RadiansPerSecond:
                    this.RadiansPerSecond = value;
                    break;
                case Unit.DegreesPerSecond:
                    this.DegreesPerSecond = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        #endregion

        #region "Операторы"

        public static AngularVelocity operator +(AngularVelocity value1, AngularVelocity value2)
        {
            return new AngularVelocity(value1.Value + value2.Value);
        }

        public static AngularVelocity operator -(AngularVelocity value1, AngularVelocity value2)
        {
            return new AngularVelocity(value1.Value - value2.Value);
        }

        public static AngularVelocity operator *(AngularVelocity value1, double value2)
        {
            return new AngularVelocity(value1.Value * value2);
        }

        public static Angle operator *(AngularVelocity angularVelocity, Time time)
        {
            return new Angle(angularVelocity.RadiansPerSecond * time.Seconds, Angle.Unit.Radians);
        }

        public static AngularVelocity operator /(AngularVelocity value1, double value2)
        {
            return new AngularVelocity(value1.Value / value2);
        }

        public static bool operator ==(AngularVelocity value1, AngularVelocity value2)
        {
            return value1.Value == value2.Value;
        }

        public static bool operator !=(AngularVelocity value1, AngularVelocity value2)
        {
            return value1.Value != value2.Value;
        }

        public static bool operator >(AngularVelocity value1, AngularVelocity value2)
        {
            return value1.Value > value2.Value;
        }

        public static bool operator <(AngularVelocity value1, AngularVelocity value2)
        {
            return value1.Value < value2.Value;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(AngularVelocity value)
        {
            return this.Value.CompareTo(value.Value);
        }

        public bool Equals(AngularVelocity value)
        {
            return this.Value.Equals(value.Value);
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
            return string.Format("{0}dps", this.DegreesPerSecond);
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
        public static AngularVelocity Abs(AngularVelocity angularVelocity)
        {
            return new AngularVelocity(Math.Abs(angularVelocity.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public AngularVelocity Abs()
        {
            return AngularVelocity.Abs(this);
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>
        public const double Exactitude = 1E-10;

        /// <summary>
        /// Возвращает истину, если переданное значение равно текущему с учетом погрешности типа данных.
        /// </summary>
        /// <param name="angularVelocity">Угловая скорость</param>
        public bool AlmostEquals(AngularVelocity angularVelocity)
        {
            return Math.Abs(this.Value - angularVelocity.Value) < AngularVelocity.Exactitude;
        }

        #endregion

    }

}
