using System;

namespace Primitives
{

    /// <summary>
    /// Инкапсулирует выраженное в Кельвинах значение температуры.
    /// </summary>
    public struct Temperature : IComparable<Temperature>, IEquatable<Temperature>
    {

        /// <summary>
        /// Температура (К).
        /// </summary>
        private double Value;

        #region "Единицы измерения"

        /// <summary>
        /// Единицы измерения давления.
        /// </summary>
        public enum Units : int
        {
            /// <summary>
            /// Кельвин.
            /// </summary>
            K = 0,
            /// <summary>
            /// Градус Цельсия.
            /// </summary>
            C = 1,
            /// <summary>
            /// Градус Фаренгейта.
            /// </summary>
            F = 2
        }

        public static double KelToCel(double value)
        {
            return value - 273;
        }

        public static double CelToKel(double value)
        {
            return value + 273;
        }

        public static double KelToFar(double value)
        {
            return value * 1.8 - 459;
        }

        public static double FarToKel(double value)
        {
            return (value + 459) / 1.8;
        }

        #endregion

        #region "Свойства"

        /// <summary>
        /// Возвращает/устанавливает значение температуры в Кельвинах.
        /// </summary>
        public double K
        {
            get { return this.Value; }
            set { this.Value = value; }
        }

        /// <summary>
        /// Возвращает/устанавливает значение температуры в градусах Цельсия.
        /// </summary>
        public double C
        {
            get { return KelToCel(this.Value); }
            set { this.Value = CelToKel(value); }
        }

        /// <summary>
        /// Возвращает/устанавливает значение температуры в градусах Фаренгейта.
        /// </summary>
        public double F
        {
            get { return KelToFar(this.Value); }
            set { this.Value = FarToKel(value); }
        }

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует структуру значением температуры в Кельвинах.
        /// </summary>
        /// <param name="value">Значение температуры (К)</param>
        private Temperature(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Инициализирует структуру значением температуры в заданных единицах измерения.
        /// </summary>
        /// <param name="value">Значение температуры</param>
        /// <param name="unit">Единица измерения</param>
        public Temperature(double value, Units unit)
        {
            this.Value = 0.0;
            switch (unit)
            {
                case Units.K:
                    this.K = value;
                    break;
                case Units.C:
                    this.C = value;
                    break;
                case Units.F:
                    this.F = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("unit");
            }
        }

        #endregion

        #region "Операторы"

        #endregion

        #region "Интерфейсы и Переопределения"

        public int CompareTo(Temperature value)
        {
            return this.Value.CompareTo(value.Value);
        }

        public bool Equals(Temperature value)
        {
            return this.Value.Equals(value.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Temperature)
            {
                return this.Equals((Temperature)obj);
            }
            else {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}*C", this.C);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

        #region "Функции и процедуры"

        #endregion

        #region "Предметная область"

        #endregion

    }

}
