using Xunit;
using WLS.Authorization.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using WLS.Authorization.Models;
using WLS.Authorization.Constants;
using System.Text.Json;
using System.Security.Claims;
using Moq;
using Newtonsoft.Json;
using FluentAssertions;

namespace WLS.AuthorizationTests.Services
{
    public class RoleAccessPermissionsTests
    {
        private readonly ILogger<RoleAccessPermissions> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _context;
        private readonly Token _token;

        public RoleAccessPermissionsTests()
        {
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _logger = Substitute.For<ILogger<RoleAccessPermissions>>();
            _context = Substitute.For<HttpContext>();
            _httpContextAccessor.HttpContext.Returns(_context);
            _token = new Token("userToken");
        }

        [Fact()]
        public void GetRoleAccessReferencesTest_Success()
        {
            RoleType roleType = new RoleType { RoleTypeID = (int)RoleTypeEnum.LPIFacilitator, RoleName = RoleTypeEnum.LPIFacilitator.ToString(), BrandID = 0 };
            AccessType accessType = new AccessType { AccessTypeID = (int)AccessTypeEnum.Account, AccessTypeName = "Account" };
            List<RoleAccessReference> expectedList = new List<RoleAccessReference>
            {
                new RoleAccessReference
                {
                    RoleType = roleType,
                    AccessType = accessType,
                    UserRoleAccessList = new List<UserRoleAccess> { new UserRoleAccess() { UserRoleID = 1, AccessTypeID = accessType.AccessTypeID, AccessRefID = 1234 } }
                }
            };

            var httpClient = GetFakeClient(HttpStatusCode.OK, expectedList, true);
            var roleAccessPermissions = new RoleAccessPermissions(_httpContextAccessor, httpClient, _logger, _token);

            var result = roleAccessPermissions.GetRoleAccessReferences();

            result.Result.Should().BeEquivalentTo(expectedList);
        }

        [Fact()]
        public async void GetRoleAccessReferencesTest_FailureThrowsException()
        {
            RoleType roleType = new RoleType { RoleTypeID = (int)RoleTypeEnum.LPIFacilitator, RoleName = RoleTypeEnum.LPIFacilitator.ToString(), BrandID = 0 };
            AccessType accessType = new AccessType { AccessTypeID = (int)AccessTypeEnum.Account, AccessTypeName = "Account" };
            List<RoleAccessReference> expectedList = new List<RoleAccessReference>
            {
                new RoleAccessReference
                {
                    RoleType = roleType,
                    AccessType = accessType,
                    UserRoleAccessList = new List<UserRoleAccess> { new UserRoleAccess() { UserRoleID = 1, AccessTypeID = accessType.AccessTypeID, AccessRefID = 1234 } }
                }
            };

            var httpClient = GetFakeClient(HttpStatusCode.BadRequest, expectedList, true);
            var roleAccessPermissions = new RoleAccessPermissions(_httpContextAccessor, httpClient, _logger, _token);

            var result = await Assert.ThrowsAsync<Exception>(async () =>
                await roleAccessPermissions.GetRoleAccessReferences());

            Assert.Equal("GetRoleAccessReferences - Call to users-api failed", result.Message);
        }

        private HttpClient GetFakeClient(HttpStatusCode statusCode, object expected, bool isAuthenticated = true)
        {
            var claim = new Claim(JwtClaimIdentifiers.Id, "lpi:singleton:facilitator:1234");

            var identity = Substitute.For<ClaimsIdentity>();
            identity.IsAuthenticated.Returns(isAuthenticated);
            identity.FindFirst(Arg.Any<string>()).Returns(claim);

            var claimsPrincipal = Substitute.For<ClaimsPrincipal>();
            claimsPrincipal.HasClaim(Arg.Any<string>(), Arg.Any<string>()).Returns(isAuthenticated);
            claimsPrincipal.HasClaim(Arg.Any<Predicate<Claim>>()).Returns(isAuthenticated);
            claimsPrincipal.Identity.Returns(identity);

            _context.User.Returns(claimsPrincipal);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonConvert.SerializeObject(expected), Encoding.UTF8, "application/json")
            });
            var fakeClient = new HttpClient(fakeHttpMessageHandler);
            fakeClient.BaseAddress = new Uri("http://www.wiley-epic.com");

            return fakeClient;
        }
    }
}