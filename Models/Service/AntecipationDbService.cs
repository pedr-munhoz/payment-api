using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    public class AntecipationDbService : IAntecipationDbService
    {
        private readonly ServerDbContext _dbContext;

        public AntecipationDbService(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<AntecipationEntity> Create(List<int> ids)
        {
            throw new System.NotImplementedException();
        }

        public Task<AntecipationEntity> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<AntecipationEntity>> Get(string status = null)
        {
            if (status == null)
            {
                return await _dbContext.Set<AntecipationEntity>()
                                    .AsNoTracking()
                                    .ToListAsync();
            }
            throw new System.NotImplementedException();
        }

        public Task<AntecipationEntity> Update(AntecipationEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}