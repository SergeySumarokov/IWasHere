using System;
using Primitives;

namespace IWH

{
    /// <summary>
    /// Участок пути, заданный двумя точками с рассчитанным направлением пути и расстоянием.
    /// </summary>
    public class Leg
    {
        /// <summary>
        /// Ссылка на путь, которому принадлежит участок
        /// </summary>
        public Way Way;
        /// <summary>
        /// Порядковый номер участка в пути
        /// </summary>
        public Int32 Number;
        /// <summary>
        /// Начальная точка участка
        /// </summary>
        public Node StartNode;
        /// <summary>
        /// Конечная точка участка
        /// </summary>
        public Node EndNode;
        /// <summary>
        /// Направление участка
        /// </summary>
        public Angle Direction;
        /// <summary>
        /// Протяженность участка
        /// </summary>
        public Distance Lenght;
        /// <summary>
        /// Истина, исли участок был посещён.
        /// </summary>
        public Boolean IsVisited;
        /// <summary>
        /// Количество посещений участка
        /// </summary>
        public Int32 VisitedCount;
        /// <summary>
        /// Время последнего посещения участка.
        /// </summary>
        public DateTime LastVisitedTime;


        /// <summary>
        /// Возвращает кратчайшее расстояние между текущим и заданным, либо отрицательное значение, если проекции точек не попадают на вектора.
        /// </summary>
        public Distance MinLegOffset(Leg anotherLeg)
        {
            Distance offset = Distance.FromKilometers(-1);
            offset = MinNodeOffset(this, anotherLeg.StartNode, offset);
            offset = MinNodeOffset(this, anotherLeg.EndNode, offset);
            offset = MinNodeOffset(anotherLeg, StartNode, offset);
            offset = MinNodeOffset(anotherLeg, EndNode, offset);
            return offset;
        }

        private static Distance MinNodeOffset(Leg leg, Node node, Distance previsionOffset)
        {
            // Возвращаем длину проекции точки на вектор, либо предыдущее (переданное) значение 
            // если оно отрицательно, меньше текущего, либо проекция не попадает на вектор.
            Angle angle = leg.StartNode.Coordinates.OrthodromicBearing(node.Coordinates) - leg.Direction;
            Distance hypotenuse = leg.StartNode.Coordinates.OrthodromicDistance(node.Coordinates);
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
