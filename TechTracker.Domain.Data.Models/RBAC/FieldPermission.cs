using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Validation;

namespace TechTracker.Domain.Data.Models.RBAC
{
    [DataContract]
    public class FieldPermission : Entity
    {
        [DataMember]
        [BsonElement("roleName")]
        public string RoleName { get; set; }

        [DataMember]
        [BsonElement("userName")]
        public string UserName { get; set; }

        [DataMember]
        [BsonElement("className")]
        public string ClassName { get; set; }

        [DataMember]
        [BsonElement("propertyName")]
        public string PropertyName { get; set; }

        [DataMember]
        [BsonElement("action")]
        [EnumeratorIncludedIn(typeof(ActionWhenHide))]
        public ActionWhenHide Action { get; set; }

        [DataMember]
        [BsonElement("substitutionText")]
        public string SubstitutionText { get; set; }
    }
}
