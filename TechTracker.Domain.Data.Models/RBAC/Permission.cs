using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.RBAC
{
    [DataContract]
    public class Permission : Entity
    {
        [DataMember]
        [Required]
        public string Description { get; set; }

        [DataMember]
        [Required]
        public string Endpoint { get; set; }

        [DataMember]
        [Required]
        public string Method { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} -> {2}", Id, Method, Endpoint);
        }
    }
}
