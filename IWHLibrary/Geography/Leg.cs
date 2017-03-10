using Primitives;

namespace Geography
{
    
    /// <summary>
    /// Представляет участок пути, заданный начальной и конечной точкой
    /// </summary>
    public class Leg
    {

        /// <summary>
        /// Начальная точка участка
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// Конечная точка участка
        /// </summary>
        public Point EndPoint { get; set; }

        /// Направление участка
        /// </summary>
        public Angle Direction;

        /// <summary>
        /// Протяженность участка
        /// </summary>
        public Distance Lenght;

        /// <summary>
        /// Выполняет пересчёт направления и протяжённости участка
        /// </summary>
        public void Recalculate()
        {
            Direction = StartPoint.Coordinates.OrthodromicBearing(EndPoint.Coordinates);
            Lenght = StartPoint.Coordinates.OrthodromicDistance(EndPoint.Coordinates);
        }

        /// <summary>
        /// Возвращает кратчайшее расстояние между текущим и заданным участком пути, либо отрицательное значение, если проекции точек не попадают на вектора.
        /// </summary>
        public Distance MinLegOffset(Leg anotherLeg)
        {
            Distance offset = Distance.FromKilometers(-1);
            offset = MinNodeOffset(this, anotherLeg.StartPoint, offset);
            offset = MinNodeOffset(this, anotherLeg.EndPoint, offset);
            offset = MinNodeOffset(anotherLeg, StartPoint, offset);
            offset = MinNodeOffset(anotherLeg, EndPoint, offset);
            return offset;
        }

        private static Distance MinNodeOffset(Leg leg, Point point, Distance previsionOffset)
        {
            // Возвращаем длину проекции точки на вектор, либо предыдущее (переданное) значение 
            // если оно отрицательно, меньше текущего, либо проекция не попадает на вектор.
            Angle angle = leg.StartPoint.Coordinates.OrthodromicBearing(point.Coordinates) - leg.Direction;
            Distance hypotenuse = leg.StartPoint.Coordinates.OrthodromicDistance(point.Coordinates);
            Distance offset = (hypotenuse * angle.Sin()).Abs();
            Distance distance = hypotenuse * angle.Cos();
            if (distance < Distance.Zero || distance > leg.Lenght || (previsionOffset > Distance.Zero & previsionOffset < offset))
            {
                offset = previsionOffset;
            }
            return offset;
        }

    }

}
