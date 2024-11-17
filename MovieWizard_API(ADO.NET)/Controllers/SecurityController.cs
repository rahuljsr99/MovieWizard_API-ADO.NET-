using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MovieWizardAPI.Models;
using System.Security.Cryptography;
using MovieWizardAPI.Service.Interfaces;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        public SecurityController(IConfiguration configuration, ISecurityService securityService)
        {
            _configuration = configuration;
            _securityService = securityService;
        }
        [HttpGet("ValidateUserAsync")]
        public async Task<ActionResult<ValidateUserResponse>> ValidateUserAsync(string email, string password)
        {
            if (email == null || password == null)
            {
                return BadRequest("username/password is null");
            }
           
            var LoginPermissionResponse = await _securityService.ValidateUserCredentials(email, password);
            if (LoginPermissionResponse != null)
            {
                return Ok(LoginPermissionResponse);
            }
            else
            {
                return Unauthorized(LoginPermissionResponse);
            }
           
        }


        [HttpPost("GetToken")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            if (login.Username == "testuser" && login.Password == "password")
            {
                var token = GenerateJwtToken(login.Username);
                var refreshToken = GenerateRefreshToken();

                // Save the refresh token to the database, associated with the user

                return Ok(new
                {
                    token,
                    refreshToken = refreshToken.Token
                });
            }
            return Unauthorized();
        }

        public string GenerateJwtToken(string username)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "User") // Example role
            };

            // Set token expiration time (adjust as needed)
            var expires = DateTime.UtcNow.AddMinutes(30);

            // Create signing credentials using a secure algorithm (e.g., HMACSHA256)
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256);


            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer:"Jwt:Issuer", // Replace with your issuer
                audience: "Jwt:Audience", // Replace with your audience
                claims: claims,

                expires: expires,
                signingCredentials: signingCredentials
            );

            var newToken = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(newToken);

            return newToken;

        }


        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7), // Set refresh token expiry
                UserId = "userId", // Assign the actual user ID
            };

            return refreshToken;
        }

        [HttpPost("ValidateToken")]
        public IActionResult ValidateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            try
            {
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;

                return Ok(new { IsValid = true, UserId = userId });
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                return Unauthorized(new { IsValid = false, Error = "Invalid issuer. Expected issuer: " + _configuration["Jwt:Issuer"] + ", but got: " + ex.Message });
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                return Unauthorized(new { IsValid = false, Error = "Invalid audience. Expected audience: " + _configuration["Jwt:Audience"] + ", but got: " + ex.Message });
            }
            catch (SecurityTokenExpiredException ex)
            {
                return Unauthorized(new { IsValid = false, Error = "Token expired. " + ex.Message });
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                return Unauthorized(new { IsValid = false, Error = "Invalid token signature. " + ex.Message });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { IsValid = false, Error = "Token validation failed. " + ex.Message });
            }
        }
        [HttpPost("ValidateTokenManually")]
        public IActionResult ValidateTokenManually([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }

            try
            {
                // Decode the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(request.Token);

                // Extract the issuer directly from the token's Issuer property, not from claims
                    var tokenIssuer = jwtToken.Issuer;
                var tokenAudience = jwtToken.Audiences.FirstOrDefault();

                if (string.IsNullOrEmpty(tokenIssuer))
                {
                    return Unauthorized(new { IsValid = false, Error = "Issuer is missing from the token." });
                }

                // Validate the issuer and audience
                var expectedIssuer = _configuration["Jwt:Issuer"];
               // var expectedAudience = _configuration["Jwt:Audience"];

                if (tokenIssuer != expectedIssuer)
                {
                    return Unauthorized(new { IsValid = false, Error = $"Invalid issuer. Expected: {expectedIssuer}, but got: {tokenIssuer}" });
                }

                //if (tokenAudience != expectedAudience)
                //{
                //    return Unauthorized(new { IsValid = false, Error = $"Invalid audience. Expected: {expectedAudience}, but got: {tokenAudience}" });
                //}

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return Unauthorized(new { IsValid = false, Error = "Token has expired." });
                }


                return Ok(new { IsValid = true, Issuer = tokenIssuer, Audience = tokenAudience });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { IsValid = false, Error = $"Token validation failed. {ex.Message}" });
            }
        }


    }


}
public class TokenRequest
    {
        public string? Token { get; set; }
    }
