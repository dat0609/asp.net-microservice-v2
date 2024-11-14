using Common.Logging;
using Customer.API.Controllers;
using Customer.API.Extensions;
using Customer.API.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((Serilogger.Configure));

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    builder.Services.ConfigureCustomerContext(builder.Configuration);
    builder.Services.AddInfrastructureServices();

    var app = builder.Build();
    app.MapCustomersAPI();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.SeedCustomerData()
        .Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shutdown {builder.Environment.ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}