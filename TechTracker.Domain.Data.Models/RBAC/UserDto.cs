using System.Runtime.Serialization;

namespace TechTracker.Domain.Data.Models.RBAC
{
    [DataContract]
    public class UserDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Username { get; set; }
    }
}