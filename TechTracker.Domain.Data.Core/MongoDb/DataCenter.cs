using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTracker.Domain.Data.Core.MongoDb
{
    [DataContract]
    public class DataCenter : Entity
    {
        /// <summary>
        /// The friendly name of this DataCenter.
        /// </summary>
        [DataMember]
        public string DataCenterName { get; set; }
        
        /// <summary>
        /// Public DataCenters are visible to regular users, and tenants can be created under them.
        /// </summary>
        [DataMember]
        public bool IsPublic { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// List of DataCenterConnections of this DataCenter
        /// </summary>
        [DataMember]
        public List<DataCenterConnection> DataCenterConnections { get; set; }

        [DataMember]
        public bool Deleted { get; set; }

        [DataMember]
        public string DeletedBy { get; set; }

        [DataMember]
        public DateTime? DeletedWhen { get; set; }

        [DataMember]
        public DateTime Created_at { get; set; }

        [DataMember]
        public DateTime Updated_at { get; set; }

        public DataCenter()
        {
            DataCenterConnections = new List<DataCenterConnection>();
            Created_at = DateTime.UtcNow;
            Updated_at = Created_at;
        }
    }
}
