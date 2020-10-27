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

        Task<AntecipationEntity> Get(int id);

        Task<List<AntecipationEntity>> Get(string status = null);

        Task<AnticipationResult> ResolvePaymentAntecipation(int antecipationId, List<int> paymentIds, bool approve);
    }
}