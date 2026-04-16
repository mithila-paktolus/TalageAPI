using Newtonsoft.Json;
using Prime.ServicesV2.Auth0.Models;
using System;
using TalageIntegration.Domain.Entities;
using Talage.SDK.Internal.Persistence;

namespace Prime.ServicesV2.Auth0
{
    public interface IAuth0JwtService
    {
        JwtDto GetJwt(Auth0Tenant tenant, Auth0Application application);
    }

	// this will silently get an Auth0 M-M jwt, also also do the care and feeding of the jwt
	// Is it used by the named http client that is created in the startup, to allow us 
	// to hit externalv5.
	// to create the v5 client the syntax is: var client = _httpClientFactory.CreateClient("InternalToExternalClient");
	// and after that everything is the same as a normal httpClient.
	// this service is not used directly, only by that named client.
	public class Auth0JwtService : IAuth0JwtService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TalageIntegrationDbContext _context;

        public Auth0JwtService(
            IHttpClientFactory httpClientFactory,
            TalageIntegrationDbContext context)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public JwtDto GetJwt(Auth0Tenant tenant, Auth0Application application)
        {
            var jwt = CheckJwtCache(tenant, application);

            if (jwt != null)
                return jwt;

            jwt = GetNewJwt(tenant, application).Result;

            SaveOrUpdateJwt(jwt, application.Name);

            return jwt;
        }

        private async Task<JwtDto> GetNewJwt(Auth0Tenant tenant, Auth0Application application)
        {
            var token = string.Empty;
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("audience", application.Audience),
                new KeyValuePair<string, string>("client_id", application.ClientId),
                new KeyValuePair<string, string>("client_secret", application.ClientSecret)
            });

            var client = _httpClientFactory.CreateClient($"auth0jwt-{tenant.Name}-{application.Name}");

            using (var response = await client.PostAsync("oauth/token", requestContent))
            {
                token = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonConvert.DeserializeObject<Auth0ErrorDto>(token);
                    throw new Exception($"Auth0 Error. {errorResponse.ErrorType}:{errorResponse.ErrorDescription}");
                }
            }

            var myToken = JsonConvert.DeserializeObject<Auth0JwtDto>(token);

            if (myToken == null)
                return null;

            return new JwtDto
            {
                Token = myToken.access_token,
                Tenant = tenant.Name,
                Audience = application.Audience,
                ExpirationDate = DateTime.Now.AddSeconds(Convert.ToDouble(myToken.expires_in))
            };
        }

        private JwtDto CheckJwtCache(Auth0Tenant tenant, Auth0Application application)
        {
            var jwt = GetStoredJwt(tenant, application);
            if (jwt == null || IsExpired(jwt)) return null;
            return jwt;
        }

        private bool IsExpired(JwtDto jwt)
        {
            if (jwt == null) return true;

            // are we within 10 minutes of expiring?
            var expDate = jwt.ExpirationDate.AddSeconds(-600);

            return DateTime.Compare(DateTime.Now, expDate) > 0;
        }

        private JwtDto GetStoredJwt(Auth0Tenant tenant, Auth0Application application)
        {
            var jwt = _context.Jwt
                .Where(x => x.SourceApplication == application.Name)
                .Where(x => x.Audience == application.Audience)
                .Where(x => x.Tenant == tenant.Name)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (jwt == null) return null;

            return new JwtDto
            {
                Token = jwt.Token,
                Tenant = jwt.Tenant,
                Audience = jwt.Audience,
                ExpirationDate = jwt.ExpirationDate
            };
        }

        public void SaveOrUpdateJwt(JwtDto jwt, string applicationName)
        {
            var record = _context.Jwt
                .Where(x => x.SourceApplication == applicationName)
                .Where(x => x.Audience == jwt.Audience)
                .Where(x => x.Tenant == jwt.Tenant)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();

            if (record == null)
            {
                var newRecord = new Jwt
                {
                    SourceApplication = applicationName,
                    Token = jwt.Token,
                    Audience = jwt.Audience,
                    Tenant = jwt.Tenant,
                    CreatedDate = DateTime.Now,
                    ExpirationDate = jwt.ExpirationDate
                };

                _context.Jwt.Add(newRecord);
            }
            else
            {
                record.Token = jwt.Token;
                record.Audience = jwt.Audience;
                record.Tenant = jwt.Tenant;
                record.CreatedDate = DateTime.Now;
                record.ExpirationDate = jwt.ExpirationDate;
            }
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}

