using System.Text.Json.Serialization;
using Erp.Purchasing.Presentation;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

const string allowSpecificOrigins = "AllowSpecificOrigins";

var corsOrigins = builder.Configuration["CORS_ORIGINS"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddPurchasingModule(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
        policy =>
        {
            if (corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins);
            }
            else
            {
                policy.AllowAnyOrigin();
            }

            policy.AllowAnyHeader().AllowAnyMethod();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Open API V1");
    });
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(allowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
