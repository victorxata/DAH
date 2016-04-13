using System.ComponentModel;

namespace TechTracker.Domain.Data.Core.MongoDb
{
    public enum TenantDatabase
    {
        /// <summary>
        /// Default MongoDb Storage
        /// </summary>
        [Description("Primary MongoDb")]
        TenantMongoDb,

        
    }
}
