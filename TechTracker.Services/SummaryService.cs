using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ISummaryRepository _summaryRepository;

        private readonly ILog Log;

        public SummaryService(ISummaryRepository summaryRepository, ILog log)
        {
            _summaryRepository = summaryRepository;
            Log = log;
        }

       public async Task<Summary> GetSummaryAsync()
        {
            var summary = await _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year)).FirstOrDefaultAsync();

           if (summary != null) return summary;

           summary = new Summary {Year = DateTime.UtcNow.Year };
           await _summaryRepository.UpdateAsync(summary);

           return summary;
        }

        public async Task UpdateSummaryAsync(Summary summary)
        {
            await _summaryRepository.UpdateAsync(summary);
        }
    }
}
