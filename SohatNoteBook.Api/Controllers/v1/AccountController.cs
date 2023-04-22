using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SohatNoteBook.Authentication.Configuration;
using SohatNoteBook.Authentication.Models.DTO.Generic;
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
        private readonly TokenValidationParameters _tokenValidationParameters;
        public AccountController(IUnitOfWork unitOfWork,
                                UserManager<IdentityUser> userManager,
                                IOptionsMonitor<JwtConfig> jwtConfig,
                                TokenValidationParameters tokenValidationParameters)
                                : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrantionRequestDto registrantionDto)
        {
            // check the model or object wse are receiving is valid
            if (ModelState.IsValid)
            {
                // check if email already  exist
                var userExist = await _userManager.FindByEmailAsync(registrantionDto.Email);
                if (userExist != null)
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
                if (!result.Succeeded)
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
                    IdentityId = new Guid(newUser.Id),
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
                var tokenData = await GenerateJwtToken(newUser);

                return Ok(new UserRegistrantionResponseDto()
                {
                    Success = true,
                    Token = tokenData.JwtToken,
                    RefreshToken = tokenData.RefreshToken
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

                if (isCorrect)
                {
                    // we need generate a jwt token
                    var tokenData = await GenerateJwtToken(userExist);

                    return Ok(new UserLoginResponseDto()
                    {
                        Success = true,
                        Token = tokenData.JwtToken,
                        RefreshToken = tokenData.RefreshToken
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

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequetsDto tokenRequestDto)
        {
            if (ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(tokenRequestDto);
                if (result == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid Token"
                        }
                    });
                }
                else
                {
                    return Ok(result);
                }
            }
            // Invalid Object
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

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequetsDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // We need to check the validity of the token
                _tokenValidationParameters.ValidateLifetime = false;    // for testing

                // Validation format
                var tokenVerifiecation = jwtTokenHandler.ValidateToken(tokenRequest.Token,
                                                                    _tokenValidationParameters,
                                                                    out var validatedToken);

                // We need to validate the result that has been generated for us
                // Validate if the string is an actual JWT token not a random string
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    // check if the jwt token is created with the same algorithms as our jwt token
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }

                // We need to check the expiry date of t the token
                var utcExpiryDate = long.Parse(tokenVerifiecation.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                // convert to date to check
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                // checking if the jwt token has expiry
                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "token has not yet expiry"
                        }
                    };
                }

                var storeToken = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequest.RefreshToken);
                if (storeToken == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid token"
                        }
                    };
                }

                // check if refresh token has been used or not
                if (storeToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh Token has been used, if cannot be reused"
                        }
                    };
                }

                if (storeToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh Token has been Revoked, if cannot be used"
                        }
                    };
                }

                // check the expiry date of a refresh token
                var jti = tokenVerifiecation.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storeToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh Token has expired, please login again"
                        }
                    };
                }

                if (storeToken.ExpiryTime < DateTime.Now)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Expiry token"
                        }
                    };
                }
                storeToken.IsUsed = true;
                var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(storeToken);
                if (updateResult)
                {
                    await _unitOfWork.CompleteAsync();

                    // Get the user to generate a new jwt token 
                    var dbUser = await _userManager.FindByIdAsync(storeToken.UserId);

                    if (dbUser == null)
                    {
                        return new AuthResult()
                        {
                            Success = false,
                            Errors = new List<string>()
                            {
                                "Error processing request"
                            }
                        };
                    }

                    // Generate a token 
                    var tokenData = await GenerateJwtToken(dbUser);
                    return new AuthResult()
                    {
                        Token = tokenData.JwtToken,
                        RefreshToken = tokenData.RefreshToken,
                        Success = true
                    };
                }

                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Error processing request"
                        }
                };
            }
            catch (Exception e)
            {
                // TODO: Add better error handling
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Server Error",
                            e.Message
                        }
                };
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Set the time to 1, Jan, 1970
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            // add the number os seconds from 1 Jan 1970
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;

        }

        private async Task<TokenData> GenerateJwtToken(IdentityUser user)
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
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),      // todo updateth expiration time to minutes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)  // todo review the algorithms
            };

            // generate the security obj token  
            var token = jwtHandler.CreateToken(tokenDescription);

            // convert the security obj token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            // Generate a refresh token 
            var refreshToken = new RefreshToken()
            {
                AddedDate = DateTime.UtcNow,
                UserId = user.Id,
                Token = $"{RandomStringGenerator(20)}_{Guid.NewGuid()}",
                IsUsed = false,
                IsRevoked = false,
                Status = 1,
                JwtId = token.Id,
                ExpiryTime = DateTime.UtcNow.AddMonths(6)
            };

            await _unitOfWork.RefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            var tokenData = new TokenData
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };

            return tokenData;
        }

        private string RandomStringGenerator(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new String(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
}
