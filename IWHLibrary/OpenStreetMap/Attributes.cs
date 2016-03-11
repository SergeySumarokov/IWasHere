using System;
using System.Xml;

namespace OSM
{
    /// <summary>
    /// Common attributes within the OSM database.
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// Used for identifying the element.
        /// </summary>
        /// <remarks>64-bit integer number ≥ 1.
        /// Element types have their own ID space, so there could be a node with id=100 and a way with id=100, which are unlikely to be related or geographically near to each other.
        /// Editors may temporarily save node ids as negative to denote ids that haven't yet been saved to the server. Node ids on the server are persistent, meaning that the assigned id of an existing node will remain unchanged each time data are added or corrected. Deleted node ids must not be reused, unless a former node is now undeleted.</remarks>
        public Int64 Id;

        /// <summary>
        /// The display name of the user who last modified the object.
        /// </summary>
        /// <remarks>A user can change their display name.</remarks>
        public string UserName;

        /// <summary>
        /// The numeric user id of the user who last modified the object.
        /// </summary>
        /// <remarks>The user id number will remain constant.</remarks>
        public Int64 UserId;

        /// <summary>
        /// Time of the last modification.
        /// </summary>
        public DateTime TimeStamp;

        /// <summary>
        /// Whether the object is deleted or not in the database.
        /// </summary>
        /// <remarks>If visible="false" then the object should only be returned by history calls.</remarks>
        public bool Visible;

        /// <summary>
        /// The edit version of the object.
        /// </summary>
        /// <remarks>Newly created objects start at version 1 and the value is incremented by the server when a client uploads a new version of the object. The server will reject a new version of an object if the version sent by the client does not match the current version of the object in the database.</remarks>
        public Int64 Version;

        /// <summary>
        /// The changeset in which the object was created or updated. 
        /// </summary>
        public Int64 Changeset;

        public static Attributes Parse(XmlNode node)
        {
            Attributes attr = new Attributes();
            attr.Id = Int64.Parse(node.Attributes["id"].Value);
            return attr;

        }


    }
}
