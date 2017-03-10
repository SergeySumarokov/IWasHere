using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Geography;

namespace IWH
{

    /// <summary>
    /// Типы точек.
    /// </summary>
    public enum NodeType : int
    {
        Unknown = 0,
        Waypoint = 1,
        City = 2,
        Town = 3,
        Village = 4,
        ShowPlace = 5
    }

    /// <summary>
    /// Узел (точка пересечения).
    /// </summary>
    /// <remarks>
    /// A node represents a specific point on the earth's surface defined by its latitude and longitude.
    /// </remarks>
    [XmlRoot("node")]
    public class Node : Geography.Point, IXmlSerializable
    {

        #region "Поля и свойства"

        /// <summary>
        /// Геодезические координаты узла.
        /// </summary>
        /// <remarks>
        /// Latitude coordinate in degrees (North of equator is positive) using the standard WGS84 projection.
        /// Decimal number ≥ −90.0000000 and ≤ 90.0000000 with 7 decimal places.
        /// Longitude coordinate in degrees (East of Greenwich is positive) using the standard WGS84 projection.
        /// Decimal number ≥ −180.0000000 and ≤ 180.0000000 with 7 decimal places.
        /// </remarks>
        //public Coordinates Coordinates;

        /// <summary>
        /// Список участков, которые используют точку.
        /// </summary>
        public List<Leg> Legs;

        /// <summary>
        /// Идентификатор узла OSM.
        /// </summary>
        /// <remarks>
        /// 64-bit integer number.
        /// Node ids are unique between nodes.
        /// </remarks>
        public Int64 OsmId;
        
        /// <summary>
        /// Тип точки.
        /// </summary>
        /// <remarks>
        /// В зависимости от тега OSM:
        /// Waypoint для узлов, используемых в Way c тэгом Highway;
        /// City, Town, Village для узлов с тэгом Place соотвествующего значения;
        /// ShowPlace для точек, загружаемых из отдельного файла.
        /// </remarks>
        public NodeType Type;

        /// <summary>
        /// Наименование узла.
        /// </summary>
        /// <remarks>
        /// Не используется для узлов типа Waypoint.
        /// </remarks>
        public String Name;

        /// <summary>
        /// Население населенного пункта.
        /// </summary>
        /// <remarks>
        /// Используется только для узлов типа City, Town, Village.
        /// </remarks>
        public Int32 Population;

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Node()
        {
            Legs = new List<Leg>();
            Name = String.Empty;
        }

        #endregion

        #region "Реализация IXmlSerializable"

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            OsmId = Int64.Parse(reader.GetAttribute("id"), xmlFormatProvider);
            Coordinates.Latitude.Degrees = double.Parse(reader.GetAttribute("lat"), xmlFormatProvider);
            Coordinates.Longitude.Degrees = double.Parse(reader.GetAttribute("lon"), xmlFormatProvider);
            string typeString = reader.GetAttribute("type");
            if (typeString != null)
            {
                Type = (NodeType)Enum.Parse(typeof(NodeType), typeString);
                Name = reader.GetAttribute("name");
                Population = Int32.Parse(reader.GetAttribute("pop"), xmlFormatProvider);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", OsmId.ToString(xmlFormatProvider));
            writer.WriteAttributeString("lat", Coordinates.Latitude.Degrees.ToString(xmlFormatProvider));
            writer.WriteAttributeString("lon", Coordinates.Longitude.Degrees.ToString(xmlFormatProvider));
            if (Type != NodeType.Waypoint)
            {
                writer.WriteAttributeString("type", Type.ToString());
                writer.WriteAttributeString("name", Name.ToString(xmlFormatProvider));
                writer.WriteAttributeString("pop", Population.ToString());
            }
        }

        #endregion

    }

}
