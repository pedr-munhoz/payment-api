using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using payment_api.Models.Result;

namespace payment_api.Models.Service
{
    public interface IAntecipationDbService
    {
        Task<SolicitationProcessResult> Create(List<int> paymentIds, DateTime solicitationDate);

        Task<AntecipationEntity> StartAnalysis(int id, DateTime startDate);

        Task<AntecipationEntity> Get(int id);

        Task<List<AntecipationEntity>> Get(string status = null);

        Task<AntecipationEntity> ResolvePaymentAntecipation(int antecipationId, List<int> paymentIds, bool approve);
    }
}