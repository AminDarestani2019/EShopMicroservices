using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

////adding https
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    var config = builder.Configuration;

//    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
//    {
//        httpsOptions.ServerCertificate = new X509Certificate2(
//            config["Kestrel:Certificates:Default:Path"],
//            config["Kestrel:Certificates:Default:Password"]);
//    });
//});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddRefitClient<ICatalogService>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
});

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
