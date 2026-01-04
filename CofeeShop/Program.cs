using CoffeeShop.Core.Services.Interfaces;
using CoffeeShop.Core.Services;
using CoffeeShop.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region DataBase Context

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseMySql(
        builder.Configuration.GetConnectionString("ResidenceConnection"),
        new MySqlServerVersion(new Version(8, 0, 41))
    );
});

#endregion DataBase Context

#region IoC

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IResidenceService, ResidenceService>();

#endregion IoC

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();