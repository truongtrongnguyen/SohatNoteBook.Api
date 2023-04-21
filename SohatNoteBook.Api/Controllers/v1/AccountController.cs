using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SohatNoteBook.Authentication.Configuration;
using SohatNoteBook.Authentication.Models.DTO.InComing;
using SohatNoteBook.Authentication.Models.DTO.Outgoing;
using SohatNoteBook.DataService.IConfiguration;
using SohatNoteBook.Entities.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SohatNoteBook.Api.Controllers.v1
{
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AccountController(IUnitOfWork unitOfWork,
                                UserManager<IdentityUser> userManager,
                                IOptionsMonitor<JwtConfig> jwtConfig)
                                : base (unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrantionRequestDto registrantionDto)
        {
            // check the model or object wse are receiving is valid
            if(ModelState.IsValid)
            {
                // check if email already  exist
                var userExist = await _userManager.FindByEmailAsync(registrantionDto.Email);
                if(userExist != null)
                {
                    return BadRequest(new UserRegistrantionResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Email already in use"
                        }
                    });
                }

                // Add the user
                var newUser = new IdentityUser()
                {
                    Email = registrantionDto.Email,
                    UserName = registrantionDto.Email,
                    EmailConfirmed = true
                };

                // Adding User to table 
                var result = await _userManager.CreateAsync(newUser, registrantionDto.Passwork);
                if(!result.Succeeded)
                {
                    return BadRequest(new UserRegistrantionResponseDto()
                    {
                        Success = result.Succeeded,
                        Errors = result.Errors.Select(x => x.Description).ToList()
                    });
                }

                // Adding user to the databse
                var _user = new User()
                {
                    IdentityId  = new Guid(newUser.Id),
                    Status = 1,
                    FirstName = registrantionDto.FirstName,
                    LastName = registrantionDto.LastName,
                    Email = registrantionDto.Email,
                    Phone = "",
                    DateOfBirth = DateTime.UtcNow,  // Convert.ToDateTime(user.DateOfBirth),
                    Country = ""
                };

                await _unitOfWork.Users.Add(_user);
                await _unitOfWork.CompleteAsync();

                // Create a jwt token
                var token = GenerateJwtToken(newUser);

                return Ok(new UserRegistrantionResponseDto()
                {
                    Success = true,
                    Token = token
                });
            }
            else
            {
                return BadRequest(new UserRegistrantionResponseDto
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
        {
            if (ModelState.IsValid)
            {
                // 1 - Check if emial exist
                var userExist = await _userManager.FindByEmailAsync(loginDto.Email);
                if (userExist == null)
                {
                    return BadRequest(new UserRegistrantionResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                    });
                }

                // 2 - Check if user has a valid password
                var isCorrect = await _userManager.CheckPasswordAsync(userExist, loginDto.Passwork);
                
                if(isCorrect)
                {
                    // we need generate a jwt token
                    var jwtToken = GenerateJwtToken(userExist);

                    return Ok(new UserLoginResponseDto()
                    {
                        Success = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    // Password doesn't match
                    return BadRequest(new UserRegistrantionResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                    });
                }
            }
            else
            {
                return BadRequest(new UserRegistrantionResponseDto
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            // the handler is going to  be responseible for creating the token
            var jwtHandler = new JwtSecurityTokenHandler();

            // Get the security key
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),     // unique id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(3),      // todo updateth expiration time to minutes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  // todo review the algorithms
            };

            // generate the security obj token  
            var token = jwtHandler.CreateToken(tokenDescription);

            // convert the security obj token into a string\
            var jwtToken = jwtHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
