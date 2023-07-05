using HexMaster.UrlShortner.Core.Abstractions;
using HexMaster.UrlShortner.Core.Commands;
using HexMaster.UrlShortner.Core.Configuration;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Repositories;
using HexMaster.UrlShortner.ShortLinks.Abstractions.Services;
using HexMaster.UrlShortner.ShortLinks.Services;
using HexMaster.UrlShortner.TableStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "DefaultCors";

// Add services to the container.
builder.Services.AddScoped<ICommandsHandler, UrlShortnerCommands>();
builder.Services.AddScoped<IShortLinksService, ShortLinksService>();
builder.Services.AddScoped<IShortLinksRepository, ShortLinksRepository>();
builder.Services.Configure<AzureCloudConfiguration>(
    builder.Configuration.GetSection(AzureCloudConfiguration.SectionName));
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://app.tinylnk.nl")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
 {
     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
 }).AddJwtBearer(options =>
 {
     options.Authority = "https://urlshortner.eu.auth0.com/";
     options.Audience = "https://api.tinylnk.nl";
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthorization();
app.MapControllers();

app.Run();
