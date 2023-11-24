using _7Colors.Data;
using _7Colors.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;
using _7Colors.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(op =>
{
    op.IdleTimeout= TimeSpan.FromMinutes(30);
    op.Cookie.IsEssential = true;
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IEmailSender, EmailSender>();
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
        options.Scope.Add("https://www.googleapis.com/auth/user.birthday.read");
        options.Events.OnTicketReceived = ctx =>
        {           
            var user = ctx.Principal!.Identities.FirstOrDefault();
            if (user!.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Email)!.Value == "soso.g.f.86@gmail.com")
            {
                user.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
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
builder.Services.AddDbContext<AppDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var email = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email == "soso.g.f.86@gmail.com")
            {
                return true;
            }
            return false;
        });
    });
    options.AddPolicy("Teacher", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var role = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (role == "Teacher")
            {
                return true;
            }
            return false;
        });
    });
});
//builder.Services.AddCors();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

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
