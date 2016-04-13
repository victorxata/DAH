using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb.Changes;

namespace TechTracker.Domain.Data.Core.MongoDb
{


    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public abstract class MongoRepository<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey>
    {
        protected abstract bool IsGlobal { get; }

        protected abstract void EnsureIndexes(string tenantId, IMongoCollection<T> collection);

        protected abstract bool TrackChanges { get; }

        protected abstract TenantDatabase DbType { get; }

        protected abstract string CollectionName { get; }

        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal IMongoCollection<T> Icollection;

        private readonly MongoCollection<T> _mongoCollection;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        protected MongoRepository()
            : this(Util<TKey>.GetDefaultConnectionString())
        {

        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        protected MongoRepository(string connectionString)
        {
            Icollection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);
            if (IsGlobal)
                EnsureIndexes(string.Empty, Collection);
            _mongoCollection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString) as MongoCollection<T>;
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        protected MongoRepository(string connectionString, string collectionName)
        {
            Icollection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString, collectionName);
            if (IsGlobal)
                EnsureIndexes(string.Empty, Collection);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        protected MongoRepository(MongoUrl url)
        {
            Icollection = Util<TKey>.GetCollectionFromUrl<T>(url);
            if (IsGlobal)
                EnsureIndexes(string.Empty, Collection);
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        protected MongoRepository(MongoUrl url, string collectionName)
        {
            Icollection = Util<TKey>.GetCollectionFromUrl<T>(url, collectionName);
            if (IsGlobal)
                EnsureIndexes(string.Empty, Collection);
        }

        #endregion

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
        /// </remarks>
        /// <value>The Mongo collection (to perform advanced operations).</value>
        public IMongoCollection<T> Collection
        {
            get
            {
                if (!IsGlobal)
                    throw new Exception(string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId, "GetCollection(tenantId)"));
                return Icollection;
            }
        }

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        //public string CollectionName
        //{
        //    get { return Icollection.CollectionNamespace.CollectionName; }
        //}

        #region MongoRepository

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await Collection.Find(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "AddAsync(T entity, string tenantId, string username) or Add(T entity, string tenantId, string username)"));
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public virtual async Task AddAsync(IEnumerable<T> entities)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "AddMany(IEnumerable<T> entities, string tenantId)"));
            await Collection.InsertManyAsync(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "Update(T entity, string tenantId, string username)"));
            await Collection.ReplaceOneAsync(x => entity.Id.Equals(x.Id), entity, new UpdateOptions {IsUpsert = true});
            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual async Task UpdateAsync(IEnumerable<T> entities)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "Update(IEnumerable<T> entities, string tenantId, string username)"));
            var tasks = entities.Select(async entity =>
            {
                await UpdateAsync(entity);
            });
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        public virtual async Task DeleteAsync(string id)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "RemoveAsync(string entityId, string tenantId, string username) or Remove(string entityId, string tenantId, string username)"));
            await Collection.DeleteOneAsync(entity => id.Equals(entity.Id));
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual async Task DeleteAsync(T entity)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "RemoveAsync(string entityId, string tenantId, string username) or Remove(string entityId, string tenantId, string username)"));
            await DeleteAsync(entity.Id.ToString());
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            if (!IsGlobal)
                throw new Exception(
                    String.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "RemoveAsync(string entityId, string tenantId, string username) or Remove(string entityId, string tenantId, string username)"));
            await Collection.DeleteManyAsync<T>(predicate);
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual async Task DeleteAllAsync()
        {
            if (!IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantId,
                    "RemoveAsync(string entityId, string tenantId, string username) or Remove(string entityId, string tenantId, string username)"));
            await DeleteAsync(entity => true);
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public virtual async Task<long> CountAsync()
        {
            if (!IsGlobal)
                throw new Exception(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantIdNoImplementation);
            return await Collection.CountAsync(entity => true);
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            if (!IsGlobal)
                throw new Exception(Constants.Errors.CannotExecuteOnThisRepositoryWithoutTenantIdNoImplementation);
            var entity = await Icollection.Find(predicate).FirstOrDefaultAsync();
            return entity != null;
        }

        #endregion

        #region MongoTxRepository

        /// <summary>
        /// Returns asyncronously the T by its given id in the datacenter that the given tenantId has the data.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <returns>The Entity T.</returns>
        public async Task<T> GetByIdAsync(string id, string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "AddAsync(T entity) or AddAsync(IEnumerable<T> entities)"));

            ObjectId objectid;
            var canParse = ObjectId.TryParse(id, out objectid);
            if (!canParse)
                throw new Exception(Constants.Errors.CannotParseId);

            var col = GetCollection(tenantId);
            
            return await col.Find(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns the T by its given id in the datacenter that the given tenantId has the data.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <returns>The Entity T.</returns>
        public T GetById(string id, string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "GetByIdAsync(string id)"));

            ObjectId objectid;
            var canParse = ObjectId.TryParse(id, out objectid);
            if (!canParse)
                throw new Exception(Constants.Errors.CannotParseId);

            var col = GetCollection(tenantId);
            
            return col.Find(x => id.Equals(x.Id)).FirstOrDefaultAsync().Result;
        }

        /// <summary>
        /// Returns asyncronously as queryable the complete collection
        /// </summary>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <returns>The queryable object</returns>
        public async Task<IQueryable<T>> GetAsync(string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "Find(filter).ToListAsync()"));
            var col = GetCollection(tenantId);
            return await Task.Run(() => col.Find(new BsonDocument()).ToListAsync().Result.AsQueryable());

        }

        public async Task<List<T>> GetAllAsync(string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "Find(filter).ToListAsync()"));
            var col = GetCollection(tenantId);
            return await col.Find(new BsonDocument()).ToListAsync();

        }

        /// <summary>
        /// Returns as queryable the complete collection
        /// </summary>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <returns>The queryable object</returns>
        public IQueryable<T> Get(string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "Find(filter).FirstOrDefaultAsync()"));
            var col = GetCollection(tenantId);
            return col.Find(new BsonDocument()).ToListAsync().Result.AsQueryable();
        }

        /// <summary>
        /// Adds the new entities in the repository. 
        /// 
        /// Important: This method will not add any change in the changes collection.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        public virtual void AddMany(IEnumerable<T> entities, string tenantId)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "Add(IEnumerable<T> entities)"));

            var col = GetCollection(tenantId);

            col.InsertManyAsync(entities).Wait();
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <param name="username">The name of the user that adds the new entity</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public async Task<T> AddAsync(T entity, string tenantId, string username)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                        "AddAsync(T entity)"));

            var col = GetCollection(tenantId);
            try
            {
                await col.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!TrackChanges) return entity;

            await TrackChange(tenantId, entity.Id.ToString(), null, username, ChangeType.Create);

            return entity;
        }

        public virtual async Task Update(IEnumerable<T> entities, string tenantId, string username)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "UpdateAsync(T entity"));
            var tasks = entities.Select(async entity =>
            {
                await Task.Run(() => Update(entity, tenantId, username));
            });
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <param name="tenantId">The Id of the tenant to open the used datacenter</param>
        /// <param name="username">The name of the user that adds the new entity</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public T Add(T entity, string tenantId, string username)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "AddAsync(T entity"));
            var col = GetCollection(tenantId);
            col.InsertOneAsync(entity).Wait();

            if (!TrackChanges) return entity;

            TrackChange(tenantId, entity.Id.ToString(), null, username, ChangeType.Create).Wait();

            return entity;
        }

        public T Update(T entity, string tenantId, string username)
        {
            if (IsGlobal)
                throw new Exception(
                    string.Format(Constants.Errors.CannotExecuteOnThisRepositoryWithTenantId,
                    "UpdateAsync(T entity"));
            T oldEntity = default(T);

            var id = entity.Id.ToString();
            if (!String.IsNullOrEmpty(id))
            {
                oldEntity = GetById(id, tenantId);

                if ((!String.IsNullOrEmpty(tenantId)) && (username.ToLower() != "system"))
                {
                    var fieldPermissions = GetFieldPermissionsByTenant(tenantId, username);

                    entity = CheckMissingFields(entity, fieldPermissions, oldEntity);
                }
            }

            if (TrackChanges)
            {
                TrackChange(tenantId, entity, oldEntity, username, ChangeType.Update);
            }

            var col = GetCollection(tenantId);
            col.ReplaceOneAsync(x => entity.Id.Equals(x.Id), entity, new UpdateOptions { IsUpsert = true }).Wait();
            return entity;
        }

        private static T CheckMissingFields(T entity, List<BsonDocument> fieldPermissions, T oldEntity)
        {
            foreach (var fieldPermission in fieldPermissions)
            {
                if (fieldPermission.Contains("className"))
                {
                    if (fieldPermission["className"].ToString() == entity.GetType().Name)
                    {
                        entity = AddMissingFields(entity, oldEntity, fieldPermissions);
                    }
                }
            }
            return entity;
        }

        public async Task<T> UpdateAsync(T entity, string tenantId, string username)
        {
            T oldEntity = default(T);


            if (entity.Id != null)
            {
                var id = entity.Id.ToString();
                oldEntity = GetById(id, tenantId);

                if ((!string.IsNullOrEmpty(tenantId)) && (username.ToLower() != "system"))
                {
                    var fieldPermissions = GetFieldPermissionsByTenant(tenantId, username);

                    entity = CheckMissingFields(entity, fieldPermissions, oldEntity);
                }
            }

            if (TrackChanges)
            {
                TrackChange(tenantId, entity, oldEntity, username, ChangeType.Update);
            }

            var col = GetCollection(tenantId);

            await col.ReplaceOneAsync(x => entity.Id.Equals(x.Id), entity, new UpdateOptions { IsUpsert = true });

            return entity;
        }

        public async Task RemoveAsync(string entityId, string tenantId, string username)
        {
            var col = GetCollection(tenantId);

            if (TrackChanges)
            {
                var oldEntity = GetByIdAsync(entityId, tenantId).Result;
                await TrackChange(tenantId, entityId, oldEntity.Id.ToString(), username, ChangeType.Delete);
            }
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entityId));
            await col.DeleteOneAsync(filter);
        }

        public void Remove(string entityId, string tenantId, string username)
        {
            var col = GetCollection(tenantId);

            if (TrackChanges)
            {
                var oldEntity = GetByIdAsync(entityId, tenantId).Result;
                TrackChange(tenantId, entityId, oldEntity.Id.ToString(), username, ChangeType.Delete).Wait();
            }
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entityId));
            col.DeleteOneAsync(filter).Wait();
        }

        #region GetCollection This methods are for Tenant Dependant Repositories

        public IMongoCollection<T> GetCollection(string tenantId)
        {
            var dataCenterConnection = GetDataCenterConnection(tenantId);

                if (!string.IsNullOrEmpty(dataCenterConnection.UserName))
                {
                    var cs1 = dataCenterConnection.ConnectionString.Replace("mongodb://", "");
                    var cs = "mongodb://" +
                             dataCenterConnection.UserName + ":" + dataCenterConnection.Password + "@" +
                             cs1 + "/" + dataCenterConnection.DataBase;
                    var secCol = Util<TKey>.GetCollectionFromConnectionString<T>(cs,
                    CollectionName);

                    EnsureIndexes(tenantId, secCol);
                    return secCol;
                }
                var col = Util<TKey>.GetCollectionFromConnectionString<T>(
                    dataCenterConnection.ConnectionString + "/" + dataCenterConnection.DataBase,
                    CollectionName);

                EnsureIndexes(tenantId, col);
                return col;
        }

        public IMongoCollection<BsonDocument> GetChangesCollection(string tenantId)
        {
            var dcc = GetDataCenterConnection(tenantId);

            var credentials = MongoCredential.CreateCredential(dcc.DataBase, dcc.UserName, dcc.Password);
            var client = new MongoClient(dcc.ConnectionString);

            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(client.Settings.Server.Host, client.Settings.Server.Port),
                Credentials = new List<MongoCredential> { credentials }
            };

            var server = new MongoClient(settings);

            var db = server.GetDatabase(dcc.DataBase);
            var col = db.GetCollection<BsonDocument>("Changes");

            return col;
        }

        #endregion

        #endregion

        #region Internal helpers

        private DataCenterConnection GetDataCenterConnection(string tenantId)
        {

            var dcc = DcData.GetConnection(tenantId, DbType);

            return dcc;
        }

        private static List<BsonDocument> GetFieldPermissionsByTenant(string tenantId, string userName)
        {
            if (string.IsNullOrEmpty(tenantId)) return new List<BsonDocument>();

            var user = GetUser(userName);

            string userId = null;
            if (user != null)
            {
                userId = user["_id"].ToString();

                var roles = GetUserRoles(tenantId, userId);

                var permissions = GetUserPermissions(tenantId, userId, roles);

                return permissions;
            }
            return new List<BsonDocument>();
        }

        private static List<BsonDocument> GetUserPermissions(string tenantId, string userName, IEnumerable<string> roles)
        {
            var dataCenterConnection = DcData.GetConnection(tenantId, TenantDatabase.TenantMongoDb);
            var client = new MongoClient(dataCenterConnection.ConnectionString);
            var db = client.GetDatabase(dataCenterConnection.DataBase);

            var fieldsCollection = db.GetCollection<BsonDocument>(Constants.MongoDbSettings.CollectionNames.FieldPermissions);

            var filter = new BsonDocument("$or", new BsonArray
            {
                new BsonDocument("tenantId", tenantId),
                new BsonDocument("userName", userName),
                new BsonDocument("roleName", new BsonDocument("$in", new BsonArray(roles.ToArray())))
            });
            var result = fieldsCollection.Find(filter).ToListAsync().Result;
            return result;
        }

        private static IEnumerable<string> GetUserRoles(string tenantId, string userId)
        {
            var dataCenterConnection = DcData.GetConnection(tenantId, TenantDatabase.TenantMongoDb);
            var client = new MongoClient(dataCenterConnection.ConnectionString);
            var db = client.GetDatabase(dataCenterConnection.DataBase);

            var rolesCollection = db.GetCollection<BsonDocument>(Constants.MongoDbSettings.CollectionNames.CustomRoles);
            var filter = Builders<BsonDocument>.Filter;
            var project = Builders<BsonDocument>.Projection;
            if (!String.IsNullOrEmpty(userId))
            {
                var result = rolesCollection
                    .Find(filter.AnyIn(userId, Constants.MongoDbSettings.CollectionNames.UserIdsFieldName))
                    .Project(project.Include("_id"))
                    .ToListAsync()
                    .Result;

                return result.Select(bsrole => bsrole.ToString()).ToList();
            }
            return new List<string>();
        }

        private static BsonDocument GetUser(string userName)
        {
            var client = new MongoClient(ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDbServer] +
                "/" + ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
            var db = client.GetDatabase(ConfigurationManager.AppSettings[Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
            var users = db.GetCollection<BsonDocument>(Constants.MongoDbSettings.CollectionNames.Users);
            var userFilter = new BsonDocument("UserName", userName);

            var user = users.Find(userFilter).Project("{_id:1}").ToListAsync().Result.ToList().FirstOrDefault();
            return user;
        }

        private async Task TrackChange(string tenantId, string entityId, string oldEntityId, string userName, ChangeType type)
        {
            var changesCollection = GetChangesCollection(tenantId);

            var entity = !String.IsNullOrEmpty(entityId) ? GetById(entityId, tenantId) : default(T);
            var oldEntity = !String.IsNullOrEmpty(oldEntityId) ? GetById(oldEntityId, tenantId) : default(T);

            BsonDocument change = null;

            if (type == ChangeType.Create)
            {
                change = new BsonDocument
                {
                    {"DateTime", DateTime.UtcNow},
                    {"NewEntity", entity.ToBsonDocument()},
                    {"EntityId", entity.Id.ToString()},
                    {"User", userName},
                    {"Type", type}
                };
            }
            if (type == ChangeType.Update)
            {
                change = new BsonDocument
                {
                    {"DateTime", DateTime.UtcNow},
                    {"NewEntity", entity.ToBsonDocument()},
                    {"OldEntity", oldEntity.ToBsonDocument()},
                    {"EntityId", entity.Id.ToString()},
                    {"User", userName},
                    {"Type", type}
                };
            }
            if (type == ChangeType.Delete)
            {
                change = new BsonDocument
                {
                    {"DateTime", DateTime.UtcNow},
                    {"OldEntity", oldEntity.ToBsonDocument()},
                    {"EntityId", entity.Id.ToString()},
                    {"User", userName},
                    {"Type", type}
                };
            }

            await changesCollection.InsertOneAsync(change);
        }

        private void TrackChange(string tenantId, T entity, T oldEntity, string userName, ChangeType type)
        {
            var changesCollection = GetChangesCollection(tenantId);

            var change = new BsonDocument
           {
               {"DateTime" , DateTime.UtcNow},
               {"NewEntity", entity.ToBsonDocument()},
               {"OldEntity", oldEntity.ToBsonDocument()},
               {"EntityId", entity.Id.ToString()},
               {"User", userName},
               {"Type", type}
           };
            changesCollection.InsertOneAsync(change).Wait();
        }

        private static T AddMissingFields(T entity, T oldEntity, List<BsonDocument> fieldPermissions)
        {
            var properties = entity.GetType().GetProperties(BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in properties)
            {
                var perm = fieldPermissions.FirstOrDefault(x => x["propertyName"] == propertyInfo.Name);

                if (perm == null) continue;

                var oldValue = propertyInfo.GetValue(oldEntity);
                propertyInfo.SetValue(entity, oldValue);
            }
            return entity;
        }

        #endregion

        #region UnitOfWork Not implemented yet

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <remarks>
        ///     <para>
        ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
        ///         RequestStart you can force a query to be on the same connection as the writes, so the query won't execute
        ///         until the server has caught up with the writes.
        ///     </para>
        ///     <para>
        ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
        ///         RequestStart/RequestDone can be nested and the right thing will happen.
        ///     </para>
        ///     <para>
        ///         Use the connectionstring to specify the readpreference; add "readPreference=X" where X is one of the following
        ///         values: primary, primaryPreferred, secondary, secondaryPreferred, nearest.
        ///         See http://docs.mongodb.org/manual/applications/replication/#read-preference
        ///     </para>
        /// </remarks>
        public virtual IDisposable RequestStart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        public virtual void RequestDone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable<T> Not implemented yet

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return Collection.Find(new BsonDocument()).ToListAsync().Result.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get
            {
                if (_provider != null) return _provider;
                var col = new MongoQueryProvider(_mongoCollection);
                _provider = col;
                return _provider;
            }
        }

        private MongoQueryProvider _provider { get; set; }

        #endregion
    }

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public abstract class MongoRepository<T> : MongoRepository<T, string>, IRepository<T>
        where T : IEntity<string>
    {
        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        protected MongoRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        protected MongoRepository(MongoUrl url)
            : base(url)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        protected MongoRepository(MongoUrl url, string collectionName)
            : base(url, collectionName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        protected MongoRepository(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        protected MongoRepository(string connectionString, string collectionName)
            : base(connectionString, collectionName)
        {
        }
    }

}