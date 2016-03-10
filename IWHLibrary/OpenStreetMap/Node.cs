using System;
using System.Collections.Generic;
using Primitives;

namespace OSM
{
    /// <summary>
    /// A node represents a specific point on the earth's surface defined by its latitude and longitude.
    /// </summary>
    /// <remarks>64-bit integer number ≥ 1</remarks>
    public class Node
    {
        /// <summary>
        /// Node ids are unique between nodes.
        /// </summary>
        /// <remarks>64-bit integer number ≥ 1. 
        /// Editors may temporarily save node ids as negative to denote ids that haven't yet been saved to the server. Node ids on the server are persistent, meaning that the assigned id of an existing node will remain unchanged each time data are added or corrected. Deleted node ids must not be reused, unless a former node is now undeleted.</remarks>
        public Int64 Id;

        /// <summary>
        /// 
        /// </summary>
        public Coordinates Coordinates;

        /// <summary>
        /// Latitude coordinate in degrees (North of equator is positive) using the standard WGS84 projection.
        /// </summary>
        /// <remarks>decimal number ≥ −90.0000000 and ≤ 90.0000000 with 7 decimal places.
        /// Some applications may not accept latitudes above/below ±85 degrees for some projections.</remarks>
        public double Lat
        {
            get { return Coordinates.Latitude.Degrees; }
            set { Coordinates.Latitude.Degrees = value; }
        }

        /// <summary>
        /// Longitude coordinate in degrees (East of Greenwich is positive) using the standard WGS84 projection.
        /// </summary>
        /// <remarks>decimal number ≥ −180.0000000 and ≤ 180.0000000 with 7 decimal places.
        /// Note that the geographic poles will be exactly at latitude ±90 degrees but in that case the longitude will be set to an arbitrary value within this range.</remarks>
        public double Lon
        {
            get { return Coordinates.Longitude.Degrees; }
            set { Coordinates.Longitude.Degrees = value; }
        }

        /// <summary>
        /// See Map Features for tagging guidelines.
        /// </summary>
        /// <remarks>A set of key/value pairs, with unique key.</remarks>
        public Dictionary<string, string> Tags { get; private set; }

        public Node()
        {
            Tags = new Dictionary<string, string>();
        }

    }
}
