using System;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Common.Utils;

namespace TechTracker.Domain.Data.Core.MongoDb
{
    public static class DcData
    {
        public static DataCenterConnection GetConnection(string tenantId, TenantDatabase database)
        {
            var datacenter = GetTenantDataCenter(tenantId);
            return GetConnection(datacenter, database);
        }

        public static string GetConnectionString(string tenantId, TenantDatabase database)
        {
            var dataCenter = GetTenantDataCenter(tenantId);
            return GetConnStr(dataCenter, database);
        }

        private static DataCenter GetTenantDataCenter(string tenantId)
        {
            DataCenter dataCenter = null;
            //var client =
            //    new MongoClient(ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDbServer] +
            //    "/" + ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
            //var db =
            //    client.GetDatabase(
            //        ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
            //var tenantsCollection = db.GetCollection<BsonDocument>(Constants.MongoDbSettings.CollectionNames.Tenants);
            //var filter = new BsonDocument("_id", ObjectId.Parse(tenantId));

            //var tenant = tenantsCollection.Find(filter).FirstOrDefaultAsync().Result;

            //if (tenant != null)
            //{
            //    var dataCentersCollection =
            //        db.GetCollection<DataCenter>(Constants.MongoDbSettings.CollectionNames.DataCenters);

            //    var dcfilter = new BsonDocument("_id", ObjectId.Parse(tenant["DataCenterId"].ToString()));

            //    dataCenter = dataCentersCollection.Find(dcfilter).ToListAsync().Result.FirstOrDefault();
            //    if (dataCenter == null)
            //    {
            //        throw new Exception("Could not find dataCenter for id " + tenant["DataCenterId"]);
            //    }
            //}
            //else
            //{
            //    throw new Exception("Could not find tenant for id " + tenantId);
            //}
            return dataCenter;
        }

        private static string GetConnStr(DataCenter dataCenter, TenantDatabase database)
        {
            string connectionStr = null;

            if (dataCenter == null)
                return null;

            var connStr = dataCenter.DataCenterConnections.FirstOrDefault(x => x.ConnectionName == database.ToString());

            if (connStr != null)
                connectionStr = connStr.ConnectionString;

            return connectionStr;
        }

        private static DataCenterConnection GetConnection(DataCenter dataCenter, TenantDatabase database)
        {
             var dcc = dataCenter.DataCenterConnections.FirstOrDefault(x => x.ConnectionName == database.ToString());

             if (dcc == null)
                throw new Exception("Could not find connection for datacenter id " + dataCenter.Id + " with name " + database.ToString());

            return dcc;
        }

    }
}