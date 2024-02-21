using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;
using UniqChat.Data;
using UniqChat.MapService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using static UniqChat.MapService.CMS;
var builder = WebApplication.CreateBuilder(args);
//services
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<IConnectionMappingService, ConnectionMappingService>();

//Database service,configuration for  SqlServer and Route to connection string
builder.Services.AddDbContext<DatabaseContext>(DbOptions => DbOptions.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.AddAllConnectedUsers.RemoveRange(dbContext.AddAllConnectedUsers);
    dbContext.SaveChanges();
}

// Configure  HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value  30 days.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "custom",
    pattern: "custom/{action}",
    defaults: new { controller = "Routes" });
app.MapControllerRoute(
    name: "SplashScreen",
    pattern: "{controller=Home}/{action=SplashScreen}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");
app.Run();
