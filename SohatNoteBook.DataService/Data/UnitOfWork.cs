using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.DataService.IRepository;
using SohatNoteBook.DataService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public IUsersRepository Users { get; private set; }
        public IRefreshTokensRepository RefreshTokens { get; private set; }
        public IHealthDatasRepository HealthData { get; private set; }


        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("db_logs");

            Users = new UsersRepository(_context, _logger);
            RefreshTokens = new RefreshTokensRepository(_context, _logger);
            HealthData = new HealthDataRepository(_context, _logger);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
