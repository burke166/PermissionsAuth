using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;

namespace PermissionsAuth.Data
{
    public class AccessToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [JsonPropertyName("access_token")]
        public string Token { get; set; }
        [JsonPropertyName("token_type")]
        public string Type { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [NotMapped]
        public DateTime Expiration
        {
            get
            {
                if (string.IsNullOrEmpty(Token))
                    return DateTime.MinValue;

                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var tokenObj = tokenHandler.ReadJwtToken(Token);
                    return tokenObj.ValidTo.ToLocalTime();
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        public AccessToken()
        {

        }

        public AccessToken(string domain, string clientId, string clientSecret)
        {
            var client = new RestClient($"https://{domain}/oauth/token");
            client.UseSystemTextJson();
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"" + clientId +
                "\",\"client_secret\":\"" + clientSecret +
                "\",\"audience\":\"https://" + domain +
                "/api/v2/\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            var response = client.Execute<Data.AccessToken>(request);

            this.Scope = response.Data.Scope;
            this.Token = response.Data.Token;
            this.ExpiresIn = response.Data.ExpiresIn;
            this.Type = response.Data.Type;
        }
    }
}

