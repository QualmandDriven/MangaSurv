using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MangaSurvWebApi.Service
{
    //    {
    //  "name": "xxx@outlook.com",
    //  "email": "xxx@outlook.com",
    //  "email_verified": true,
    //  "roles": [
    //    "admin",
    //    "user"
    //  ],
    //  "iss": "https://qualmanddriven.eu.auth0.com/",
    //  "sub": "auth0|5817589d344073a3012a2e08",
    //  "aud": "HlyqIQq8LEDJxkRYu8lD4gXYWtuw8Tok",
    //  "exp": 1486318209,
    //  "iat": 1486282209
    //}

    public class UserTokenDetails
    {
        public string name { get; set; }
        public string email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string iss { get; set; }
        public string id { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public string exp { get; set; }
        public string iat { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsVerified { get; set; }

        public UserTokenDetails(ClaimsPrincipal claimsIdentity)
        {
            this.id = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            this.name = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            this.email = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().ForEach(role =>
            {
                this.Roles.Add(role.Value);
            });
            this.iss = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
            this.sub = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            this.aud = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
            this.exp = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            this.iat = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;
            bool bVerified = false;
            if(bool.TryParse(claimsIdentity.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value, out bVerified))
            {
                this.IsVerified = bVerified;
            }
            this.IsAdmin = this.Roles.Contains("admin");
        }
    }
}
