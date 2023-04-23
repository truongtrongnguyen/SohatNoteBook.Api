using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.Api.Profiles;
using SohatNoteBook.Configuration.Messages;
using SohatNoteBook.DataService.Data;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.DbSet;
using SohatNoteBook.Entities.Dto.Generic;
using SohatNoteBook.Entities.Dto.Incoming;
using SohatNoteBook.Entities.Dto.Outgoing.Profile;
using System.Collections.Generic;

namespace SohatNoteBook.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork,
                            UserManager<IdentityUser> userManager,
                            IMapper mapper) : base(unitOfWork, userManager, mapper)
        {
            
        }

        [HttpGet]
        [HttpHead]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.All();

            var result = new PagedResult<User>();

            result.Content = users.ToList();

            result.ResultCount = users.Count();

            return Ok(result);
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var _mapperUser = _mapper.Map<User>(user);
            
            await _unitOfWork.Users.Add(_mapperUser);
            await _unitOfWork.CompleteAsync();

            // Todo: Add the correct return to this action
            var result = new Result<UserDto>();
            result.Content = user;

            return CreatedAtRoute("GetUser", new { _mapperUser.Id }, result);
        }

        [HttpGet]
        [Route("GetById", Name = "GetUser")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var user = await _unitOfWork.Users.GetById(Id);

            var result = new Result<ProfileDto>();

            if(user != null)
            {
                var mapperUser = _mapper.Map<ProfileDto>(user);

                result.Content = mapperUser;

                return Ok(result);
            }

            result.Error = PopulateError(404,
                                        ErrorsMessage.UserMessage.UserNotFound,
                                        ErrorsMessage.Generic.ObjectNotFound);
            return BadRequest(result);
        }
    }
}
