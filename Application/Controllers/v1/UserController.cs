using Domain.DTO;
using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IAcademy_User_API.Controllers.v1
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(
            ILogger<UserController> logger, 
            IUserService userService,
            IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute] string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.Get(userId, cancellationToken);

                if (!string.IsNullOrEmpty(user.Id))
                    return Ok(user);

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = await _userService.Save(userRequest, cancellationToken);

                return Created(string.Empty, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user");
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UserRequest userRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.Update(userId, userRequest, cancellationToken);

                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", userId);
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.ValidatePassword(loginRequest, cancellationToken);

                if(string.IsNullOrEmpty(user.Id))
                    return Unauthorized("Invalid credentials");

                var expirationTimeInMinutes = int.Parse(_configuration.GetValue<string>("IAcademy:JwtSettings:ExpirationTimeInMinutes"));
                var secretKey = _configuration.GetValue<string>("IAcademy:JwtSettings:SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.Id)
                    }),
                     Expires = DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
                     SigningCredentials = new SigningCredentials(
                         new SymmetricSecurityKey(key), 
                         SecurityAlgorithms.HmacSha256Signature
                     )
                 };

                 var tokenHandler = new JwtSecurityTokenHandler();
                 var token = tokenHandler.CreateToken(tokenDescriptor);

                 return Ok(new { Token = tokenHandler.WriteToken(token) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password for user with ID: {Email}", loginRequest.Email);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("{userId}/update-password")]
        public async Task<IActionResult> UpdatePassword([FromRoute] string userId, [FromBody] UserUpdatePasswordRequest updatePasswordRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.UpdatePassword(userId, updatePasswordRequest, cancellationToken);
                
                return result ? NoContent() : BadRequest("Unable to update password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user with ID: {UserId}", userId);
                return BadRequest();
            }
        }
    }
}