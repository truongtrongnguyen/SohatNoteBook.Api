using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.Entities.DbSet;
using SohatNoteBook.Entities.Dto.Incoming;

namespace SohatNoteBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // Get
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.Where(x => x.Status == 1).ToList();
            return Ok(users);
        }

        [HttpPost]
        [Route("CreateUser")]
        public IActionResult AddUser(UserDto user)
        {
            var _user = new User()
            {
                Status = 1,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = Convert.ToDateTime(user.DateOfBirth),
                Country = user.Country
            };
            _context.Users.Add(_user);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("GetById")]
        public IActionResult GetById(Guid Id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == Id);
            return Ok(user);
        }
    }
}
