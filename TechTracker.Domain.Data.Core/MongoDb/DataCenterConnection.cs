using System.Runtime.Serialization;

namespace TechTracker.Domain.Data.Core.MongoDb
{
    [DataContract]
    public class DataCenterConnection 
    {
        [DataMember]
        public string ConnectionName { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string DataBase { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
