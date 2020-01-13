using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationPlugin;
using CWheelsApi.Data;
using CWheelsApi.Models;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CWheelsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public AccountsController(IConfiguration configuration, CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromBody]User user)
        {
            var userWithSameEmail = _cWheelsDbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if (userWithSameEmail != null)
            {
                return BadRequest("User with same email already exists");
            }
            var userObj = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
            };

            _cWheelsDbContext.Users.Add(userObj);
            _cWheelsDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }

        [HttpPost]
        public IActionResult Login([FromBody]User user)
        {
            var userEmail = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userEmail == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                return Unauthorized();
            }

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(ClaimTypes.Email, user.Email),
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id = userEmail.Id
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel changePasswordModel)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }

            if (!SecurePasswordHasherHelper.Verify(changePasswordModel.OldPassword, user.Password))
            {
                return Unauthorized("Sorry you can't change the password");
            }
            user.Password = SecurePasswordHasherHelper.Hash(changePasswordModel.NewPassword);
            _cWheelsDbContext.SaveChanges();
            return Ok("Your password has been changed");
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditPhoneNumber([FromBody]ChangePhoneModel changePhoneModel)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }
            user.Phone = changePhoneModel.PhoneNumber;
            _cWheelsDbContext.SaveChanges();
            return Ok("Your phone number has been updated");
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditUserProfile([FromBody]byte[] ImageArray)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }
            var stream = new MemoryStream(ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                user.ImageUrl = file;
                _cWheelsDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }

        }



    }
}