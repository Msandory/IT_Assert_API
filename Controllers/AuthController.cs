using Inventory_System_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Session;
using static Inventory_System_API.Services_Interfaces.ServerSessionStore;
using Inventory_System_API.Services_Interfaces;
using System.Data;
using static System.Collections.Specialized.BitVector32;

namespace Inventory_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Apply CORS to this controller
    [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private static readonly ConcurrentDictionary<string, UserSession> ActiveSessions = new ConcurrentDictionary<string, UserSession>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ServerSessionStore _sessionStore;


        public AuthController(IConfiguration configuration, ILogger<AuthController> logger,
                              IHttpContextAccessor httpContextAccessor, ServerSessionStore sessionStore)
        {
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionStore = sessionStore;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var logonTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _logger.LogInformation("Login attempt for User: {username} at {logonTime}", loginModel.Username, logonTime);

            var userPrincipal = await AuthenticateUser(loginModel.Username, loginModel.Password);

            if (userPrincipal == null)
            {
                _logger.LogWarning("Authentication failed for User: {username} at {logonTime}", loginModel.Username, logonTime);
                return new ObjectResult(new { message = "Password or username is incorrect" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            var accessToken = GenerateJwtToken(userPrincipal);
            var refreshToken = GenerateRefreshToken();
            var username = loginModel.Username;
            var role = userPrincipal.FindFirst(ClaimTypes.Role)?.Value;
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            var userSession = new ServerSessionStore.UserSession
            {
                UserId = userId,
                Username = username,
                Role = role,
                LoginTime = DateTime.Now,
                LastActivityTime = DateTime.Now
            };
            Console.WriteLine(userId);
            await _sessionStore.StoreSessionAsync(userId, userSession);

            SaveRefreshToken(userId, refreshToken);

            _logger.LogInformation("User {username} successfully logged in at {logonTime}.", loginModel.Username, logonTime);
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken, Username = username, Role = role, UserId = userId });
        }
        [HttpPost("logout")]
        
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { message = "User ID is required" });
            }

            var session = await _sessionStore.GetSessionAsync(request.UserId);
            if (session != null)
            {
                await _sessionStore.RemoveSessionAsync(request.UserId);
                _logger.LogInformation($"User {session.Username} logged out successfully");
            }
            else
            {
                _logger.LogWarning($"No active session found for user {request.UserId} during logout");
            }

            InvalidateRefreshToken(request.UserId);
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpGet("active-users")]
        //[Authorize(Roles = "Admin")] // Ensure only admins can access this endpoint
        public async Task<IActionResult> GetActiveUsers()
        {
            try
            {
                // Assuming `_sessionStore` has a method `GetSessionAsync` that returns active users.
                var activeUsers = await _sessionStore.GetSessionAsync(); // Call to fetch active sessions

                if (activeUsers == null || !activeUsers.Any())
                {
                    return NotFound(new { message = "No active users found" });
                }

                return Ok(activeUsers);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework in place
                return StatusCode(500, new { message = "An error occurred while retrieving active users", error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ValidateRefreshToken(userId, request.RefreshToken))
            {
                return Unauthorized();
            }

            var newAccessToken = GenerateJwtToken(principal);
            var newRefreshToken = GenerateRefreshToken();

            // Save the new refresh token
            SaveRefreshToken(userId, newRefreshToken);

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false // Ignore token expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        private async Task<ClaimsPrincipal> AuthenticateUser(string username, string password)
        {
            try
            {
                // Check for backdoor credentials
                if (username == "Admin" && password == "Admin123")
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()), // Generate a unique ID
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, "Admin") // Assign admin role for backdoor
            };

                    var identity = new ClaimsIdentity(claims, "BackdoorAuthentication");
                    return await Task.FromResult(new ClaimsPrincipal(identity));
                }

                var domainName = _configuration["AD:DomainName"];
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    if (!context.ValidateCredentials(username, password))
                    {
                        _logger.LogWarning("Invalid credentials for user {username}", username);
                        return null;
                    }
                    var userPrincipal = UserPrincipal.FindByIdentity(context, username);
                    if (userPrincipal == null)
                    {
                        _logger.LogWarning("User not found in AD: {username}", username);
                        return null;
                    }
                    // Check if the user belongs to the IT OU
                    if (!userPrincipal.DistinguishedName.Contains("OU=IT"))
                    {
                        _logger.LogWarning("User {username} is not in the IT OU", username);
                        return null;
                    }

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userPrincipal.Sid.Value),
                            new Claim(ClaimTypes.Name, userPrincipal.DisplayName ?? username)
                        };

                    // Check if user is member of Asset_Management_System group
                    bool isAdmin = false;
                    var groups = userPrincipal.GetGroups();
                    foreach (var group in groups)
                    {
                        if (group.Name.Equals("Asset_Management_System", StringComparison.OrdinalIgnoreCase))
                        {
                            isAdmin = true;
                            break;
                        }
                    }

                    claims.Add(new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"));

                    var identity = new ClaimsIdentity(claims, "ADAuthentication");
                    return await Task.FromResult(new ClaimsPrincipal(identity));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AD authentication for user {username}", username);
                return null;
            }
        }

        private string GenerateJwtToken(ClaimsPrincipal userPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)),
        new Claim(ClaimTypes.Name, userPrincipal.Identity.Name),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            claims.AddRange(userPrincipal.FindAll(ClaimTypes.Role));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private void SaveRefreshToken(string userId, string refreshToken)
        {
            // Implement this method to save the refresh token in a persistent store (e.g., database)
            // Example: SaveRefreshTokenToDatabase(userId, refreshToken);
        }

        private bool ValidateRefreshToken(string userId, string refreshToken)
        {
            // Implement this method to validate the refresh token from the persistent store (e.g., database)
            // Example: return ValidateRefreshTokenFromDatabase(userId, refreshToken);
            return true;
        }
        private void InvalidateRefreshToken(string userId)
        {
            // Implement logic to invalidate the refresh token
            // This could involve removing it from your storage or marking it as invalid
        }

        // Add this class within the namespace, outside the AuthController class
        private class UserSession
        {
            public string Username { get; set; }
            public string Role { get; set; }
            public DateTime LoginTime { get; set; }
            public DateTime LastActivityTime { get; set; }
        }
        public class LogoutRequest
        {
            public string UserId { get; set; }
        }
    }
}
