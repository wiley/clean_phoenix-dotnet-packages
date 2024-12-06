using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using WLS.Authorization.Constants;
using WLS.Authorization.Models;

namespace WLS.Authorization.Services
{
    public class RoleAccessPermissions : IRoleAccessPermissions
    {
        private readonly HttpContext _context;
        private readonly HttpClient _client;
        private readonly ILogger<RoleAccessPermissions> _logger;
        private readonly string _userApiToken;
        public RoleAccessPermissions(IHttpContextAccessor httpContextAccessor, HttpClient client, ILogger<RoleAccessPermissions> logger, Token token)
        {
            _context = httpContextAccessor.HttpContext;
            _client = client;
            _logger = logger;
            _userApiToken = token.UserApiToken;
        }

        public async Task<List<RoleAccessReference>> GetRoleAccessReferences()
        {
            if (!_context.User.Identity.IsAuthenticated) throw new UnauthorizedAccessException();
            _logger.LogInformation("GetRoleAccessReferences - Find User");
            var uniqueID = _context.User.FindFirst(JwtClaimIdentifiers.Id)?.Value;
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_client.BaseAddress}api/v1.0/Users/RoleAccessReferences?uniqueID={uniqueID}"),
                Headers = {
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "UsersAPIToken", _userApiToken }
                }
            };

            HttpResponseMessage message = await _client.SendAsync(httpRequestMessage);

            if (message.IsSuccessStatusCode)
            {
                _logger.LogInformation("GetRoleAccessReferences - Success status code");
                var roleAccessReferences = await message.Content.ReadAsAsync<List<RoleAccessReference>>();

                return roleAccessReferences;
            }
            else
            {
                _logger.LogInformation("GetRoleAccessReferences - Call to users-api Failed");
                throw new Exception("GetRoleAccessReferences - Call to users-api failed");
            }
        }

        /// <summary>Determine if the currently authenticated user has Account Access permissions for a given Account</summary>
        /// <param name="epicAccountID">The Epic Account ID or -1 to find any Epic Account where the user has Account Access permissions</param>
        /// <returns>The Epic Account ID</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<int> HasEPICAccountAccess(int epicAccountID)
        {
            if (!_context.User.Identity.IsAuthenticated) throw new UnauthorizedAccessException();

            List<RoleAccessReference> roleAccessReferences = await GetRoleAccessReferences();
            
            int? accountRefID = roleAccessReferences.FirstOrDefault(
                p => p.UserRoleAccessList.Exists(
                    ura => (ura.AccessTypeID == (int)AccessTypeEnum.Account) && (epicAccountID == -1 || ura.AccessRefID == epicAccountID)))?.UserRoleAccessList[0].AccessRefID;

            if (accountRefID == null) throw new UnauthorizedAccessException($"Account Access Permissions Not Found for {epicAccountID}");

            return (int)accountRefID;
        }
    }
}
