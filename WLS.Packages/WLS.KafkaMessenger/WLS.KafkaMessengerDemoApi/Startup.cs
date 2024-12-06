using Confluent.Kafka;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NSwag;
using NSwag.AspNetCore;

using System;
using System.Collections.Generic;
using System.IO.Compression;

using WLS.KafkaMessenger.Infrastructure;
using WLS.KafkaMessenger.Infrastructure.Interface;
using WLS.KafkaMessenger.Services;
using WLS.KafkaMessenger.Services.Interfaces;

namespace KafkaMessengerDemoApi
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true;
			});

			services.AddScoped<ILearnerService, LearnerService>();
			services.AddScoped<IKafkaMessengerService, KafkaMessengerService>();
			services.AddScoped<IKafkaConfig, KafkaConfig>();

			string kafkaHost = Environment.GetEnvironmentVariable("KAFKA_HOST");
			string kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
			var senders = new List<KafkaSender>
			{
				new KafkaSender
				{
					Topic = kafkaTopic
				}
			};
			services.AddSingleton<IKafkaConfig>(kc =>
				new KafkaConfig() { Host = kafkaHost, Sender = senders }
			);
			services.AddScoped(p => new ProducerBuilder<string, string>(new ProducerConfig
			{
				BootstrapServers = kafkaHost
			}).Build());

			services.AddResponseCompression(options =>
			{
				options.EnableForHttps = true;
				options.Providers.Add<BrotliCompressionProvider>(); //Brotli will be chosen first based upon order here
				options.Providers.Add<GzipCompressionProvider>();
			});

			services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
			services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder =>
				{
					builder.AllowAnyOrigin();
					builder.AllowAnyHeader();
					builder.AllowAnyMethod();
				});
			});

			services.AddSwaggerDocument(document =>
			{
				document.DocumentName = "swagger";
				document.Title = "WLS Kafka Messenger Demo API";
				document.Description = "WLS Kafka Messenger Demo API";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment() || env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseResponseCompression();
			app.UseCors("AllowAll");
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			if (env.IsDevelopment() || env.IsStaging())
			{
				app.UseOpenApi(options =>
				{
					options.DocumentName = "swagger";
					options.Path = "/swagger/v1/swagger.json";
					options.PostProcess = (document, x) =>
					{
						document.Schemes = new[] { OpenApiSchema.Https };
						if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOMAIN")))
						{
							document.Host = Environment.GetEnvironmentVariable("DOMAIN");
						}
					};
				});

				app.UseSwaggerUi3(options =>
				{
					options.Path = "/swagger";
					options.DocumentPath = "swagger/v1/swagger.json";
					options.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/swagger/v1/swagger.json"));
				});
			}
		}
	}
}
