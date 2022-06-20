var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("CanaryDbContextConnection");
// builder.Services.AddDbContext<CanaryDbContext>(options => options.UseNpgsql(connectionString));

// builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
// {
// options.Password.RequireNonAlphanumeric = false;
// options.SignIn.RequireConfirmedAccount = true;
// })
// .AddEntityFrameworkStores<CanaryDbContext>()
// .AddDefaultTokenProviders();

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
// builder.Services.AddEmailSender("no-reply@canaryoverflow.com", "CanaryOverflow");

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
