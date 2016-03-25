using System;
using System.Collections.Generic;

namespace OSM
{
    /// <summary>
    /// A way is an ordered list of nodes which normally also has at least one tag or is included within a Relation.
    /// </summary>
    /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist. A way can be open or closed. A closed way is one whose last node on the way is also the first on that way. A closed way may be interpreted either as a closed polyline, or an area, or both.</remarks>
    public class Way
    {

        /// <summary>
        /// Common attributes.
        /// </summary>
        public Attributes Attributes;

        /// <summary>
        /// Way id.
        /// </summary>
        public Int64 Id
        {
            get { return Attributes.Id; }
            set { Attributes.Id = value; }
        }

        /// <summary>
        /// Оrdered list of nodes.
        /// </summary>
        /// <remarks>A way can have between 2 and 2,000 nodes, although it's possible that faulty ways with zero or a single node exist.</remarks>
        public List<Node> Nodes { get; private set; }

        /// <summary>
        /// See Map Features for tagging guidelines.
        /// </summary>
        /// <remarks>A set of key/value pairs, with unique key.</remarks>
        public Dictionary<string, string> Tags { get; private set; }

        /// <summary>
        /// Инициализирует новый пустной экземпляр класса.
        /// </summary>
        public Way()
        {
            Nodes = new List<Node>();
            Tags = new Dictionary<string, string>();
        }

    }

}
