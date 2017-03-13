using System;
using System.Linq;
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
    public class Way : Geography.Way, IXmlSerializable
    {

        #region "Поля и свойства"

        /// <summary>
        /// Идентификатор OSM.
        /// </summary>
        public Int64 OsmId;

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
        /// Средняя скорость движения.
        /// </summary>
        public Speed AverageSpeed;

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
        //public List<Node> Points { get; private set; }

        /// <summary>
        /// Упорядоченный список участков пути.
        /// </summary>
        public List<Leg> Legs { get; set; }

        /// <summary>
        /// Возвращает первый участок пути
        /// </summary>
        public Leg FirstLeg
        {
            get { return Legs[0]; }
            private set { }
        }

        /// <summary>
        /// Возвращает последний участок пути
        /// </summary>
        public Leg LastLeg
        {
            get { return Legs[Legs.Count-1]; }
            private set { }
        }

        /// <summary>
        /// Возвращает первую точку пути
        /// </summary>
        public Node FirstPoint
        {
            get { return FirstLeg.StartPoint; }
            private set { }
        }

        /// <summary>
        /// Возвращает последщюю точку пути
        /// </summary>
        public Node LastPoint
        {
            get { return LastLeg.EndPoint; }
            private set { }
        }

        #endregion

        #region "Конструкторы"

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Way()
        {
            Name = String.Empty;
            Legs = new List<Leg>();
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
            Time visitedTime = Time.Zero;
            foreach (Leg leg in Legs)
            {
                Lenght += leg.Lenght;
                if (leg.IsVisited)
                {
                    VisitedLenght += leg.Lenght;
                    visitedTime += leg.Lenght / leg.Speed;
                }
                if (leg.LastVisitedTime > LastVisitedTime)
                {
                    LastVisitedTime = leg.LastVisitedTime;
                }
            }
            AverageSpeed = VisitedLenght / visitedTime;
            IsVisited = Lenght.AlmostEquals(VisitedLenght);
        }

        /// <summary>
        /// Разделяет путь на два. Указанное количество участков от начала пути переносится в новый путь.
        /// </summary>
        /// <param name="legsToCut"></param>
        /// <returns></returns>
        public Way CutLegs(int legsToCut)
        {
            Way result = (Way)this.MemberwiseClone();
            result.Legs = this.Legs.ToList();
            result.Legs.RemoveRange(legsToCut, this.Legs.Count - legsToCut);
            result.Recalculate();
            this.Legs.RemoveRange(0, legsToCut);
            this.Recalculate();
            return result;
        }


        /// <summary>
        /// Присоединяет заданный путь.
        /// </summary>
        /// <param name="way"></param>
        public void CombineLegs (Way attachedWay)
        {
            // Определяем каким концом присоединять
            if (FirstPoint.Equals(attachedWay.LastLeg))
            {
                // Присоединяем путь перед
                List<Leg> tempLegs = new List<Leg>(attachedWay.Legs);
                tempLegs.AddRange(Legs);
                Legs = tempLegs;
            }
            else
            {
                // Присоединяем путь после
                Legs.AddRange(attachedWay.Legs);
            }
            Recalculate();
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
            writer.WriteAttributeString("id", OsmId.ToString(xmlFormatProvider));
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
            // Первые точки участков
            for (int i=0; i<Legs.Count; i++)
            {
                WriteXml_WriteNode(writer, Legs[i], Legs[i].StartPoint);
            }
            // Последняя точка
            WriteXml_WriteNode(writer, null, Legs[Legs.Count-1].EndPoint);
        }

        private void WriteXml_WriteNode(XmlWriter writer, Leg leg, Node node)
        {
            writer.WriteStartElement("ref");
            writer.WriteAttributeString("id", node.OsmId.ToString(xmlFormatProvider));
            if (leg != null)
            {
                writer.WriteAttributeString("visited", leg.IsVisited.ToString(xmlFormatProvider));
                writer.WriteAttributeString("count", leg.VisitedCount.ToString(xmlFormatProvider));
                writer.WriteAttributeString("last", leg.LastVisitedTime.ToString(xmlFormatProvider));
            }
            writer.WriteEndElement();

        }

        #endregion

    }

}
