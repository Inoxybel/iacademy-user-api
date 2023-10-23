using Domain.Interfaces.Services;
using Domain.Validations;
using FluentValidation;
using FluentValidation.AspNetCore;
using IAcademy_User_API.Infra.APIConfigurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json.Serialization;

namespace IAcademy_User_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Configuration["ASPNETCORE_ENVIRONMENT"] != "Development")
            {
                builder.Configuration.AddAzureAppConfiguration(options =>
                {
                    options.Connect(Environment.GetEnvironmentVariable("AppConfigConnectionString"))
                        .TrimKeyPrefix("IAcademy:")
                        .Select("IAcademy:UserManager:Mongo:*")
                        .Select("IAcademy:ExternalServices:*")
                        .Select("IAcademy:JwtSettings:*")
                        .ConfigureRefresh(refreshOptions =>
                        {
                            refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(1));
                            refreshOptions.Register("IAcademy", true);
                        });
                });
            }

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigureApp(app, builder.Configuration);

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            MongoConfiguration.RegisterConfigurations();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IActivationCodeService, ActivationCodeService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<UserRequestValidator>();

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = false;
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                });

            services
                .AddSwagger()
                .AddOptions(configuration)
                .AddRepositories()
                .AddHealthChecks()
                .AddMongoDb(
                    mongodbConnectionString: configuration["MongoDB:ConnectionString"] ?? "ConnectionString not founded", 
                    name: "health-check-mongodb"
                );

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        public static void ConfigureApp(WebApplication app, IConfiguration configuration)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");

                c.RoutePrefix = "swagger";

                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseCors();

            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();
        }
    }
}