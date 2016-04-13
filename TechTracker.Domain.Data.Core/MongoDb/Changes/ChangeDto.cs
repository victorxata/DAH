using System;

namespace TechTracker.Domain.Data.Core.MongoDb.Changes
{
    public class ChangeDto
    {
        public string EntityId { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedDate { get; set; }
        public string PropertyName { get; set; }
        public Object PreviousValue { get; set; }
        public Object NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} -> {2}", PropertyName, PreviousValue ?? "null", NewValue);
        }
    }
}