using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.Dto.Errors;

namespace SohatNoteBook.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly UserManager<IdentityUser> _userManager;
        public readonly IMapper _mapper;

        public BaseController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        internal Error PopulateError(int code, string message, string type)
        {
            return new Error()
            {
                Code = code, 
                Message = message,
                Type = type
            };
        }
    }
}
