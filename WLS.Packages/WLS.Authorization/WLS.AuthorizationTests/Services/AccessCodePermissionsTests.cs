using Xunit;
using WLS.Authorization.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using WLS.Authorization.Models;
using WLS.Authorization.Constants;
using Moq;
using System.Text.Json;

namespace WLS.AuthorizationTests.Services
{
    public class AccessCodePermissionsTests
    {
        IAccessCodePermissions _accessCodePermissions;
        IRoleAccessPermissions _roleAccessPermissions;
        ILogger<AccessCodePermissions> _logger;
        IHttpContextAccessor _httpContextAccessor;
        HttpContext _context;

        public AccessCodePermissionsTests()
        {
            _roleAccessPermissions = Substitute.For<IRoleAccessPermissions>();
            _logger = Substitute.For<ILogger<AccessCodePermissions>>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            _context = new DefaultHttpContext();
            _httpContextAccessor.HttpContext.Returns(_context);

            _accessCodePermissions = new AccessCodePermissions(_httpContextAccessor, _roleAccessPermissions, _logger);
        }

        [Fact()]
        public void CanCreateTest_IsAuthroized()
        {
            var user = _context.User;
            int accountID = 1234;
            RoleType roleType = new RoleType { RoleTypeID = (int)RoleTypeEnum.LPIFacilitator, RoleName = RoleTypeEnum.LPIFacilitator.ToString(), BrandID = 0 };
            AccessType accessType = new AccessType { AccessTypeID = (int)AccessTypeEnum.Account, AccessTypeName = "Account" };
            List<RoleAccessReference> rarList = new List<RoleAccessReference>
            {
                new RoleAccessReference
                {
                    RoleType = roleType,
                    AccessType = accessType,
                    UserRoleAccessList = new List<UserRoleAccess> { new UserRoleAccess() { UserRoleID = 1, AccessTypeID = accessType.AccessTypeID, AccessRefID = 1234 } }
                }
            };

            user.AddIdentity(new ClaimsIdentity(new Claim[] { 
                new Claim(JwtClaimIdentifiers.Rol, RoleTypeEnum.LPIFacilitator.ToString()), 
                new Claim(JwtClaimIdentifiers.RoleAccessRefIDs, JsonSerializer.Serialize(rarList)) }));


            _roleAccessPermissions.GetRoleAccessReferences().Returns(rarList);
            var result = _accessCodePermissions.CanCreate(accountID).Result;

            Assert.True(result);
        }

        [Fact()]
        public void CanCreateTest_NotAuthroizedForRole()
        {
            var user = _context.User;
            int accountID = 1234;
            RoleType roleType = new RoleType { RoleTypeID = (int)RoleTypeEnum.LPIFacilitator, RoleName = RoleTypeEnum.LPILeader.ToString(), BrandID = 0 };
            AccessType accessType = new AccessType { AccessTypeID = (int)AccessTypeEnum.Account, AccessTypeName = "Account" };
            List<RoleAccessReference> rarList = new List<RoleAccessReference>
            {
                new RoleAccessReference
                {
                    RoleType = roleType,
                    AccessType = accessType,
                    UserRoleAccessList = new List<UserRoleAccess> { new UserRoleAccess() { UserRoleID = 1, AccessTypeID = accessType.AccessTypeID, AccessRefID = 1234 } }
                }
            };

            user.AddIdentity(new ClaimsIdentity(new Claim[] {
                new Claim(JwtClaimIdentifiers.Rol, RoleTypeEnum.LPILeader.ToString()),
                new Claim(JwtClaimIdentifiers.RoleAccessRefIDs, JsonSerializer.Serialize(rarList)) }));


            _roleAccessPermissions.GetRoleAccessReferences().Returns(rarList);
            var result = _accessCodePermissions.CanCreate(accountID).Result;

            Assert.False(result);
        }

        [Fact()]
        public void CanCreateTest_NotAuthroizedForAccount()
        {
            var user = _context.User;
            int accountID = 1234;
            RoleType roleType = new RoleType { RoleTypeID = (int)RoleTypeEnum.LPIFacilitator, RoleName = RoleTypeEnum.LPILeader.ToString(), BrandID = 0 };
            AccessType accessType = new AccessType { AccessTypeID = (int)AccessTypeEnum.Account, AccessTypeName = "Account" };
            List<RoleAccessReference> rarList = new List<RoleAccessReference>
            {
                new RoleAccessReference
                {
                    RoleType = roleType,
                    AccessType = accessType,
                    UserRoleAccessList = new List<UserRoleAccess> { new UserRoleAccess() { UserRoleID = 1, AccessTypeID = accessType.AccessTypeID, AccessRefID = 9876 } }
                }
            };

            user.AddIdentity(new ClaimsIdentity(new Claim[] {
                new Claim(JwtClaimIdentifiers.Rol, RoleTypeEnum.LPILeader.ToString()),
                new Claim(JwtClaimIdentifiers.RoleAccessRefIDs, JsonSerializer.Serialize(rarList)) }));


            _roleAccessPermissions.GetRoleAccessReferences().Returns(rarList);
            var result = _accessCodePermissions.CanCreate(accountID).Result;

            Assert.False(result);
        }
    }
}