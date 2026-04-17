using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Talage.SDK.EntityFramework.TalageIntegration.Model;
using Talage.SDK.EntityFramework.Repository;

namespace TalageIntegration.API.Services
{
    public interface IQuoteStatusService
    {
        IEnumerable<QuoteStatus> GetAll();
        Task<QuoteStatus> GetById(int id);
        Task<QuoteStatus> Create(QuoteStatus quoteStatus);
        Task Update(QuoteStatus quoteStatus);
        Task Delete(int id);
    }

    public class QuoteStatusService : IQuoteStatusService
    {
        private readonly ITalageIntegrationRepository _repository;

        public QuoteStatusService(ITalageIntegrationRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<QuoteStatus> GetAll()
        {
            return _repository.GetAll<QuoteStatus>().ToList();
        }

        public async Task<QuoteStatus> GetById(int id)
        {
            return await _repository.GetAll<QuoteStatus>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<QuoteStatus> Create(QuoteStatus quoteStatus)
        {
            _repository.Add(quoteStatus);
            await _repository.SaveAsync();
            return quoteStatus;
        }

        public async Task Update(QuoteStatus quoteStatus)
        {
            _repository.MarkForUpdate(quoteStatus);
            await _repository.SaveAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _repository.Delete(entity);
                await _repository.SaveAsync();
            }
        }
    }
}
