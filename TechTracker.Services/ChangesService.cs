using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using log4net;
using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Common.Utils.Extensions;
using TechTracker.Domain.Data.Core.MongoDb.Changes;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class ChangesService : IChangesService
    {
        private readonly IChangesRepository _changesRepository;

        private readonly ILog Log;

        public ChangesService(ILog logger, IChangesRepository changesRepository)
        {
            Log = logger;
            _changesRepository = changesRepository;
        }

        public async Task<List<ChangeDto>> GetHistory(string entityId, string tenantId, DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var filter = Builders<Change>.Filter;
            var result = new List<ChangeDto>();
            var query = filter.Eq(x => x.EntityId, entityId);

            if (fromDate != null)
                query = filter.And(query,
                    filter.Gte(x => x.DateTime, fromDate));

            if (toDate != null)
                query = filter.And(query,
                    filter.Lte(x => x.DateTime, toDate));

            var collection = _changesRepository.Collection;
            var changesList = await collection.Find(query).ToListAsync();

            foreach (var change in changesList)
            {
                AddChanges(change, result);
            }

            return result;
        }

        private static void AddChanges(Change change, List<ChangeDto> changes)
        {
            var result = new ChangeDto
            {
                EntityId = change.EntityId,
                ChangedBy = change.User,
                ChangedDate = change.DateTime
            };

            switch (change.Type)
            {
                    case ChangeType.Create:
                        AddChangesCreate(result, change.NewEntity, changes, String.Empty);
                    break;
                    case ChangeType.Delete:
                    AddChangesDelete(result, change.OldEntity, changes, String.Empty);
                    break;
                default:
                    AddChangesUpdate(result, change.NewEntity, change.OldEntity, changes, String.Empty);
                    break;
            }
        }

        private static void AddChangesUpdate(ChangeDto change, object newObjectToFilter, object oldObjectToFilter,
            List<ChangeDto> changes, string prefix)
        {
            if (!String.IsNullOrEmpty(prefix))
                prefix = prefix + ".";
            var nd = (newObjectToFilter is ExpandoObject) ? newObjectToFilter : newObjectToFilter.ToDynamic();
            var od = (oldObjectToFilter is ExpandoObject) ? oldObjectToFilter : oldObjectToFilter.ToDynamic();
            var newDict = (IDictionary<string, object>) nd;
            var oldDict = (IDictionary<string, object>) od;

            var properties = newDict.Keys;

            foreach (var propertyInfo in properties)
            {
                if (Constants.IgnorePropertiesList.Contains(propertyInfo)) continue;

                try
                {
                    if (newDict[propertyInfo].ToString() != oldDict[propertyInfo].ToString())
                    {
                        var newChange = new ChangeDto
                        {
                            EntityId = change.EntityId,
                            ChangedBy = change.ChangedBy,
                            ChangedDate = change.ChangedDate,
                            PropertyName = $"{prefix}{propertyInfo}",
                            NewValue = newDict[propertyInfo],
                            PreviousValue = oldDict[propertyInfo]
                        };
                        changes.Add(newChange);
                    }
                }
                catch (Exception)
                {
                    
                    //throw;
                }
                
            }
        }

        private static void AddChangesCreate(ChangeDto change, object objectToFilter, List<ChangeDto> changes, string prefix)
        {
            if (!String.IsNullOrEmpty(prefix))
                prefix = prefix + ".";

            var nd = (objectToFilter is ExpandoObject) ? objectToFilter : objectToFilter.ToDynamic();

            var dict = (IDictionary<string, object>)nd;

            var properties = dict.Keys;

            foreach (var propertyInfo in properties)
            {
                if (Constants.IgnorePropertiesList.Contains(propertyInfo)) continue;

                if (dict[propertyInfo] is List<Object>)
                {
                    foreach (var subitem in (List<Object>)dict[propertyInfo])
                    {
                        if (subitem is ExpandoObject)
                        {
                            AddChangesCreate(change, subitem, changes, $"{prefix}{propertyInfo}");
                        }
                    }
                }
                else
                {
                    var newChange = new ChangeDto
                    {
                        EntityId = change.EntityId,
                        ChangedBy = change.ChangedBy,
                        ChangedDate = change.ChangedDate,
                        PropertyName = $"{prefix}{propertyInfo}",
                        NewValue = dict[propertyInfo]
                    };
                    changes.Add(newChange);
                }
            }
        }

        private static void AddChangesDelete(ChangeDto change, object objectToFilter, List<ChangeDto> changes, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
                prefix = prefix + ".";
            var nd = (objectToFilter is ExpandoObject) ? objectToFilter : objectToFilter.ToDynamic();

            var dict = (IDictionary<string, object>)nd;
            var properties = dict.Keys;

            foreach (var propertyInfo in properties)
            {
                if (Constants.IgnorePropertiesList.Contains(propertyInfo)) continue;

                if (dict[propertyInfo] is List<object>)
                {
                    foreach (var subitem in (List<object>)dict[propertyInfo])
                    {
                        if (subitem is ExpandoObject)
                        {
                            AddChangesDelete(change, subitem, changes, $"{prefix}{propertyInfo}");
                        }
                    }
                }
                else
                {
                    var newChange = new ChangeDto
                    {
                        EntityId = change.EntityId,
                        ChangedBy = change.ChangedBy,
                        ChangedDate = change.ChangedDate,
                        PropertyName = $"{prefix}{propertyInfo}",
                        PreviousValue = dict[propertyInfo]
                    };
                    changes.Add(newChange);
                }
            }
        }


        //private static IEnumerable<ChangeDto> AnalyzeChange(Change change)
        //{
        //    var result = new List<ChangeDto>();

        //    AddChanges(change, result);
            
        //    //var props = ((Entry)change.NewEntity).GetType().GetProperties();
        //    //var props = typeof (Entry).GetProperties();

        //    //if (change.Type == ChangeType.Create)
        //    //{
        //    //    foreach (var prop in props)
        //    //    {
        //    //        var entity = new Entry();
        //    //        var e = (ExpandoObject)change.NewEntity;
        //    //        Field
        //    //        result.Add(cd);
        //    //    }
        //    //    //result.AddRange(props.Where(prop=> (prop.Name != "LowerTerm") && (prop.Name != "UpperTerm")).Select(prop => new ChangeDto
        //    //    //{
        //    //    //    EntityId = change.EntityId, 
        //    //    //    ChangedBy = change.User, 
        //    //    //    ChangedDate = change.DateTime, 
        //    //    //    PropertyName = prop.Name, 
        //    //    //    NewValue = prop.GetValue((change.NewEntity as Entry), null)
        //    //    //}));
        //    //}

        //    if (change.Type == ChangeType.Delete)
        //    {
        //        result.AddRange(props.Where(prop => (prop.Name != "LowerTerm") && (prop.Name != "UpperTerm")).Select(prop => new ChangeDto
        //        {
        //            EntityId = change.EntityId, 
        //            ChangedBy = change.User, 
        //            ChangedDate = change.DateTime, 
        //            PropertyName = prop.Name,
        //            PreviousValue = prop.GetValue((change.OldEntity as Entry), null)
        //        }));
        //    }

        //    if (change.Type != ChangeType.Update) 
        //        return result;

        //    foreach (var prop in props)
        //    {
        //        if (prop.Name != "Translations")
        //        {
        //            var oldV = prop.GetValue((change.OldEntity as Entry), null);
        //            var newV = prop.GetValue((change.NewEntity as Entry), null);

        //            if (!HasChanges(oldV, newV))
        //                continue;

        //            if ((prop.Name != "LowerTerm") && (prop.Name != "UpperTerm"))
        //            {
        //                result.Add(GetChangeDto(prop.Name, oldV, newV, change.EntityId, change.User, change.DateTime));
        //            }
        //        }
        //        else
        //        {
        //            var translationProperties = typeof (Target).GetProperties();
        //            var oldTargets = ((List<Target>)prop.GetValue((change.OldEntity as Entry)));
        //            var newTargets = ((List<Target>)prop.GetValue((change.NewEntity as Entry)));
        //            foreach (var target in newTargets)
        //            {
        //                var oldTarget = oldTargets.FirstOrDefault(x => x.CorrelationId == target.CorrelationId); //Find(t => t.Created_at == target.Created_at && t.CreatedBy == target.CreatedBy && t.LanguageCode == target.LanguageCode);

        //                foreach (var translationProperty in translationProperties)
        //                {
        //                    var oldV = oldTarget == null ? null : translationProperty.GetValue(oldTarget, null);
        //                    var newV = translationProperty.GetValue(target, null);

        //                    if (!HasChanges(oldV, newV))
        //                        continue;

        //                    if ((translationProperty.Name != "LowerTranslation") && (translationProperty.Name != "UpperTranslation"))
        //                    {
        //                        result.Add(GetChangeDto(string.Format("Target.{0}", translationProperty.Name), oldV,
        //                            newV, change.EntityId, change.User, change.DateTime));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        //private static ChangeDto GetChangeDto(string propName, object oldV, object newV, string entityId, string userName, DateTime changeDateTime)
        //{
        //    var item = new ChangeDto
        //    {
        //        EntityId = entityId,
        //        ChangedBy = userName,
        //        ChangedDate = changeDateTime,
        //        PropertyName = propName,
        //        PreviousValue = oldV,
        //        NewValue = newV
        //    };

        //    return item;
        //}

        //private static bool HasChanges(object oldV, object newV)
        //{
        //    bool changed;
        //    if ((oldV == null) && (newV == null))
        //        return false;

        //    if ((oldV == null) || (newV == null)) 
        //        return true;

        //    if (newV.GetType() == Type.GetType("System.Collections.Generic.List`1[System.String]"))
        //    {
        //        if (oldV.GetType() != Type.GetType("System.Collections.Generic.List`1[System.String]"))
        //            return true;

        //        var oldList = (List<string>)oldV;
        //        var newList = (List<string>)newV;
        //        changed = !newList.SequenceEqual(oldList);
        //    }
        //    else
        //    {
        //        changed = !oldV.ToString().Equals(newV.ToString());
        //    }

        //    return changed;
        //}

        
    }

   
}

