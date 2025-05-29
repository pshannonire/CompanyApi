using CompanyAPI.Application;
using CompanyAPI.Extensions;
using CompanyAPI.Infrastructure;
using CompanyAPI.Infrastructure.Data;
using CompanyAPI.Middleware;
using Serilog;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CompanyAPI.Application.DependencyInjection).Assembly);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

try
{
    Log.Information("Initializing Company API with in-memory database...");
    await DatabaseInitializer.InitializeAsync(app.Services);
    Log.Information("Database initialization completed successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Database initialization failed. Application cannot start.");
    throw;
}



app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();

public partial class Program();
