using CoreMVCproject.DataAccessLayer;
using CoreMVCproject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
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
            // Manually validate the required fields
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            bool isValidUser = _authRepository.ValidateUser(user);
            if (isValidUser)
            {
                return RedirectToAction("Index", "Home"); // Redirect to home page after successful login
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password."); // Add error message
                return View(); // Return to the login view with error
            }
        }

    }

}
