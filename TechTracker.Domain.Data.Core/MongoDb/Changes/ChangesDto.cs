using System;

namespace TechTracker.Domain.Data.Core.MongoDb.Changes
{
    public class ChangesDto
    {
        public string Id { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}