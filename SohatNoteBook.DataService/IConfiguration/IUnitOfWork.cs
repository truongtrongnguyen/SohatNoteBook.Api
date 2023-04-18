using SohatNoteBook.DataService.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.IConfiguration
{
    public interface IUnitOfWork
    {
        IUsersRepository Users { get; }

        Task CompleteAsync();
    }
}
