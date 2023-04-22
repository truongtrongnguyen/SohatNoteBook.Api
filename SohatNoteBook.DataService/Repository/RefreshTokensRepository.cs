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
    public class RefreshTokensRepository : GenericRepository<RefreshToken>, IRefreshTokensRepository
    {
        public RefreshTokensRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<RefreshToken>> All()
        {
            try
            {
                return await _dbSet.Where(x => x.Status == 1)
                            .AsNoTracking()
                            .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method has generated an error", typeof(RefreshTokensRepository));
                return new List<RefreshToken>();
            }
            
        }

        public async Task<RefreshToken> GetByRefreshToken(string refreskToken)
        {
            try
            {
                return await _dbSet.Where(x => x.Token.ToLower() == refreskToken.ToLower())
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokensRepository));
                return null;
            }
        }

        public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
        {
            try
            {
                var token = await _dbSet.Where(x => x.Token.ToLower() == refreshToken.Token.ToLower())
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
                if (token == null) return false;

                token.IsUsed = refreshToken.IsUsed;
                _dbSet.Update(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokensRepository));
                return false;
            }
        }
    }
}
