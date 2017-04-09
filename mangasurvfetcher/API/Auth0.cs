using Jose;
using mangasurvlib.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mangasurvfetcher.API
{
    public class Auth0Connector
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        public Auth0Connector()
        {

        }

        public Auth0Connector(string username, string password, string clientId, string clientSecret)
        {
            this.Username = username;
            this.Password = password;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
        }

        public string GetIdToken()
        {
            RestController ctr = RestController.GetRestController("https://qualmanddriven.eu.auth0.com");
            var postData = new
            {
                grant_type = "password",
                username = this.Username,
                password = this.Password,
                scope = "openid name email roles",
                client_id = this.ClientId,
                client_secret = this.ClientSecret
            };

            string response = ctr.Post("oauth/token", postData).Item2;
            var token = Newtonsoft.Json.Linq.JObject.Parse(response);
            string sToken = token.GetValue("id_token").ToString();
            string sTokenType = token.GetValue("token_type").ToString();
            string sAccessToken = token.GetValue("access_token").ToString();


            // Jetzt nochmal ein Token anfordern mit allen Scopes, bei oauth/token Aufruf werden die "Roles" nicht zurückgegeben
            var delegationData = new
            {
                grant_type = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                scope = "openid name email roles",
                client_id = this.ClientId,
                id_token = sToken
            };

            response = ctr.Post("delegation", delegationData).Item2;
            token = Newtonsoft.Json.Linq.JObject.Parse(response);
            sToken = token.GetValue("id_token").ToString();
            sTokenType = token.GetValue("token_type").ToString();

            return sToken;
        }
    }
}
