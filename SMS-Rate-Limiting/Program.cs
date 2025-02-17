using SMS_Rate_Limiting.DatabaseConnection;
using SMS_Rate_Limiting.Repositories;
using SMS_Rate_Limiting.Services;
using SMS_Rate_Limiting.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();   // Enables API Explorer for OpenAPI/Swagger
builder.Services.AddSwaggerGen();             // Adds Swagger generation support

// Register your services
builder.Services.AddSingleton<ISmsRateLimitService, SmsRateLimitService>()
    .AddSingleton<ISettingRepositories, SettingRepositories>()
    .AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>()
    .AddSingleton<ISmsRateLimitRepository, SmsRateLimitRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development environment
    app.UseSwagger();  // Generate the Swagger JSON
    app.UseSwaggerUI(); // Expose Swagger UI for testing and exploring API
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAllOrigins");

app.Run();