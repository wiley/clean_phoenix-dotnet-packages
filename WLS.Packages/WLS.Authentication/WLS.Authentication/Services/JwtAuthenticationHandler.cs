using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WLS.Authentication.Models;

namespace WLS.Authentication.Services
{

    public class JwtSchemeOptions : AuthenticationSchemeOptions { }

	public class JwtAuthenticationHandler : AuthenticationHandler<JwtSchemeOptions>
	{
		private readonly ICookiesService _cookiesService;
		private readonly TokenValidationParameters _tokenValidationParameters;
		private readonly JwtOptions _jwtOptions;

		public JwtAuthenticationHandler(IOptionsMonitor<JwtSchemeOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock,
										ICookiesService cookiesService, JwtOptions jwtOptions)
			: base(options, loggerFactory, encoder, clock)
		{
			_cookiesService = cookiesService;
			_jwtOptions = jwtOptions;
			_tokenValidationParameters = GetTokenValidationParameters();
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			try
			{
				//string accessTokenFingerprint = _cookiesService.GetCookie(Context.Request, "AccessTokenFingerprint");
				//ValidateAccessTokenFingerprint

				var accessToken = Context.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);

				var handler = new JwtSecurityTokenHandler();

				ClaimsPrincipal principal = handler.ValidateToken(accessToken, _tokenValidationParameters, out SecurityToken validateToken);

				var ticket = new AuthenticationTicket(principal, Scheme.Name);

				return Task.FromResult(AuthenticateResult.Success(ticket));
			}
			catch (SecurityTokenExpiredException ex)
            {
				return Task.FromResult(AuthenticateResult.Fail(ex));
			}
			catch (Exception)
			{
				return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
			}
		}

		private TokenValidationParameters GetTokenValidationParameters()
        {
			return new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _jwtOptions.JwtValidIssuer,

				ValidateAudience = true,
				ValidAudience = _jwtOptions.JwtValidAudience,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _jwtOptions.JwtSigningCredentials.Key,

				RequireExpirationTime = false,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero,
			};
		}
	}
}
