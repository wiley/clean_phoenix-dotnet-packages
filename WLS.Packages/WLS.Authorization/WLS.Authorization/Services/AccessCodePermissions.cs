using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using WLS.Authorization.Constants;
using WLS.Authorization.Models;

namespace WLS.Authorization.Services
{
    public class AccessCodePermissions: IAccessCodePermissions
    {
        private readonly IRoleAccessPermissions _rolePermissionsService;
        private readonly ILogger<AccessCodePermissions> _logger;
        private readonly ClaimsPrincipal _user;
        public AccessCodePermissions(IHttpContextAccessor httpContextAccessor, IRoleAccessPermissions rolePermissionsService, ILogger<AccessCodePermissions> logger)
        {
            _user = httpContextAccessor.HttpContext.User;
            _logger = logger;
            _rolePermissionsService = rolePermissionsService;
        }

        public async Task<bool> CanCreate(int accountID) 
        { 
            List<string> allowedRoles = new List<string> { RoleTypeEnum.LPIFacilitator.ToString(), RoleTypeEnum.CatalystFacilitator.ToString() };
            var isFacilitator = _user.HasClaim(c => c.Type == JwtClaimIdentifiers.Rol && allowedRoles.Contains(c.Value.Replace(" ","")));
            if (isFacilitator)
            {
                var rolePermissions = await _rolePermissionsService.GetRoleAccessReferences();
                if (rolePermissions.Exists(rar => rar.UserRoleAccessList.Exists(ura => ura.AccessTypeID == (int)AccessTypeEnum.Account && ura.AccessRefID == accountID)))
                    return true;
            }
            return false;
        }
        
    }
}
