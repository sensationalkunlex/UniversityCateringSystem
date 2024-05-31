using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UniversityCateringSystem.AuthService;
using UniversityCateringSystem.Controllers;
using UniversityCateringSystem.Data;
using UniversityCateringSystem.Services;

namespace UniversityCateringSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("WebApiDatabase")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
            });
            builder.Services.AddScoped<IProductServices, ProductServices>();
            builder.Services.AddHttpClient<PayPalService>();
            builder.Services.AddSingleton<PayPalService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(config =>
       {
           config.Cookie.Name = "UserLoginCookie"; // Name of the cookie
           config.LoginPath = "/Auth/Login"; // Path for the redirect to the user login page
           config.AccessDeniedPath = "/Auth/AccessDenied"; // Path for access denied page
           config.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Cookie expiration time
       });

            builder.Services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                     policy => policy.RequireRole("Administrator"));
                options.AddPolicy("RequireCustomerRole",
                     policy => policy.RequireRole("Customer"));
                // Add more policies as needed
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}