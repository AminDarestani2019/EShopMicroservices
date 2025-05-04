using Discount.grpc.Data;
using Discount.grpc.Services;
using Microsoft.EntityFrameworkCore;
//using System.Security.Cryptography.X509Certificates;

//var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
//var certificatePath = Path.Combine(appData, "ASP.NET", "https", "mycertificate.pfx");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ConfigureHttpsDefaults(httpsOptions =>
//    {
//        httpsOptions.ServerCertificate = new X509Certificate2(certificatePath, "mypassword");
//    });
//});

//adding https
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var config = builder.Configuration;

    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(
            config["Kestrel:Certificates:Default:Path"],
            config["Kestrel:Certificates:Default:Password"]);
    });
});

builder.Services.AddDbContext<DiscountContext>(opts =>
opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMigration();
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
