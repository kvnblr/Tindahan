using Api.Entities.Models;
using Api.Handlers.Exceptions;
using Api.Persistence;
using Api.Services;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Microsoft.EntityFrameworkCore;
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
    builder.Services.AddAutoMapper(configuration =>
    {
        configuration.CreateMap<Product, ProductDto>();
        configuration.CreateMap<CreateProductDto, Product>();
    }, typeof(Program));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

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
