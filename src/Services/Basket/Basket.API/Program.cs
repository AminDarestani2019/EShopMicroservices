using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;
using Discount.grpc;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config => 
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

/* //adding https
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var config = builder.Configuration;

    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(
            config["Kestrel:Certificates:Default:Path"],
            config["Kestrel:Certificates:Default:Password"]);
    });
}); */

// فقط HTTP گوش بده (بدون https و certificate)

//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
}).ConfigurePrimaryHttpMessageHandler(()=>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = 
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

//Async Communication services
builder.Services.AddMessageBroker(builder.Configuration);

//Cross-Cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")! );

var app = builder.Build();

app.MapCarter();
// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions 
    { 
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
app.Run();
