﻿using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Repositories.Interfaces
{
    public interface ISkillsRepository : IRepository<Skill>
    {
    }
}