using System;
using System.Collections.Generic;

namespace TechTracker.Common.Utils
{
    public static class Constants
    {
        public static List<string> IgnorePropertiesList = new List<string> { "LowerTerm", "UpperTerm", "LowerTranslation", "UpperTranslation" };

        public static class MongoDbAppSettings
        {
            public const string GlobalDbMongoConnectionStringName = "MongoServerSettings";
            public const string GlobalDbMongoDatabaseName = "GlobalDatabase";
            public const string GlobalDbMongoDbServer = "MongoServer";
        }

        public static class MongoDbSettings
        {
            public static class CollectionNames
            {
                public const string Users = "users";
                public const string Roles = "roles";
                public const string Countries = "Country";
                public const string CharacterSets = "CharacterSet";
                public const string EmailTemplates = "EmailTemplate";
                public const string Languages = "Language";
                public const string Preregistrations = "PreRegistration";
                public const string RecoverPasswords = "RecoverPassword";
                public const string CustomRoles = "Roles";
                public const string FieldPermissions = "FieldPermissions";
                public const string Changes = "Changes";
                public const string FieldPermissionTypes = "FieldPermissionType";
                public const string Permissions = "Permission";
                public const string UserIdsFieldName = "UserIds";
                public const string Skills = "Skills";
                public const string Accounts = "Accounts";
                public const string Candidates = "Candidates";
                public const string Opportunities = "Opportunities";
                public const string Summary = "Summaries";
            }

        }
        public static class Errors
        {
            public const string DuplicateKey = "Cannot insert current entity because it has an existing key in the collection";
            public const string EntryNotFound = "Entry not found";
            public const string CannotParseId = "Cannot parse given Id";

            public static string CannotExecuteOnThisRepositoryWithoutTenantId = "Cannot Execute On This Repository Without TenantId";
            public static string CannotExecuteOnThisRepositoryWithoutTenantIdNoImplementation = "Cannot Execute On This Repository Without Tenant Id No Implementation";
            public static string CannotExecuteOnThisRepositoryWithTenantId = "Cannot Execute On This Repository With TenantId";
        }
       
    }
}