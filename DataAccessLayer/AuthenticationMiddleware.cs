using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    //public async Task InvokeAsync(HttpContext context, AuthRepository authRepository)
    //{
    //    try
    //    {
    //        // Retrieve username and password from the session
    //        var username = context.Session.GetString("Username");
    //        var password = context.Session.GetString("Password");

    //        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
    //        {
    //            // Authenticate the user using AuthRepository
    //            var user = authRepository.Authenticate(username, password);
    //            if (user != null)
    //            {
    //                // Create claims for the authenticated user
    //                var claims = new List<Claim>
    //                {
    //                    new Claim(ClaimTypes.Name, user.Username),
    //                    new Claim(ClaimTypes.Email, user.Email)
    //                };

    //                // Add roles to claims
    //                foreach (var role in user.Roles)
    //                {
    //                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
    //                }

    //                // Create a ClaimsIdentity and ClaimsPrincipal
    //                var identity = new ClaimsIdentity(claims, "CustomAuthentication");
    //                context.User = new ClaimsPrincipal(identity);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the exception (e.g., using a logging framework)
    //        Console.Error.WriteLine($"Authentication error: {ex.Message}");
    //    }

    //    // Call the next middleware in the pipeline
    //    await _next(context);
    //}
}