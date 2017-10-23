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

        public UsersController(UserManager<User> userManager,SignInManager<User> signInManager, IProfileRepository profileRepository, ILogRepository logger, IConfigurationRoot configuration, INutritionRepository nutritionRepository) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepository = profileRepository;
            _configuration = configuration;
            _nutritionRepository = nutritionRepository;
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
            result.Logins = logins.Select(l => l.LoginProvider).ToArray();
            result.HasPassword = hasPassword;
            result.Username = user.UserName;

            return Ok(result);
        }
        [HttpPut("me")]
        public IActionResult SaveProfile([FromBody]ProfileRequest model)
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
            profile.Height = model.Height;
            profile.Rmr = model.Rmr;
            profile.Weight = model.Weight;

            _profileRepository.SaveProfile(profile);
            var result = AutoMapper.Mapper.Map<ProfileResponse>(profile);

            return Ok(result);
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

            return await TokenResult(user.Id);
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

            return await TokenResult(user.Id);
        }

        private void InitProfile(User user)
        {
            var profile = new Profile.Profile
            {
                UserId = user.Id
            };
            _profileRepository.SaveProfile(profile);

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
                Constants.EnergyDistributionId,
                Constants.EnergyId,
                Constants.ProteinId,
                Constants.CarbId,
                Constants.FatId
            });
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
            var jwtToken = await GetJwtSecurityToken(user.Id);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect($"{returnUrl.TrimEnd('/','#')}/#/{refreshToken}/{accessToken}");
            }

            return RedirectToLocal($"/#/login-success/{client}/{refreshToken}/{accessToken}");
        }
        [HttpGet("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            var userId = _profileRepository.GetUserIdByRefreshToken(refreshToken);
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            return await TokenResult(userId.Value);
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
            var jwtToken = await GetJwtSecurityToken(user.Id);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return await TokenResult(user.Id);
        }

        private async Task<IActionResult> TokenResult(Guid userId)
        {
            var refreshToken = _profileRepository.GetRefreshToken(userId);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = _profileRepository.UpdateRefreshToken(userId);
            }
            var jwtToken = await GetJwtSecurityToken(userId);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);


            return Ok(new TokenResponse
            {
                RefreshToken = refreshToken,
                Expires = jwtToken.ValidTo,
                AccessToken = accessToken
            });
        }

        private async Task<JwtSecurityToken> GetJwtSecurityToken(Guid userId)
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
