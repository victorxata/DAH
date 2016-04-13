using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.Business
{
    [DataContract]
    public class Skill : Entity
    {
        [DataMember]
        public string Description { get; set; }
        
    }
}
