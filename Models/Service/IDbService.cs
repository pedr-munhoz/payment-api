using System.Collections.Generic;
using System.Threading.Tasks;

namespace payment_api.Models.Service
{
    public interface IDbService<T>
    {
        Task<List<T>> Get();

        Task<T> Get(int id);

        Task<T> Create(T entity);

        Task<T> Update(T entity);
    }
}