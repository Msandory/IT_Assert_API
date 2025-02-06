
//using Inventory_System_API.Data;
using Inventory_System_API.Data;
using Inventory_System_API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory_System_API.Services_Interfaces
{
    public class LoginRepository
    {
        //bring your datacontex
        private readonly DataContex _dataContex;
        private readonly IConfiguration _configuration;
        public bool ValidateUser(string username, string password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, "logico.co.sz"))
            {
                return context.ValidateCredentials(username, password);
            }
        }
        public string CreateToken(Owner owner)
        {
            List<Claim> claims = new List<Claim> {
               new Claim(ClaimTypes.Name, owner.name!),

           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
