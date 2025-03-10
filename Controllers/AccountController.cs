using CoreMVCproject.DataAccessLayer;
using CoreMVCproject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CoreMVCproject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthRepository _authRepository;

        public AccountController(AuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // GET: /Account/Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Role()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Role(Role role)
        {
            if (ModelState.IsValid)
            {
                bool isRoleAdded = _authRepository.AddRole(role);
                if (isRoleAdded)
                {
                    return RedirectToAction("User");
                }
                else
                {
                    ModelState.AddModelError("", "Role already exists.");
                    return View(role);
                }
            }
            return View(role);
        }

        [HttpGet]
        public IActionResult User()
        {
            // Fetch all roles from the database
            var roles = _authRepository.GetAllRoles();

            // Pass roles to the view using ViewBag
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult User(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save the user to the Users table
                    string errorMessage;
                    int userId = _authRepository.AddUser(user, out errorMessage);

                    if (userId == -1)
                    {
                        // Username already exists
                        ModelState.AddModelError("Username", errorMessage);
                    }
                    else if (userId == -2)
                    {
                        // Email already exists
                        ModelState.AddModelError("Email", errorMessage);
                    }
                    else if (userId > 0)
                    {
                        // Save the selected role to the UserRoles table
                        bool isRoleAssigned = _authRepository.AssignRoleToUser(userId, user.RoleId);

                        if (isRoleAssigned)
                        {
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to assign role.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User registration failed.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (e.g., using a logging framework)
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                }
            }

            // Reload roles if the model state is invalid
            ViewBag.Roles = new SelectList(_authRepository.GetAllRoles(), "Id", "Name");
            return View(user);
        }

      
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the user exists in the database
                var dbUser = _authRepository.GetUserByUsernameAndPassword(user.Username, user.PasswordHash);

                if (dbUser != null)
                {
                    // Redirect to the home page or dashboard
                    return RedirectToAction("Index", "Home");
                }
            }

            // If the model state is invalid or login fails, return to the login view
            return View(user);
        }
    }
}
//// User exists
//// Create a new ClaimsIdentity
//var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, dbUser.Username),
//                new Claim(ClaimTypes.Email, dbUser.Email),
//                new Claim(ClaimTypes.Role, dbUser.Roles[0].Name) // Assuming the user has at least one role
//            };

//var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

//// Create a new ClaimsPrincipal
//var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//// Sign in the user
//await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

//// Redirect to the home page or dashboard
//return RedirectToAction("Index", "Home");
//                }
//                else
//{
//    // User does not exist
//    ModelState.AddModelError("", "Invalid username or password.");
//}