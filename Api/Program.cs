using Api.Entities.Models;
using Api.Handlers.Exceptions;
using Api.Health;
using Api.Persistence;
using Api.Services;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
            );

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ILinkService, LinkService>();
    builder.Services.AddAutoMapper(configuration =>
    {
        configuration.CreateMap<Product, ProductDto>();
        configuration.CreateMap<CreateProductDto, Product>();
        configuration.CreateMap<UpdateProductDto, Product>()
            .ForMember(destinationMember => destinationMember.Id, options => options.Ignore())
            .ReverseMap();
    }, typeof(Program));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var sqlTimeoutSeconds = builder.Configuration.GetValue("HealthChecks:SqlTimeoutSeconds", 3);
    if (sqlTimeoutSeconds <= 0)
        throw new InvalidOperationException("SQL health-check timeout must be greater than zero.");
    builder.Services
        .AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"])
        .AddSqlServer(
                connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection invalid."),
                healthQuery: "SELECT 1",
                name: "sql-server",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready", "database"],
                timeout: TimeSpan.FromSeconds(sqlTimeoutSeconds)
                );
    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApiContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occured while migrating the database...");
        }
    }

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Tindahan API v1");
        options.RoutePrefix = string.Empty;
    });

    app.UseExceptionHandler();

    // app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    app.UseCors("CorsPolicy");

    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health",
        new HealthCheckOptions
        {
            ResponseWriter = HealthResponseWriter.WriteResponse,
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        }
    );

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = HealthResponseWriter.WriteResponse,
        AllowCachingResponses = false,
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthResponseWriter.WriteResponse,
        AllowCachingResponses = false
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
