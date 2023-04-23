using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.Configuration.Messages;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.DbSet;
using SohatNoteBook.Entities.Dto.Errors;
using SohatNoteBook.Entities.Dto.Generic;
using SohatNoteBook.Entities.Dto.Incoming.Profile;
using SohatNoteBook.Entities.Dto.Outgoing.Profile;

namespace SohatNoteBook.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : BaseController
    {
        public ProfileController(IUnitOfWork unitOfWork,
                                 UserManager<IdentityUser> userManager,
                                 IMapper mapper) : base(unitOfWork, userManager, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            var result = new Result<ProfileDto>();

            if (loggedInUser == null)
            {
                result.Error = PopulateError(400,
                          ErrorsMessage.ProfileMessage.UserNotFound,
                          ErrorsMessage.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (profile == null)
            {
                result.Error = PopulateError(400,
                           ErrorsMessage.ProfileMessage.UserNotFound,
                           ErrorsMessage.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var mapperUser = _mapper.Map<ProfileDto>(profile);

            result.Content = mapperUser;

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            var result = new Result<ProfileDto>();

            if (!ModelState.IsValid)
            {
                result.Error = PopulateError(400,
                            ErrorsMessage.Generic.InvalidPayload,
                            ErrorsMessage.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                result.Error = PopulateError(400,
                           ErrorsMessage.ProfileMessage.UserNotFound,
                           ErrorsMessage.Generic.TypeBadRequest);   
                return BadRequest(result);
            }

            var identityId = new Guid(loggedInUser.Id);

            var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (userProfile == null)
            {
                result.Error = PopulateError(400,
                            ErrorsMessage.ProfileMessage.UserNotFound,
                            ErrorsMessage.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            userProfile.Country = profileDto.Country;
            userProfile.Address = profileDto.Address;
            userProfile.MobileNumber = profileDto.MobileNumber;
            userProfile.Sex = profileDto.Sex;

            var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();

                var mapperUser = _mapper.Map<ProfileDto>(userProfile);

                result.Content = mapperUser;

                return Ok(result);
            }

            result.Error = PopulateError(500,
                                        ErrorsMessage.Generic.SomethingWentWrong,
                                        ErrorsMessage.Generic.UnableToProcess);
            return BadRequest(result);
        }
    }
}
