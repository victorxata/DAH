using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTracker.Domain.Data.Models.RBAC
{
    [DataContract]
    public class RoleDto 
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Permission> Permissions { get; set; }

        [DataMember]
        public List<UserDto> Users { get; set; }
    
    }
}