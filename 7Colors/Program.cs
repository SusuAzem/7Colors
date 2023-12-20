using _7Colors.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;
using _7Colors.Services;
using _7Colors.Data.IRepository;
using _7Colors.Data.Repository;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
   .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("ServerConnection")));

//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.AutomaticAuthentication = false;
//});

//var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions
//{
//    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
//    ClientCertificateMode = ClientCertificateMode.AllowCertificate,
//    ServerCertificate = new X509Certificate2("./Lets_Encrypt_7sevencolors.com.pfx", "Ss12345*")
//};


builder.Services.AddSession(op =>
{
    op.IdleTimeout = TimeSpan.FromMinutes(30);
    op.Cookie.IsEssential = true;
});

builder.Services.AddLogging(builder => builder.AddDebug());
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IEmailSender, EmailSender>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IFileManager, FileManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllersWithViews(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    }).AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.Events.OnTicketReceived = ctx =>
        {
            var user = ctx.Principal!.Identities.FirstOrDefault();
            if (user!.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Email)!.Value == StringDefault.AdminEmail)
            {
                user.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                user.AddClaim(new Claim(type: "Registered", "true"));
            }
            List<AuthenticationToken> tokens = ctx.Properties!.GetTokens().ToList();

            tokens.Add(new AuthenticationToken()
            {
                Name = "TicketCreated",
                Value = DateTime.UtcNow.ToString()
            });
            ctx.Properties!.StoreTokens(tokens);
            return Task.CompletedTask;
        };
    });

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(2); // Sets the expiry to two days
});

builder.Services.AddDataProtection()
    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
    .ProtectKeysWithCertificate(
        new X509Certificate2("./Lets_Encrypt_7sevencolors.com.pfx", "Ss12345*",
           X509KeyStorageFlags.EphemeralKeySet))
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    policy.RequireClaim(ClaimTypes.Email, StringDefault.AdminEmail));

    options.AddPolicy("Teacher", policy =>
    policy.RequireClaim(ClaimTypes.Role, "Teacher"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseForwardedHeaders();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "ECommerce",
    areaName: "ECommerce",
    pattern: "ECommerce/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Course",
    areaName: "Course",
    pattern: "Course/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Panel}/{action=FrontPage}/{id?}");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();