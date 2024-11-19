using Basket.API.Extensions;
using Common.Logging;
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
    builder.Host.AddAppConfigurations();
    builder.Services.ConfigureServices();

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, "Unhandled exception: {ex.Message}", ex.Message);
}
finally
{
    Log.Information("Shutdown {builder.Environment.ApplicationName} complete", builder.Environment.ApplicationName);
    Log.CloseAndFlush();
}