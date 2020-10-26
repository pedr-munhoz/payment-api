using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using payment_api.Infrastructure.Database;

namespace payment_api.Models.Service
{
    public abstract class BaseDbService<T> : IDbService<T>
        where T : DbEntity
    {
        private readonly ServerDbContext _dbContext;

        protected BaseDbService(ServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Create(T entity)
        {
            await _dbContext.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<List<T>> Get()
            => await _dbContext.Set<T>()
                    .ToListAsync();

        public async Task<T> Get(int id)
            => await _dbContext.Set<T>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

        public Task<T> Update(T entity)
        {
            throw new System.NotImplementedException();
        }
    }
}