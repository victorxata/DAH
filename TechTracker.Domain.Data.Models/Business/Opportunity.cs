using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Validation;

namespace TechTracker.Domain.Data.Models.Business
{
    [DataContract]
    public class Opportunity : Entity
    {
        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public int Target { get; set; }

        [DataMember]
        [EnumeratorIncludedIn(typeof(EmployeeLevel))]
        public EmployeeLevel Level { get; set; }

        [DataMember]
        public List<Skill> Skills { get; set; } 

        [DataMember]
        public bool Sold { get; set; }
    }
}
