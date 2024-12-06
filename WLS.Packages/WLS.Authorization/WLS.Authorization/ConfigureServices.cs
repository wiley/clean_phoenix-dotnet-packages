using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Extensions.Http;

using System;
using System.Collections.Generic;
using System.Net.Http;

using WLS.Authorization.Constants;
using WLS.Authorization.Models;
using WLS.Authorization.Services;

namespace WLS.Authorization
{
	public static class ConfigureServices
	{
		public static void AddWLSAuthorization(this IServiceCollection services, string baseUrl, string userApiToken)
		{
			services.AddSingleton(new Token(userApiToken));
			services.AddHttpContextAccessor();

			ConfigureHttpClient(services, baseUrl);

			var facilitatorRoles = new List<string> { RoleTypeEnum.LPIFacilitator.ToString(), RoleTypeEnum.CatalystFacilitator.ToString() };

			services.AddAuthorizationCore(options =>
			{
				options.AddPolicy("Facilitator", builder =>
					builder.AddAuthenticationSchemes(JwtSchemes.Default).RequireAssertion(context =>
					   context.User.HasClaim(claim =>
						  claim.Type == JwtClaimIdentifiers.Rol && facilitatorRoles.Contains(claim.Value.Replace(" ", "")))));
			});

			services.AddScoped<IAccessCodePermissions, AccessCodePermissions>();
		}

		private static void ConfigureHttpClient(IServiceCollection services, string usersApiBaseUrl)
		{
			services.AddHttpClient<IRoleAccessPermissions, RoleAccessPermissions>(client =>
			{
				client.BaseAddress = new Uri(usersApiBaseUrl);
				client.Timeout = TimeSpan.FromSeconds(30);
			})
				.AddPolicyHandler(GetRetryPolicy())
				.AddPolicyHandler(GetCircuitBreakerPolicy());
		}

		private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
				.WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
		}

		private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
		}
	}
}
