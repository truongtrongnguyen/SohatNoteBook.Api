using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.DataService.IRepository;
using SohatNoteBook.Entities.DbSet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.Repository
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<User>> All()
        {
            try
            {
                return await _dbSet.Where(x => x.Status == 1)
                            .AsNoTracking()
                            .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method has generated an error", typeof(UsersRepository));
                return new List<User>();
            }
            
        }
    }
}
