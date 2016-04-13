using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Core.MongoDb.Changes;

namespace TechTracker.Services.Interfaces
{
    public interface IChangesService
    {
        Task<List<ChangeDto>> GetHistory(string entityId, string tenantId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}

