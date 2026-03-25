using IPL.Gaming.Database;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services;
using IPL.Gaming.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Database Service
builder.Services.AddScoped<ICosmosService, CosmosService>();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMatchService, MatchService>();

// Register User Cache Service (Singleton for caching, Hosted Service for startup loading)
builder.Services.AddSingleton<UserCacheService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<UserCacheService>());

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
