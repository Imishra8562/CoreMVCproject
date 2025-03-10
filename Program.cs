using CoreMVCproject.DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreMVCproject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a WebApplicationBuilder to configure services and the app.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                            .AddRazorRuntimeCompilation(); // Enable runtime compilation

            // Read the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register the ProductRepository in the DI container
            builder.Services.AddScoped<ProductRepository>(provider =>
                new ProductRepository(connectionString));

            // Register AuthRepository in the DI container with the connection string
            builder.Services.AddScoped<AuthRepository>(provider =>
                new AuthRepository(connectionString));

            // Add authentication services with a default scheme
            builder.Services.AddAuthentication("Cookies") // Specify a default scheme (e.g., "Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Login"; // Specify the login path
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Specify the access denied path
                });

            // Register session services
            builder.Services.AddSession();

            // Add authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // Build the application.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // Use a custom error handler in non-development environments.
                app.UseExceptionHandler("/Home/Error");
                // Enable HTTP Strict Transport Security (HSTS).
                app.UseHsts();
            }

            // Redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Serve static files (e.g., CSS, JavaScript, images).
            app.UseStaticFiles();

            // Enable routing.
            app.UseRouting();

            // Enable session middleware
            app.UseSession();

            // Enable authentication and authorization.
            app.UseAuthentication(); // Ensure this is called before UseAuthorization
            app.UseAuthorization();

            // Map the default route for controllers.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=Index}/{id?}");

            // Start the application.
            app.Run();
        }
    }
}