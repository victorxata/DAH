using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.Business
{
    [DataContract]
    public class Account : Entity
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int TargetPositions { get; set; }

        [DataMember]
        public int FilledPositions { get; set; }

        public Account()
        {
            TargetPositions = 0;
            FilledPositions = 0;
        }
    }
}
