using System.Globalization;
using DressCode.Data;
using DressCode.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IQRCodeService, QRCodeService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity configuration
builder.Services.AddDefaultIdentity<Korisnik>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure Identity cookies - briše se kad zatvoričeš browser
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Login traje 30 minuta
    options.SlidingExpiration = true; // Produžava se kad klikćeš
    options.Cookie.MaxAge = null; // SESSION COOKIE - briše se kad zatvoričeš browser
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Radi sa HTTP i HTTPS
});

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

// Session configuration - sve se briše kad zatvoričeš browser
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // 60 minuta neaktivnosti (umesto 1)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = null; // SESSION COOKIE - briše se kad zatvoričeš browser
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Radi sa HTTP i HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax; // Hosting kompatibilnost
});

// Configure localization for Bosnia and Herzegovina
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("bs-BA"), // Bosnian (Bosnia and Herzegovina)
        new CultureInfo("hr-BA"), // Croatian (Bosnia and Herzegovina)
        new CultureInfo("sr-Latn-BA") // Serbian Latin (Bosnia and Herzegovina)
    };

    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("bs-BA");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// IMPORTANT: Session must be before Authorization
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Session middleware
app.UseAuthentication(); // Authentication middleware (if not already there)
app.UseAuthorization(); // Authorization middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();