using System.Collections.Generic;
using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.RBAC
{
     [DataContract]
    public class FieldPermissionType : Entity
    {
         [DataMember]
         public string TypeName { get; set; }

         [DataMember]
         public IEnumerable<string> Properties { get; set; }

         public FieldPermissionType()
         {
             Properties = new List<string>();
         }
     }
}
