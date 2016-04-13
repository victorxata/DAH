﻿using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.RBAC;

namespace TechTracker.Repositories.Interfaces
{
    public interface IRolesRepository : IRepository<Role>
    {
    }
}