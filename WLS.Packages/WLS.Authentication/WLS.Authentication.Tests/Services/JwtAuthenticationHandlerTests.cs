using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Moq;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using WLS.Authentication.Constants;
using WLS.Authentication.Models;
using WLS.Authentication.Services;

using Xunit;

namespace WLS.Authentication.Tests.Services
{

    public class JwtAuthenticationHandlerTests
    {
        private readonly Mock<IOptionsMonitor<JwtSchemeOptions>> _options;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<UrlEncoder> _encoder;
        private readonly Mock<ISystemClock> _clock;
        private readonly Mock<ICookiesService> _cookiesService;
        private readonly JwtOptions _jwtOptions;
        private readonly JwtAuthenticationHandler _handler;

        public JwtAuthenticationHandlerTests()
        {
            _jwtOptions = new JwtOptions("webApi", "http://localhost", "abcdefghijklmnopqrstuvwxyz123456");
            _options = new Mock<IOptionsMonitor<JwtSchemeOptions>>();

            // This Setup is required for .NET Core 3.1 onwards.
            _options
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new JwtSchemeOptions());

            var logger = new Mock<ILogger<JwtAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();
            _cookiesService = new Mock<ICookiesService>();

            _handler = new JwtAuthenticationHandler(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object, _cookiesService.Object, _jwtOptions);

        }

        [Fact]
        public async Task HandleAuthenticateAsync_Authorized()
        {
            var utcNow = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.JwtValidIssuer,
                audience: _jwtOptions.JwtValidAudience,
                claims: new Claim[] {
                    new Claim(JwtClaimIdentifiers.Rol, "LPI Facilitator"),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.JwtValidIssuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.JwtValidAudience)
                },
                notBefore: utcNow,
                expires: utcNow.AddSeconds(1),
                signingCredentials: _jwtOptions.JwtSigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {encodedJwt}");

            await _handler.InitializeAsync(new AuthenticationScheme(JwtSchemes.Default, null, typeof(JwtAuthenticationHandler)), context);
            var result = await _handler.AuthenticateAsync();

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ExpiredToken()
        {
            var utcNow = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.JwtValidIssuer,
                audience: _jwtOptions.JwtValidAudience,
                claims: new Claim[] {
                    new Claim(JwtClaimIdentifiers.Rol, "LPI Facilitator"),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.JwtValidIssuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.JwtValidAudience)
                },
                notBefore: utcNow,
                expires: utcNow.AddSeconds(1),
                signingCredentials: _jwtOptions.JwtSigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {encodedJwt}");

            await _handler.InitializeAsync(new AuthenticationScheme(JwtSchemes.Default, null, typeof(JwtAuthenticationHandler)), context);

            Thread.Sleep(2000); //ensure the jwt token is expired.

            var result = await _handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Contains("The token is expired.", result.Failure.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_Unauthorized()
        {
            var context = new DefaultHttpContext();

            await _handler.InitializeAsync(new AuthenticationScheme(JwtSchemes.Default, null, typeof(JwtAuthenticationHandler)), context);
            var result = await _handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Equal("Unauthorized", result.Failure.Message);
        }
    }


}