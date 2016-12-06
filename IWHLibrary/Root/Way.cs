using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Primitives;

namespace IWH
{

    /// <summary>
    /// Типы линий.
    /// </summary>
    public enum HighwayType : int
    {
        Unknown = 0,
        Motorway = 1,
        Trunk = 2,
        Primary = 3,
        Secondary = 4,
        Tertiary = 5
    }

    public enum HighwaySurface : int
    {
        Unknown = 0,
        Asphalt = 1,
        Concrete = 2,
        Other = 3
    }

    public enum HighwaySmoothness : int
    {
        Unknown = 0,
        Excellent = 1,
        Good = 2,
        Intermediate = 3,
        Bad = 4,
        Horrible = 5
    }

    /// <summary>
    /// Линия.
    /// </summary>
    /// <remarks>
    /// A way is an ordered list of nodes which normally also has at least one tag or is included within a Relation.
    /// A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist. A way can be open or closed. A closed way is one whose last node on the way is also the first on that way. A closed way may be interpreted either as a closed polyline, or an area, or both.
    /// Описание полей относится к линиям, представляющим дороги.
    /// </remarks>
    [XmlRoot("way")]
    public class Way : IXmlSerializable
    {

        #region "Поля и свойства"

        /// <summary>
        /// Идентификатор OSM.
        /// </summary>
        public Int64 Id;

        /// <summary>
        /// Тип дороги.
        /// </summary>
        /// <remarks>
        /// tag k=highway - важность дороги в пределах дорожной сети.
        /// </remarks>
        public HighwayType Type;

        /// <summary>
        /// Истина, если линия является link.
        /// </summary>
        /// <remarks>
        /// tag k=highway v="*_link - связующие элементы дорог: съезды, въезды и т.п.
        /// </remarks>
        public Boolean IsLink;

        /// <summary>
        /// Покрытие дороги.
        /// </summary>
        /// <remarks>
        /// tag k=surface - тип покрытия дороги.
        /// </remarks>
        public HighwaySurface Surface;

        /// <summary>
        /// Качество дороги.
        /// </summary>
        /// <remarks>
        /// tag k=smoothness - качество дорожного покрытия.
        /// </remarks>
        public HighwaySmoothness Smoothness;

        /// <summary>
        /// Название дороги.
        /// </summary>
        public String Name;

        /// <summary>
        /// Признак наличия искусственного освещения.
        /// </summary>
        public Boolean Lighting;

        /// <summary>
        /// Признак одностороннего движения.
        /// </summary>
        public Boolean OneWay;

        /// <summary>
        /// Количество полос для движния.
        /// </summary>
        public Byte Lanes;

        /// <summary>
        /// Общая протяжённость дороги.
        /// </summary>
        public Distance Lenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков дороги.
        /// </summary>
        public Distance VisitedLenght;

        /// <summary>
        /// Истина, исли все сегменты дороги отмечены как посещенные?
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения одного из участков дороги.
        /// </summary>
        public DateTime LastVisitedTime;

        /// <summary>
        /// Упорядоченный список узлов (точек) линии.
        /// </summary>
        public List<Node> Nodes { get; private set; }

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Way()
        {
            Name = String.Empty;
            Nodes = new List<Node>();
        }

        #endregion

        #region "Методы и функции"

        /// <summary>
        /// Выполняет пересчет параметров линии.
        /// </summary>
        public void Recalculate()
        {
            Lenght = Distance.Zero;
            VisitedLenght = Distance.Zero;
            for (int i = 1; i <= Nodes.Count-1; i++)
            {
                Nodes[i].PartLenght = Nodes[i-1].Coordinates.OrthodromicDistance(Nodes[i].Coordinates);
                Nodes[i].PartDirection = Nodes[i-1].Coordinates.OrthodromicBearing(Nodes[i].Coordinates);
                Lenght += Nodes[i].PartLenght;
                if (Nodes[i].IsVisited && Nodes[i-1].IsVisited)
                    VisitedLenght += Nodes[i].PartLenght;
                if (Nodes[i].LastVisitedTime > LastVisitedTime)
                    LastVisitedTime = Nodes[i].LastVisitedTime;
            }
            IsVisited = Lenght.AlmostEquals(VisitedLenght);
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
            throw new NotSupportedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Линия
            writer.WriteAttributeString("name", Name.ToString(xmlFormatProvider));
            writer.WriteAttributeString("type", Type.ToString());
            writer.WriteAttributeString("link", IsLink.ToString());
            writer.WriteAttributeString("surface", Surface.ToString());
            writer.WriteAttributeString("smoothness", Smoothness.ToString());
            writer.WriteAttributeString("lighting", Lighting.ToString());
            writer.WriteAttributeString("oneway", OneWay.ToString());
            writer.WriteAttributeString("lanes", Lanes.ToString());
            writer.WriteAttributeString("visited", IsVisited.ToString(xmlFormatProvider));
            writer.WriteAttributeString("last", LastVisitedTime.ToString(xmlFormatProvider));
            writer.WriteAttributeString("id", Id.ToString(xmlFormatProvider));
            // Точки
            foreach (Node node in Nodes)
            {
                writer.WriteStartElement("ref");
                writer.WriteAttributeString("id", node.Id.ToString(xmlFormatProvider));
                writer.WriteEndElement();
            }
        }

        #endregion

    }

}
