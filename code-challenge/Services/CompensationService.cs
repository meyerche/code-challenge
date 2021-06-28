using System;
using challenge.Models;
using challenge.Repositories;
using Microsoft.Extensions.Logging;

namespace challenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }
        
        public Compensation GetById(string id)
        {
            return String.IsNullOrEmpty(id) ? null : _compensationRepository.GetById(id);
        }

        public Compensation Create(Compensation compensation)
        {
            if(compensation != null)
            {
                compensation = _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }

            return compensation;
        }
    }
}