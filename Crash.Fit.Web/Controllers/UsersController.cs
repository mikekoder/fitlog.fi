using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Crash.Fit.Api.Models.Users;
using Crash.Fit.Web.Models.Auth;
using Microsoft.AspNetCore.Antiforgery;
using Crash.Fit.Logging;
using Crash.Fit.Profile;
using Crash.Fit.Api.Models.Profile;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Crash.Fit.Nutrition;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Crash.Fit.Measurements;
using Crash.Fit.Training;

namespace Crash.Fit.Web.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IProfileRepository _profileRepository;
        private readonly IConfigurationRoot _configuration;
        private readonly INutritionRepository _nutritionRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMeasurementRepository _measurementRepository;

        public UsersController(UserManager<User> userManager,SignInManager<User> signInManager, IProfileRepository profileRepository, IMeasurementRepository measurementRepository, ILogRepository logger, IConfigurationRoot configuration, INutritionRepository nutritionRepository, RoleManager<Role> roleManager) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepository = profileRepository;
            _configuration = configuration;
            _nutritionRepository = nutritionRepository;
            _roleManager = roleManager;
            _measurementRepository = measurementRepository;
            /*
            var roles = new[]
            {
                Role.Admin,
                Role.UserAdmin,
                Role.FoodAdmin
            };

            if(! _roleManager.RoleExistsAsync("admin").Result)
            {
                _roleManager.CreateAsync(new Role {Name = "admin" });
                _roleManager.
            }
            */
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var profile = _profileRepository.GetProfile(CurrentUserId);
            if(profile == null)
            {
                profile = new Profile.Profile
                {
                    UserId = CurrentUserId
                };
            }
            var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            var logins = await _userManager.GetLoginsAsync(user);
            var hasPassword = await _userManager.HasPasswordAsync(user);

            var result = AutoMapper.Mapper.Map<ProfileResponse>(profile);

            var measures = _measurementRepository.GetMeasures(CurrentUserId);
            result.Weight = measures.FirstOrDefault(m => m.Id == Constants.Measurements.WeightId)?.LatestValue;
            result.Height = measures.FirstOrDefault(m => m.Id == Constants.Measurements.HeightId)?.LatestValue;
            result.Rmr = measures.FirstOrDefault(m => m.Id == Constants.Measurements.RmrId)?.LatestValue;

            result.Logins = logins.Select(l => l.LoginProvider).ToArray();
            result.HasPassword = hasPassword;
            result.Username = user.UserName;

            return Ok(result);
        }
        [HttpPut("me")]
        public async Task<IActionResult> SaveProfile([FromBody]ProfileRequest model)
        {
            var profile = _profileRepository.GetProfile(CurrentUserId);
            if (profile == null)
            {
                profile = new Profile.Profile
                {
                    UserId = CurrentUserId
                };
            }
            profile.DoB = model.DoB;
            profile.Gender = model.Gender;

            var measures = _measurementRepository.GetMeasures(CurrentUserId);
            if (model.Weight.HasValue && model.Weight != measures.FirstOrDefault(m => m.Id == Constants.Measurements.WeightId)?.LatestValue)
            {
                _measurementRepository.CreateMeasurement(new Measurement
                {
                    MeasureId = Constants.Measurements.WeightId,
                    UserId = CurrentUserId,
                    Time = DateTimeOffset.Now,
                    Value = model.Weight.Value
                });
            }
            if (model.Height.HasValue && model.Height != measures.FirstOrDefault(m => m.Id == Constants.Measurements.HeightId)?.LatestValue)
            {
                _measurementRepository.CreateMeasurement(new Measurement
                {
                    MeasureId = Constants.Measurements.HeightId,
                    UserId = CurrentUserId,
                    Time = DateTimeOffset.Now,
                    Value = model.Height.Value
                });
            }
            if (model.Rmr.HasValue && model.Rmr != measures.FirstOrDefault(m => m.Id == Constants.Measurements.RmrId)?.LatestValue)
            {
                _measurementRepository.CreateMeasurement(new Measurement
                {
                    MeasureId = Constants.Measurements.RmrId,
                    UserId = CurrentUserId,
                    Time = DateTimeOffset.Now,
                    Value = model.Rmr.Value
                });
            }

            _profileRepository.SaveProfile(profile);

            return await GetProfile();
        }
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var profile = _profileRepository.GetProfile(CurrentUserId);
            profile.DoB = null;
            profile.Gender = null;

            _profileRepository.SaveProfile(profile);

            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(model.Username);
            if(user == null)
            {
                return BadRequest();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded || result.IsLockedOut)
            {
                return BadRequest();
            }

            return TokenResult(user.Id);
        }
        [HttpPut("login")]
        public async Task<IActionResult> UpdateLogin([FromBody]ChangeLoginRequest model)
        {
            var user = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            if(user == null)
            {
                return Unauthorized();
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return BadRequest();   
                }
            }
            else
            {
                var createPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!createPasswordResult.Succeeded)
                {
                    return BadRequest();
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Username) && model.Username != user.UserName)
            {
                var setUsernameResult = await _userManager.SetUserNameAsync(user, model.Username);
                if (!setUsernameResult.Succeeded)
                {
                    return BadRequest();
                }
            }

            return Ok();
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterRequest model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { ErrorCodes = result.Errors.Select(e => e.Code) });
            }

            InitProfile(user);

            return TokenResult(user.Id);
        }

        private void InitProfile(User user)
        {
            var profile = new Profile.Profile
            {
                UserId = user.Id
            };
            _profileRepository.SaveProfile(profile);

            
            try
            {
                var mealDefinitions = new[]
                {
                    new Nutrition.MealDefinition
                    {
                        UserId = user.Id,
                        Name = "Aamiainen",
                        Start = new TimeSpan(6,0,0),
                        End = new TimeSpan(10,0,0)
                    },
                    new Nutrition.MealDefinition
                    {
                        UserId = user.Id,
                        Name = "Lounas",
                        Start = new TimeSpan(10,0,0),
                        End = new TimeSpan(14,0,0)
                    },
                    new Nutrition.MealDefinition
                    {
                        UserId = user.Id,
                        Name = "Päivällinen",
                        Start = new TimeSpan(14,0,0),
                        End = new TimeSpan(19,0,0)
                    },
                    new Nutrition.MealDefinition
                    {
                        UserId = user.Id,
                        Name = "Iltapala",
                        Start = new TimeSpan(19,0,0),
                        End = new TimeSpan(23,0,0)
                    },
                    new Nutrition.MealDefinition
                    {
                        UserId = user.Id,
                        Name = "Välipalat",
                        Start = null,
                        End = null
                    }
                };
                _nutritionRepository.SaveMealDefinitions(mealDefinitions);
            
                _nutritionRepository.SaveHomeNutrients(user.Id, new[] 
                {
                    Constants.Nutrition.EnergyDistributionId,
                    Constants.Nutrition.EnergyKcalId,
                    Constants.Nutrition.ProteinId,
                    Constants.Nutrition.CarbId,
                    Constants.Nutrition.FatId
                });

                /*
                var exercises = new[]
                {
                    new Exercise
                    {
                        UserId = user.Id,
                        Name = "Kyykky",
                        PercentageBW = 70m
                    },
                    new Exercise
                    {
                        UserId = user.Id,
                        Name="Penkkipunnerrus",
                        PercentageBW = 0m
                    },
                    new Exercise
                    {
                        UserId = user.Id,
                        Name="Leuanveto",
                        PercentageBW = 95m
                    },
                    new Exercise
                    {
                        UserId = user.Id,
                        Name="Pystypunnerrus",
                        PercentageBW = 0
                    },
                    new Exercise
                    {
                        UserId = user.Id,
                        Name="Hauiskääntö mutkatangolla",
                        PercentageBW = 0
                    },
                    new Exercise
                    {
                        UserId = user.Id,
                        Name="Vatsalihasliike",
                        PercentageBW = 0
                    },
                };

                var goldenSix = new RoutineDetails
                {
                    UserId = user.Id,
                    Name = "Kultainen kuusikko",
                    Active = true,
                    Workouts = new[]
                    {
                        new RoutineWorkout
                        {
                            Name = "Treeni",
                            Frequency = 3m,
                            Exercises = new[]
                            {
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[0].Id,
                                    Sets = 4,
                                    Reps = 10
                                },
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[1].Id,
                                    Sets = 3,
                                    Reps = 10
                                },
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[2].Id,
                                    Sets = 3,
                                    Reps = 10
                                },
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[3].Id,
                                    Sets = 4,
                                    Reps = 10
                                },
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[4].Id,
                                    Sets = 3,
                                    Reps = 10
                                },
                                new RoutineExercise
                                {
                                    ExerciseId = exercises[5].Id,
                                    Sets = 3,
                                    Reps = 10
                                }
                            }
                        }
                    }
                };
                */
            }
            catch { }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToLocal("/");
        }


        [HttpGet("external-login")]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null, string client = null, bool add = false, string token = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Users", new
            {
                ReturnUrl = returnUrl,
                Client = client ?? Api.ApiClient.Web,
                Token = token
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("external-login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string client, string returnUrl = null, string remoteError = null, string token = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if(user == null)
            {
                // register / login
                if(string.IsNullOrWhiteSpace(token))
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    user = new User { Email = email, UserName = info.LoginProvider + "_" + info.ProviderKey };
                    var creationResult = await _userManager.CreateAsync(user);
                    if (!creationResult.Succeeded)
                    {
                    }
                    InitProfile(user);
                }
                // add login
                else
                {
                    var userId = _profileRepository.GetUserIdByRefreshToken(token);
                    if (userId.HasValue)
                    {
                        user = await _userManager.FindByIdAsync(userId.ToString());
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                }
            }
            else if (!string.IsNullOrWhiteSpace(token))
            {
                // adding login, but it's already linked to another user
                // TODO: error or merge?
                return BadRequest();
            }

            var refreshToken = _profileRepository.GetRefreshToken(user.Id);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _profileRepository.UpdateRefreshToken(user.Id);
            }
            var jwtToken = GetJwtSecurityToken(user.Id);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect($"{returnUrl.TrimEnd('/')}/{refreshToken}/{accessToken}");
            }

            return RedirectToLocal($"/#/login-success/{client}/{refreshToken}/{accessToken}");
        }
        [HttpGet("refresh-token")]
        [AllowAnonymous]
        public IActionResult RefreshAccessToken(string refreshToken)
        {
            var userId = _profileRepository.GetUserIdByRefreshToken(refreshToken);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            return TokenResult(userId.Value);
        }
        [HttpGet("token-login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithToken(string provider, string token)
        {
            if (provider.Equals("google", StringComparison.CurrentCultureIgnoreCase))
            {
                return await LoginWithGoogleToken(token);
            }
            return NotFound();
        }
        private async Task<IActionResult> LoginWithGoogleToken(string idToken)
        {
            string userId;
            string email;
            using (var client = new HttpClient())
            {
                var url = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + idToken;
                var response = client.GetStringAsync(url).Result;
                var data = JObject.Parse(response);
                userId = data.GetValue("sub").ToString();
                email = data.GetValue("email").ToString();
            }
            var user = await _userManager.FindByLoginAsync("Google", userId);
            if (user == null)
            {
                
                user = new User { Email = email, UserName = "Google_" + userId };
                var creationResult = await _userManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                {
                }
                InitProfile(user);
                
              

                var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", userId, null));
                if (!addLoginResult.Succeeded)
                {
                }
            }

            var refreshToken = _profileRepository.GetRefreshToken(user.Id);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _profileRepository.UpdateRefreshToken(user.Id);
            }
            var jwtToken = GetJwtSecurityToken(user.Id);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return TokenResult(user.Id);
        }

        private IActionResult TokenResult(Guid userId)
        {
            var refreshToken = _profileRepository.GetRefreshToken(userId);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _profileRepository.UpdateRefreshToken(userId);
            }
            var jwtToken = GetJwtSecurityToken(userId);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);


            return Ok(new TokenResponse
            {
                RefreshToken = refreshToken,
                Expires = jwtToken.ValidTo,
                AccessToken = accessToken
            });
        }

        private JwtSecurityToken GetJwtSecurityToken(Guid userId)
        {
            var claims = new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            return new JwtSecurityToken(
                issuer: _configuration.GetSection("Authentication:Jwt:SiteUrl").Value,
                audience: _configuration.GetSection("Authentication:Jwt:SiteUrl").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Jwt:Key").Value)), SecurityAlgorithms.HmacSha256)
            );
        }
   
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
