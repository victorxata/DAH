using System;

namespace TechTracker.Domain.Data.Core.MongoDb.Exceptions
{
    [Serializable]
    public class DataCenterMissingConnectionException : System.Exception
    {
        public DataCenterMissingConnectionException(string connectionName, string tenantId)
            :base(string.Format("Cannot find a Data Center Connection named \"{0}\" for tenant ID {1}", connectionName, tenantId))
        {
            
        }
    }
}
