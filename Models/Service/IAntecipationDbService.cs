using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public interface IAntecipationDbService
    {
        Task<AntecipationEntity> Create(List<int> ids);

        Task<AntecipationEntity> Update(AntecipationEntity entity);

        Task<AntecipationEntity> Get(int id);

        Task<List<AntecipationEntity>> Get(string status = null);
    }
}