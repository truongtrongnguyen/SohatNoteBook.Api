using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected AppDbContext _context;
        protected readonly ILogger _logger;
        internal DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context, ILogger logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _logger = logger;
        }

        public virtual async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual Task<bool> Delete(Guid Id, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
