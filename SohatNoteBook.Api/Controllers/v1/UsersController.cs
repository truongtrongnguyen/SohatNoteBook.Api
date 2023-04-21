using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.DbSet;
using SohatNoteBook.Entities.Dto.Incoming;

namespace SohatNoteBook.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }

        [HttpGet]
        [HttpHead]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.All();
            return Ok(users);
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> AddUser(UserDto user)
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

            await _unitOfWork.Users.Add(_user);
            await _unitOfWork.CompleteAsync();

            return CreatedAtRoute("GetUser", new { _user.Id }, user);
        }

        [HttpGet]
        [Route("GetById", Name = "GetUser")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var user = await _unitOfWork.Users.GetById(Id);
            return Ok(user);
        }
    }
}
