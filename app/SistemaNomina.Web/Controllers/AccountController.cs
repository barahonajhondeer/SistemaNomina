using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaNomina.Web.Data;
using SistemaNomina.Web.Models;
using SistemaNomina.Web.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaNomina.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Si el usuario ya está autenticado, redirigir al inicio
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar usuario por nombre de usuario
                var user = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.usuario == model.Usuario);

                if (user != null)
                {
                    // Verificar contraseña usando BCrypt
                    if (BCrypt.Net.BCrypt.Verify(model.Clave, user.clave))

                    {
                        // Crear claims para la autenticación

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.usuario),
                            new Claim(ClaimTypes.Role, user.rol),
                            new Claim("emp_no", user.emp_no.ToString()),
                            new Claim(ClaimTypes.Email, user.Employee?.correo ?? "")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.Recordarme,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        return RedirectToAction("Index", "home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register (solo para administradores)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.usuario == model.Usuario);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Usuario", "El nombre de usuario ya está en uso.");
                    return View(model);
                }

                // Verificar que el empleado existe
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.emp_no == model.emp_no);

                if (employee == null)
                {
                    ModelState.AddModelError("emp_no", "El número de empleado no existe.");
                    return View(model);
                }

                // Verificar que el empleado no tenga ya un usuario
                var userWithEmployee = await _context.Users
                    .FirstOrDefaultAsync(u => u.emp_no == model.emp_no);

                if (userWithEmployee != null)
                {
                    ModelState.AddModelError("emp_no", "Este empleado ya tiene un usuario asignado.");
                    return View(model);
                }

                // Crear nuevo usuario con contraseña hasheada
                var user = new User
                {
                    emp_no = model.emp_no,
                    usuario = model.Usuario,
                    clave = BCrypt.Net.BCrypt.HashPassword(model.Clave),
                    rol = model.Rol
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}