using api_financiamento.src.Configuration;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.AddBuilderConfiguration();
builder.AddBuilderAuthentication();
builder.AddContext();
builder.AddBuilderServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();