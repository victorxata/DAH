using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Validation;

namespace TechTracker.Domain.Data.Models.Business
{
    [DataContract]
    public class Candidate : Entity
    {
        [DataMember]
        [Required]
        public string Name { get; set; }

        [DataMember]
        public List<Skill> Skills { get; set; }

        [DataMember]
        public List<CandidateTrack> Tracks { get; set; } 

        public Candidate()
        {
            Skills = new List<Skill>();
            Tracks = new List<CandidateTrack>();
        }
    }

    [DataContract]
    public class CandidateTrack : Entity
    {
        [DataMember]
        [Required]
        public string AccountId { get; set; }

        [DataMember]
        [Required]
        public string OpportunityId { get; set; }

        [DataMember]
        public DateTime? LatestUpdate {
            get
            {
                var phase = Phases.OrderByDescending(x => x.Date).FirstOrDefault();
                return phase?.Date;
            }
        }

        [DataMember]
        public HireStatus? Status {
            get
            {
                var phase = Phases.OrderByDescending(x => x.Date).FirstOrDefault();
                return phase?.Status;
            }
        }

        [DataMember]
        public string HireReason { get; set; }

        [DataMember]
        public List<HirePhases> Phases { get; set; }

        [DataMember]
        [EnumeratorIncludedIn(typeof(FinalStatus))]
        public FinalStatus FinalStatus { get; set; }

        public CandidateTrack()
        {
            Phases = new List<HirePhases>();
            FinalStatus = FinalStatus.ActiveInProgress;
        }

    }

    public enum FinalStatus
    {
        ActiveInProgress = 1,
        ClosedHired = 2,
        ClosedNotHired = 3
    }

    [DataContract]
    public class HirePhases
    {
        [DataMember]
        [Required]
        public DateTime Date { get; set; }

        [DataMember]
        [EnumeratorIncludedIn(typeof(HireStatus))]
        public HireStatus Status { get; set; }

        public HirePhases()
        {
            Date = DateTime.UtcNow;;
        }
    }

    public enum HireStatus
    {
        HrPipeline = 1,
        PassedHrScreening = 3,
        Failed = 5,
        FirstInterview = 6,
        SecondInterview = 8,
        FailedInterview = 9,
        DecisionPending = 10,
        Offer = 11, 
        Withdrawn = 12,
        OfferDeclined = 13,
        Hired = 15
    }
}
