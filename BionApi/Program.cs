using NLog;
using BionApi.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using BionApi.Presentation.ActionFilters;
using Contracts;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();

//registering the repository manager - 59
builder.Services.ConfigureRepositoryManager();
//registering the service manager - 62
builder.Services.ConfigureServiceManager();
//registering the sql context - 63
builder.Services.ConfigureSqlContext(builder.Configuration);

//refenrence the presentation here - 66
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(BionApi.Presentation.AssemblyReference).Assembly);

//registering the identity configuration - 292
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();

//registering the jwt - 303
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddEmailConfiguration(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

//registering the rate limit - 286
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureSwagger();

var app = builder.Build();

//84-85
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if(app.Environment.IsProduction())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

//registring rate limit - 286
app.UseIpRateLimiting();
app.UseCors("CorsPolicy");

//registering the identity configuration - 292
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Bion Api v1");
   // s.SwaggerEndpoint("/swagger/v2/swagger.json", "Bion Api v2");
});

app.Run();


//P = 13-25