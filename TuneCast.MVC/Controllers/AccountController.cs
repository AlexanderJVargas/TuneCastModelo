using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            // Configura el endpoint de la API para usuarios
            Crud<Usuario>.EndPoint = "https://localhost:7194/api/Usuarios"; // Reemplaza con tu URL real
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string contraseña, bool rememberMe = false, string returnUrl = null)
        {
            try
            {
                var usuarios = Crud<Usuario>.GetAll();
                var usuario = usuarios.FirstOrDefault(u => u.Email == email && u.Contraseña == contraseña);

                if (usuario != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = rememberMe
                        });

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    // Redirige según el rol del usuario
                    if (usuario.Rol == "Administrador")
                    {
                        return RedirectToAction("Dashboard", "Planes");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
            }

                ViewData["ErrorMessage"] = "Email o contraseña incorrectos.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error al iniciar sesión: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string contraseña)
        {
            try
            {
                var usuarios = Crud<Usuario>.GetAll();
                if (usuarios.Any(u => u.Email == email))
                {
                    ViewData["ErrorMessage"] = "El correo electrónico ya está registrado.";
                    return View();
                }

                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Contraseña = contraseña,
                    Rol = "Usuario" // Rol por defecto
                };

                var usuarioCreado = await Crud<Usuario>.Create(nuevoUsuario);

                if (usuarioCreado?.Id > 0)
                {
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor inicie sesión.";
                    return RedirectToAction("Login");
                }

                ViewData["ErrorMessage"] = "No se pudo crear el usuario. Intente nuevamente.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error al registrar: {ex.Message}";
                return View();
            }
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}