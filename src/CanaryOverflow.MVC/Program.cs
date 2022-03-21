using CanaryOverflow.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using CanaryOverflow.Infrastructure.Contexts;
using CanaryOverflow.MVC.EmailServices;
using Microsoft.AspNetCore.Identity;
using CanaryOverflow.MVC.Features.Auth;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CanaryDbContextConnection");
builder.Services.AddDbContext<CanaryDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<CanaryDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.LoginPath = "/Auth/Login";
});

builder.Services.AddAuthValidators();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddFluentValidation(fv =>
    {
        fv.DisableDataAnnotationsValidation = true;
    });
builder.Services.AddEmailSender("no-reply@canaryoverflow.com", "CanaryOverflow");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
