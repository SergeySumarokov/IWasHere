using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Primitives;

namespace IWH
{

    /// <summary>
    /// Типы точек.
    /// </summary>
    public enum NodeType : int
    {
        Waypoint = 0,
        City = 1,
        Town = 2,
        Village = 3,
        ShowPlace = 4
    }

    /// <summary>
    /// Точка.
    /// </summary>
    [XmlRoot("node")]
    public class Node : IXmlSerializable
    {

        /// <summary>
        /// Идентификатор OSM.
        /// </summary>
        public Int64 Id;

        /// <summary>
        /// Геодезические координаты точки.
        /// </summary>
        public Coordinates Coordinates;

        /// <summary>
        /// Тип точки.
        /// </summary>
        public NodeType Type;

        /// <summary>
        /// Наименование точки.
        /// </summary>
        public String Name;

        /// <summary>
        /// Длина участка пути (удаление от предыдущей точки линии).
        /// </summary>
        public Distance PartLenght;

        /// <summary>
        /// Направление участка пути (азимут от предыдущей точки линии).
        /// </summary>
        public Angle PartDirection;

        /// <summary>
        /// Радиус окружности, при входе в которую точка считается посещенной.
        /// </summary>
        public Distance Range;

        /// <summary>
        /// Истина, исли точка была посещена.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения точки.
        /// </summary>
        public DateTime LastVisitedTime;

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Node()
        {
            Name = String.Empty;
        }

        #region "IXmlSerializable Members"

        private static IFormatProvider xmlFormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute("name");
            Type = (NodeType)Enum.Parse(typeof(NodeType), reader.GetAttribute("type"));
            Coordinates.Latitude.Degrees = double.Parse(reader.GetAttribute("lat"), xmlFormatProvider);
            Coordinates.Longitude.Degrees = double.Parse(reader.GetAttribute("lon"), xmlFormatProvider);
            IsVisited = Boolean.Parse(reader.GetAttribute("visited"));
            LastVisitedTime = DateTime.Parse(reader.GetAttribute("last"), xmlFormatProvider);
            Id = Int64.Parse(reader.GetAttribute("id"), xmlFormatProvider);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name.ToString(xmlFormatProvider));
            writer.WriteAttributeString("type", Type.ToString());
            writer.WriteAttributeString("lat", Coordinates.Latitude.Degrees.ToString(xmlFormatProvider));
            writer.WriteAttributeString("lon", Coordinates.Longitude.Degrees.ToString(xmlFormatProvider));
            writer.WriteAttributeString("visited", IsVisited.ToString(xmlFormatProvider));
            writer.WriteAttributeString("last", LastVisitedTime.ToString(xmlFormatProvider));
            writer.WriteAttributeString("id", Id.ToString(xmlFormatProvider));
        }

        #endregion

    }

}
