using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.Dto.Incoming.Profile;

namespace SohatNoteBook.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : BaseController
    {
        public ProfileController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                return BadRequest("User Not found");
            }

            var identityId = new Guid(loggedInUser.Id);

            var profile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (profile == null)
            {
                return BadRequest("User Not found");
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("invalid payload");
            }

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

            if (loggedInUser == null)
            {
                return BadRequest("User Not found");
            }

            var identityId = new Guid(loggedInUser.Id);

            var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (userProfile == null)
            {
                return BadRequest("User Not found");
            }

            userProfile.Country = profileDto.Country;
            userProfile.Address = profileDto.Address;
            userProfile.MobileNumber = profileDto.MobileNumber;
            userProfile.Sex = profileDto.Sex;

            var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

            if (isUpdated)
            {
                await _unitOfWork.CompleteAsync();
                return Ok(userProfile);
            }
            return BadRequest("Something went wrong, please try again later");
        }
    }
}
