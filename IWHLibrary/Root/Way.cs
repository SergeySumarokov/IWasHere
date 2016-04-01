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
    public enum WayType : int
    {
        Unknown = 0,
        Motorway = 1,
        Trunk = 2,
        Primary = 3,
        Secondary = 4
    }

    /// <summary>
    /// Линия.
    /// </summary>
    [XmlRoot("way")]
    public class Way : IXmlSerializable
    {

        /// <summary>
        /// Идентификатор OSM.
        /// </summary>
        public Int64 Id;

        /// <summary>
        /// Тип линии.
        /// </summary>
        public WayType Type;

        /// <summary>
        /// Истина, если линия является link
        /// </summary>
        public Boolean IsLink;

        /// <summary>
        /// Наименование линии.
        /// </summary>
        public String Name;

        /// <summary>
        /// Общая протяженность линии.
        /// </summary>
        public Distance Lenght;

        /// <summary>
        /// Суммарная протяженность посещённых участков линии.
        /// </summary>
        public Distance VisitedLenght;

        /// <summary>
        /// Истина, исли вся линия была посещена.
        /// </summary>
        public Boolean IsVisited;

        /// <summary>
        /// Время последнего посещения любой из точек линии.
        /// </summary>
        public DateTime LastVisitedTime;

        /// <summary>
        /// Оrdered list of nodes.
        /// </summary>
        /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist.</remarks>
        public List<Node> Nodes { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Way()
        {
            Name = String.Empty;
            Nodes = new List<Node>();
        }

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

        #region "IXmlSerializable Members"

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
