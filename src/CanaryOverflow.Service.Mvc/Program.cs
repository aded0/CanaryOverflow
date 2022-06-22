using CanaryOverflow.Infrastructure.Data;
using CanaryOverflow.Infrastructure.Models;
using CanaryOverflow.Service.Mvc;
using CanaryOverflow.Service.Mvc.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CanaryDb");
builder.Services.AddDbContext<CanaryDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<CanaryDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<MultilanguageIdentityErrorDescriber>();

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.AccessDeniedPath = "/Auth/AccessDenied";
//     options.LoginPath = "/Auth/Login";
// });

// builder.Services.AddAuthValidators();
builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    opts.AddSupportedUICultures("en-US", "ru-RU");
    opts.SetDefaultCulture("en-US");
});

var mvcBuilder = builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

// .AddFluentValidation(fv =>
// {
// fv.DisableDataAnnotationsValidation = true;
// });
builder.Services.AddEmailSender("no-reply@canaryoverflow.com", "CanaryOverflow", "mx1.canaryoverflow.com");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();

app.UseRouting();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{userId?}");

app.Run();
