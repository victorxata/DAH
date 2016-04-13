using System;
using System.Runtime.Serialization;

namespace TechTracker.Domain.Data.Core.MongoDb.Changes
{
    /// <summary>
    /// Class that tracks changes on entities
    /// </summary>
    [DataContract]
    public class Change : Entity
    {
        /// <summary>
        /// Date and Time when the change occurs
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The user that triggers the change
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        /// The whole old Entity changed
        /// </summary>
        [DataMember]
        public Object OldEntity { get; set; }

        /// <summary>
        /// The whole new Entity
        /// </summary>
        [DataMember]
        public Object NewEntity { get; set; }

        /// <summary>
        /// The type of the change made to the entity
        /// </summary>
        [DataMember]
        public ChangeType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }
    }
}
