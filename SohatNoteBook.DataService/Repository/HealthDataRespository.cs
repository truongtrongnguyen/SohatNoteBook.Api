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
    public class HealthDataRepository : GenericRepository<HealthData>, IHealthDatasRepository
    {
        public HealthDataRepository(AppDbContext context, ILogger logger) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<HealthData>> All()
        {
            try
            {
                return await _dbSet.Where(x => x.Status == 1)
                            .AsNoTracking()
                            .ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Repo} All method has generated an error", typeof(HealthDataRepository));
                return new List<HealthData>();
            }
        }

        public async Task<bool> UpdateHealthData(HealthData healthData)
        {
            try
            {
                var existing = await _dbSet.Where(x => x.Status == 1 && x.Id == healthData.Id)
                                            .FirstOrDefaultAsync();
                if(existing == null)
                {
                    return false;
                }

                existing.Height = healthData.Height;
                existing.Weight = healthData.Weight;
                existing.Race = healthData.Race;
                existing.BloodType = healthData.BloodType;
                existing.UseGlasses = healthData.UseGlasses;
                existing.UpdateDate = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateHealthData method has generated an error", typeof(HealthDataRepository));
                return false;
            }
        }
    }
}
