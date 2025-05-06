using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;
using System.Security.Cryptography.X509Certificates;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var certificatePath = Path.Combine(appData, "ASP.NET", "https", "mycertificate.pfx");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2(certificatePath, "mypassword");
    });
});


builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
// configure the http request pipeline
app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.Run();
