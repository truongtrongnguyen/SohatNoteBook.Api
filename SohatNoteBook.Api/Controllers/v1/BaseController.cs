using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.DataService.IConfiguration;

namespace SohatNoteBook.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly UserManager<IdentityUser> _userManager;

        public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
    }
}
