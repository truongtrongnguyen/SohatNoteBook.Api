using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SohatNoteBook.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.DataService.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
       
        // dotnet new classlib -n NameClass
        // dotnet add SohatNoteBook.Api/SohatNoteBook.Api.csproj reference SohatNoteBook.Authentication/SohatNoteBook.Authentication.csproj

        // cd SohatNoteBook.DataService
        // dotnet ef migrations add "Initial" --startup-project ../SohatNoteBook.Api
        // dotnet ef migrations remove --startup-project ../SohatNoteBook.Api
        // dotnet ef database update --startup-project ../SohatNoteBook.Api
    }
}
