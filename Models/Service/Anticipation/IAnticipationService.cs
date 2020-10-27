using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public interface IAnticipationService
    {
        Task<AnticipationResult> Create(List<int> paymentIds, DateTime solicitationDate);

        Task<AnticipationResult> StartAnalysis(int id, DateTime startDate);

        Task<AnticipationEntity> Get(int id);

        Task<List<AnticipationEntity>> Get(string status = null);

        Task<AnticipationResult> ResolvePaymentAnticipation(int anticipationId, List<int> paymentIds, bool approve, DateTime timestamp);
    }
}