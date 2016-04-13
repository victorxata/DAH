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
    public class CandidatesService : ICandidatesService
    {
        private readonly ICandidatesRepository _candidatesRepository;

        private readonly ILog Log;

        public CandidatesService(ICandidatesRepository candidatesRepository, ILog log)
        {
            _candidatesRepository = candidatesRepository;
            Log = log;
        }

        public async Task<IEnumerable<Candidate>> GetCandidateAsync()
        {
            return await _candidatesRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Candidate> GetCandidateByIdAsync(string candidateId)
        {
            return await _candidatesRepository.GetByIdAsync(candidateId);
        }

        public async Task<Candidate> AddCandidateAsync(Candidate candidate)
        {
            return await _candidatesRepository.AddAsync(candidate);
        }

        public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
        {
            return await _candidatesRepository.UpdateAsync(candidate);
        }

        public async Task DeleteCandidateAsync(string candidateId)
        {
            await _candidatesRepository.DeleteAsync(candidateId);
        }
    }
}
