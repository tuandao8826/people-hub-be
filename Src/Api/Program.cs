using Api.Common.ApiVersioning;
using Api.Common.Auths;
using Api.Common.Logging;
using Api.Common.Swagger;
using Api.Configurations;
using Api.Middlewares;
using Api.TrainingNotifications;
using Application.Modules.SystemAccounts;
using Microsoft.AspNetCore.Authorization;
using Application;
using Application.Common.Helpers;
using Application.Common.Seeders;
using Infrastructure;

try
{
	var builder = WebApplication.CreateBuilder(args);
	var configuration = builder.Configuration;

	builder.Services.AddControllers();
	builder.Services.AddAuthorization(options => options.AddPolicy(
		SystemAccountAccess.Policy,
		policy => policy.RequireAuthenticatedUser().AddRequirements(new ManageSystemAccountsRequirement())));
	builder.Services.AddScoped<IAuthorizationHandler, SystemAccountAuthorization>();
	builder.Services.AddBrowserSession(builder.Environment);
	builder.Services.AddEndpointsApiExplorer();

    #region ApiVersioning
    builder.Services.AddApiVersioningSetup();
    #endregion

    #region Main dependencies
    builder.AddLoggingSetup();
	builder.AddConfiguration();
    builder.Configuration.AddEnvironmentVariables();
    builder.Services.AddSwaggerSetup(configuration);
	#endregion

	#region Layers dependencies
	builder.Services.AddInfrastructure(configuration);
	builder.Services.AddApplication(configuration);
	builder.Services.AddTrainingNotifications(configuration);
	#endregion

	builder.Services.AddCors(options =>
	{
		options.AddPolicy(
			"AllowSpecificOrigin",
			builder =>
			{
				builder
					.WithOrigins()
					.AllowAnyHeader()
					.AllowAnyMethod();
			}
		);
	});

    var app = builder.Build();

	#region Initialization
	await app.Services.InitializeSeedDataAsync();
	#endregion

	app.OpenSwagger();
    app.UseAuthentication();
	app.UseMiddlewares();
	app.UseAuthorization();

	app.UseCors("AllowSpecificOrigin");


    app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
	ConsoleHelper.WriteLine("[Error]", $"Detail: {ex}");
	Console.Error.WriteLine($"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}] [FATAL] {ex}");
}
