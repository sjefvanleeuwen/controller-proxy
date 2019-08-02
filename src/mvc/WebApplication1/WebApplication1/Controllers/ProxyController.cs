using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Net.WebRequestMethods;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;

namespace WebApplication1.Controllers
{
    public class ProxyController : ApiController
    {
        [AcceptVerbs(Http.Get, Http.Head, Http.MkCol, Http.Post, Http.Put)]
        public async Task<HttpResponseMessage> Proxy()
        {
            var token = GenerateToken("s00000001:1234567890");
            this.Request.Headers.Add("Authorization", "Bearer " + token);

            using (HttpClient http = new HttpClient())
            {
                this.Request.RequestUri = new Uri(ConfigurationManager.AppSettings["webapi:ProxyUrl"] + this.Request.RequestUri.PathAndQuery);
                if (this.Request.Method == HttpMethod.Get)
                {
                    this.Request.Content = null;
                }
                try
                {
                    var s = await http.SendAsync(this.Request);
                    return s;
                }
                catch (Exception ex)
                {

                }
                return null;
            }
        }

        public static string GenerateToken(string username)
        {
            IdentityModelEventSource.ShowPII = true;
            byte[] key = Convert.FromBase64String(ConfigurationManager.AppSettings["webapi:JwtSecret"]);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
