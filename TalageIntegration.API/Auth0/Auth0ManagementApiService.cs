using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Prime.ServicesV2.Auth0.Models;

namespace Prime.ServicesV2.Auth0
{
    public interface IAuth0ManagementApiService
    {
        Task<IList<Auth0User>> GetUsers(string username, string tenant, string application);
        Task<IList<Auth0User>> GetUsersByEmail(string email, string tenant, string application);
        Task<IList<Auth0User>> GetUsersByEmailForAllTenants(string username, string applicationName);
        Task<IList<Auth0BlockedInfoDto>> GetBlockedStatus(string username, string tenant, string application);
        Task<bool> UnblockUser(string username, string tenant, string application);
        Task<bool> UnblockIpAddress(string ipAddress, string tenant, string application);
        Task<bool> SendVerificationEmailByEmail(string email, string tenant, string application);
        Task<bool> SendVerificationEmailByUserId(string userId, string tenant, string application);
        Task<Auth0User> GetUser(string userId, string tenant, string application);
    }

    public class Auth0ManagementApiService : IAuth0ManagementApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Auth0Configuration _auth0Configuration;


        public Auth0ManagementApiService(
            IOptions<Auth0Configuration> auth0Configuration,
            IHttpClientFactory httpClientFactory)
        {
            _auth0Configuration = auth0Configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IList<Auth0User>> GetUsersByEmailForAllTenants(string email, string applicationName)
        {
            var users = new List<Auth0User>();

            foreach (var tenant in _auth0Configuration.Tenants)
                foreach (var application in tenant.Applications.Where(x => x.Name == applicationName))
                    users.AddRange(await GetUsersByEmail(email, tenant.Name, application.Name));

            return users;
        }

        public async Task<IList<Auth0User>> GetUsers(string username, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.GetAsync($"api/v2/users?q=username:{username}");

                if (!response.IsSuccessStatusCode)
                    return new List<Auth0User>();

                var users = await response.Content.ReadFromJsonAsync<List<Auth0User>>();
                users.ForEach(user => user.Tenant = tenant);

                return users;
            }
        }

        public async Task<IList<Auth0User>> GetUsersByEmail(string email, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.GetAsync($"api/v2/users-by-email?email={email}");

                if (!response.IsSuccessStatusCode)
                    return new List<Auth0User>();

                var users = await response.Content.ReadFromJsonAsync<List<Auth0User>>();
                users.ForEach(user => user.Tenant = tenant);

                return users;
            }
        }

        public async Task<IList<Auth0BlockedInfoDto>> GetBlockedStatus(string identifier, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.GetAsync($"api/v2/user-blocks?identifier={identifier}");

                if (!response.IsSuccessStatusCode)
                    return new List<Auth0BlockedInfoDto>();

                return (await response.Content.ReadFromJsonAsync<Auth0BlockedStatusDto>())?.Blocks;
            }
        }

        public async Task<bool> UnblockUser(string identifier, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.DeleteAsync($"api/v2/user-blocks?identifier={identifier}");

                // No Content = success code
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    throw new Exception("Error unblocking user in Auth0. " + await response.Content.ReadAsStringAsync());

                return true;
            }
        }

        public async Task<bool> UnblockIpAddress(string ipAddress, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.DeleteAsync($"api/v2/anomaly/blocks/ips/{ipAddress}");

                // No Content = success code
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                    throw new Exception("Error unblocking IP Address in Auth0. " + await response.Content.ReadAsStringAsync());

                return true;
            }
        }

        public async Task<bool> SendVerificationEmailByEmail(string email, string tenant, string application)
        {
            var user = (await GetUsersByEmail(email, tenant, application)).FirstOrDefault();

            if (string.IsNullOrEmpty(user?.user_id))
                throw new Exception($"SendVerificationEmailByEmail: Unable to locate an Auth0 user by email: {email}, tenant: {tenant}, application: {application}");

            return await SendVerificationEmailByUserId(user.user_id, tenant, application);
        }

        public async Task<bool> SendVerificationEmailByUserId(string userId, string tenant, string application)
        {
            var user = await GetUser(userId, tenant, application);

            if (string.IsNullOrEmpty(user?.user_id))
                throw new Exception($"Error resending email verification in Auth0 for {userId}");

            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                JObject content = new JObject { };

                var identity = user.Identities.FirstOrDefault(); // may need to expand for multiple identities

                content = new JObject
                {
                    ["user_id"] = user.user_id,
                    ["identity"] = new JObject
                    {
                        ["user_id"] = identity.user_id,
                        ["provider"] = identity.Provider
                    }
                };

                var response = await client.PostAsJsonAsync($"api/v2/jobs/verification-email", content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error resending email verification in Auth0. " + await response.Content.ReadAsStringAsync());

                return true;
            }
        }

        public async Task<Auth0User> GetUser(string userId, string tenant, string application)
        {
            using (var client = _httpClientFactory.CreateClient($"auth0management-{tenant}-{application}"))
            {
                var response = await client.GetAsync($"api/v2/users/{userId}");

                if (!response.IsSuccessStatusCode)
                    return new Auth0User();

                Auth0User user = await response.Content.ReadFromJsonAsync<Auth0User>();

                return user;
            }
        }

    }
}
