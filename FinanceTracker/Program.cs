using FinanceTracker.DataAccess;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.DataAccess.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{ 
    scope.ServiceProvider.GetRequiredService<FinanceTrackerDbContext>().Database.Migrate(); }
//await SeedData.RunAsync(db);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
