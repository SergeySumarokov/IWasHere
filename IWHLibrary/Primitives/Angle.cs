using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженное в радианах значение угла.
    /// </summary>
    public struct Angle : IComparable<Angle>, IEquatable<Angle>
    {

        /// <summary>
        /// Значение угла (рад).
        /// </summary>
        private double Value;
        /// <summary>
        /// Представляет нулевое значение угла.
        /// </summary>
        public static readonly Angle Zero = new Angle(0);
        /// <summary>
        /// Представляет значение прямого угла (90 гр).
        /// </summary>
        public static readonly Angle Right = new Angle(Math.PI / 2);
        /// <summary>
        /// Представляет значение развернутого угла (180 гр).
        /// </summary>
        public static readonly Angle Straight = new Angle(Math.PI);
        
        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения угла.
        /// </summary>
        public enum Unit : int
        {
            Radians = 0,
            Degrees = 1
        }
        
        private const double _convertDegToRad = Math.PI / 180;
        /// <summary>
        /// Перевод градусов в радианы.
        /// </summary>
        /// <param name="dergees">Значение в градусах</param>
        /// <returns>Значение в радианах</returns>
        public static double DegToRad(double dergees)
        {
            return dergees * _convertDegToRad;
        }

        /// <summary>
        /// Перевод радиан в градусы.
        /// </summary>
        /// <param name="radians">Значение в радианах</param>
        /// <returns>Значение в градусах</returns>
        public static double RadToDeg(double radians)
        {
            return radians / _convertDegToRad;
        }

        #endregion

        #region "Свойства"

        private const double _PI = Math.PI;
        private const double _2PI = Math.PI * 2;

        /// <summary>
        /// Возвращает/устанавливает значение угла в радианах от -PI (исключая) до +PI.
        /// </summary>
        /// <remarks>При установке значения проводится преобразование к требуемому диапазону.</remarks>
        public double Radians
        {
            get { return this.Value; }
            set
            {
                value = value % _2PI;
                if (value > _PI)
                {
                    value -= _2PI;
                }
                else if (value <= -_PI)
                {
                    value += _2PI;
                }
                this.Value = value;
            }
        }

        /// <summary>
        /// Возвращает/устанавливает значение угла в градусах от -180 (исключая) до +180.
        /// </summary>
        /// <remarks>При установке значения проводится преобразование к требуемому диапазону.</remarks>
        public double Degrees
        {
            get { return Angle.RadToDeg(this.Value); }
            set { this.Radians = Angle.DegToRad(value); }
        }

        /// <summary>
        /// Возвращает Истину, если значение не задано.
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.Value == 0.0); }
        }

        /// <summary>
        /// Возвращает Истину, если значение больше нуля.
        /// </summary>
        public bool IsPositive
        {
            get { return (this.Value > 0.0); }
        }

        /// <summary>
        /// Возвращает Истину, если значение меньше нуля.
        /// </summary>
        public bool IsNegative
        {
            get { return (this.Value < 0.0); }
        }

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует структуру значением угла в радианах.
        /// </summary>
        /// <param name="value">Значение угла (рад)</param>
        private Angle(double value)
        {
            this.Value = 0.0;
            this.Radians = value;
        }

        /// <summary>
        /// Инициализирует структуру заданных значением в указанных единицах измерения.
        /// </summary>
        /// <param name="value">Значение угла</param>
        /// <param name="unit">Единица измерения</param>
        public Angle(double value, Angle.Unit unit)
        {
            this.Value = 0.0;
            switch (unit)
            {
                case Unit.Radians:
                    this.Radians = value;
                    break;
                case Unit.Degrees:
                    this.Degrees = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество радиан.
        /// </summary>
        /// <param name="value">Количество радиан</param>
        public static Angle FromRadians(double value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Возвращает структуру, представляющую указанное количество градусов.
        /// </summary>
        /// <param name="value">Количество градусов</param>
        public static Angle FromDegrees(double value)
        {
            return new Angle(value, Unit.Degrees);
        }

        #endregion

        #region "Операторы"

        public static Angle operator +(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.Value + angle2.Value);
        }

        public static Angle operator -(Angle angle1, Angle angle2)
        {
            return new Angle(angle1.Value - angle2.Value);
        }

        public static Angle operator *(Angle angle, double value)
        {
            return new Angle(angle.Value * value);
        }

        public static Angle operator /(Angle angle, double value)
        {
            return new Angle(angle.Value / value);
        }

        public static AngularVelocity operator /(Angle angle, Time time)
        {
            return new AngularVelocity(angle.Radians / time.Seconds, AngularVelocity.Unit.RadiansPerSecond);
        }

        public static bool operator ==(Angle angle1, Angle angle2)
        {
            return angle1.Value == angle2.Value;
        }

        public static bool operator !=(Angle angle1, Angle angle2)
        {
            return angle1.Value != angle2.Value;
        }

        public static bool operator <(Angle angle1, Angle angle2)
        {
            return angle1.Value < angle2.Value;
        }

        public static bool operator >(Angle angle1, Angle angle2)
        {
            return angle1.Value > angle2.Value;
        }

        public static bool operator <=(Angle angle1, Angle angle2)
        {
            return angle1.Value <= angle2.Value;
        }

        public static bool operator >=(Angle angle1, Angle angle2)
        {
            return angle1.Value >= angle2.Value;
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Angle angle)
        {
            return this.Value.CompareTo(angle.Value);
        }

        public bool Equals(Angle angle)
        {
            return this.Value.Equals(angle.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Angle)
            {
                return this.Equals((Angle)obj);
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
            return string.Format("{0}deg", this.Degrees);
        }

        #endregion

        #region "Функции и процедуры"

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public static Angle Abs(Angle angle)
        {
            return new Angle(Math.Abs(angle.Value));
        }

        /// <summary>
        /// Возвращает новую структуру, содержащую абсолютное значение.
        /// </summary>
        public Angle Abs()
        {
            return Angle.Abs(this);
        }

        /// <summary>
        /// Определяет точность значения при сравнении методом AlmostEquals.
        /// </summary>
        public const double Exactitude = 1E-10;

        /// <summary>
        /// Возвращает истину, если переданное значение равно текущему с учетом погрешности типа данных.
        /// </summary>
        /// <param name="angle">Угол</param>
        public bool AlmostEquals(Angle angle)
        {
            return Math.Abs(this.Value - angle.Value) < Angle.Exactitude;
        }

        /// <summary>
        /// Возвращает новую структуру Angle со значением от 0 (исключая) до PI*2 .
        /// </summary>
        public Angle Normal()
        {
            Angle result = this;
            if (result.Value <= 0)
            {
                result.Value += _2PI;
            }
            return result;
        }

        #endregion

        #region "Тригонометрия"

        /// <summary>
        /// Возвращает синус угла.
        /// </summary>
        public double Sin()
        {
            return Math.Sin(this.Value);
        }

        /// <summary>
        /// Возвращает косинус угла.
        /// </summary>
        public double Cos()
        {
            return Math.Cos(this.Value);
        }

        /// <summary>
        /// Возвращает тангенс угла.
        /// </summary>
        public double Tan()
        {
            return Math.Tan(this.Value);
        }

        #endregion

    }

}
