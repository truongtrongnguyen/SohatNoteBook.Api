using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> All();

        // Get specific entity based on Id
        Task<T> GetById(Guid id);

        Task<bool> Add(T entity);

        Task<bool> Delete(Guid Id, string userId);

        // Update or Add if it does not exist
        Task<bool> Upsert(T entity);
    }
}
