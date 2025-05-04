using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discount.grpc.Data;

public static class Extensions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    { 
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        dbContext.Database.MigrateAsync();
        return app;
    }
}
