using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженное в Паскалях значение давления.
    /// </summary>
    public struct Pressure : IComparable<Pressure>, IEquatable<Pressure>
    {

        /// <summary>
        /// Давление (Па).
        /// </summary>
        private double Value;

        /// <summary>
        /// Представляет нулевое значение давления.
        /// </summary>
        public static readonly Pressure Zero = new Pressure(0);

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения давления.
        /// </summary>
        public enum Unit : int
        {
            Pa = 0,
            mBar = 1,
            mmHg = 2,
            inHg = 3
        }

        private const double _convertTomBar = 0.01;
        private const double _convertTommHg = 0.007500638;
        private const double _convertToinHg = 0.000295301;

        public static double PaTomBar(double value)
        {
            return value * _convertTomBar;
        }

        public static double mBarToPa(double value)
        {
            return value / _convertTomBar;
        }

        public static double PaTommHg(double value)
        {
            return value * _convertTommHg;
        }

        public static double mmHgToPa(double value)
        {
            return value / _convertTommHg;
        }

        public static double PaToinHg(double value)
        {
            return value * _convertToinHg;
        }

        public static double inHgToPa(double value)
        {
            return value / _convertToinHg;
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение давления в миллибарах (мБа), т.е. гектопаскалях (гПа).
        /// </summary>
        public double mBar
        {
            get { return PaTomBar(this.Value); }
            set { this.Value = mBarToPa(value); }
        }

        /// <summary>
        /// Возвращает/устанавливает значение давления в миллиметрах ртутного столба (мм.рт.ст.).
        /// </summary>
        public double mmHg
        {
            get { return PaTommHg(this.Value); }
            set { this.Value = mmHgToPa(value); }
        }

        /// <summary>
        /// Возвращает/устанавливает значение давления в дюймах ртутного столба.
        /// </summary>
        public double inHg
        {
            get { return PaToinHg(this.Value); }
            set { this.Value = inHgToPa(value); }
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
        /// Инициализирует структуру значением давления в паскалях.
        /// </summary>
        /// <param name="value">Значение давления (Па)</param>
        private Pressure(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру значением давления в заданных единицах измерения.
        /// </summary>
        /// <param name="value">Значение давления</param>
        /// <param name="unit">Единица измерения</param>
        public Pressure(double value, Unit unit)
        {
            this.Value = 0.0;
            switch (unit)
            {
                case Unit.Pa:
                    this.Value = value;
                    break;
                case Unit.mBar:
                    this.mBar = value;
                    break;
                case Unit.mmHg:
                    this.mmHg = value;
                    break;
                case Unit.inHg:
                    this.inHg = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        #endregion

        #region "Операторы"

        public static Pressure operator +(Pressure value1, Pressure value2)
        {
            return new Pressure(value1.Value + value2.Value);
        }

        public static Pressure operator -(Pressure value1, Pressure value2)
        {
            return new Pressure(value1.Value - value2.Value);
        }

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Pressure value)
        {
            return this.Value.CompareTo(value.Value);
        }

        public bool Equals(Pressure value)
        {
            return this.Value.Equals(value.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Pressure)
            {
                return this.Equals((Pressure)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}mBar", this.mBar);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

        #region "Функции и процедуры"

        #endregion

        #region "Предметная область"

        /// <summary>
        /// Возвращает относительное давление для заданной высоты.
        /// </summary>
        /// <remarks>
        /// ВАЖНО! Зачение вычисляется по примерной формуле и подходит для высот до 200м.
        /// </remarks>
        public Pressure GetQFE(Altitude altitude)
        {
            Pressure baroAlt = default(Pressure);
            baroAlt.mBar = altitude.Meters / 8.25;
            return this - baroAlt;
        }

        #endregion

    }

}
