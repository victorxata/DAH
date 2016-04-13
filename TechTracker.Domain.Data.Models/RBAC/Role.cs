using System.Collections.Generic;
using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.RBAC
{
    [DataContract]
    public class Role : Entity
    {
        public const string SuperUserName = "SuperUser";

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<string> PermissionNames { get; set; }

        [DataMember]
        public List<string> UserIds { get; set; }

        public Role()
        {
            PermissionNames = new List<string>();
            UserIds = new List<string>();
        }
    }
}
