using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TuneCastAPIConsumer;
using TuneCastModelo;
using System.Linq;

namespace TuneCast.MVC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            // Configura el endpoint de la API para usuarios
            Crud<Usuario>.EndPoint = "https://localhost:7194/api/Usuarios"; // Reemplaza con tu URL real
            Crud<Cancion>.EndPoint = "https://localhost:7194/api/Canciones";
            Crud<Suscripcion>.EndPoint = "https://localhost:7194/api/Suscripciones";
            Crud<Plan>.EndPoint = "https://localhost:7194/api/Planes";
        
        }
        [HttpGet]
        public IActionResult RecuperarPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarPassword(string email, string palabraClave, string newPassword)
        {
            try
            {
                var usuario = Crud<Usuario>.GetAll().FirstOrDefault(u => u.Email == email);

                if (usuario == null)
                {
                    ViewData["ErrorMessage"] = "Correo no encontrado";
                    ViewData["Email"] = email;
                    ViewData["PalabraClave"] = palabraClave;
                    return View();
                }

                if (usuario.PalabraClaveRecuperacion != palabraClave)
                {
                    ViewData["ErrorMessage"] = "Palabra clave de recuperación incorrecta";
                    ViewData["Email"] = email;
                    ViewData["PalabraClave"] = palabraClave;
                    return View();
                }

                var passwordPattern = new Regex(@"^(?=.[A-Z])(?=.[\W]).{8,}$");
                if (!passwordPattern.IsMatch(newPassword))
                {
                    ViewData["ErrorMessage"] = "La nueva contraseña debe comenzar con mayúscula, contener al menos un carácter especial y tener al menos 8 caracteres.";
                    ViewData["Email"] = email;
                    ViewData["PalabraClave"] = palabraClave;
                    return View();
                }

                usuario.Contraseña = newPassword;
                bool success = Crud<Usuario>.Update(usuario.Id, usuario);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contraseña actualizada correctamente";
                    return RedirectToAction("Login");
                }

                ViewData["ErrorMessage"] = "Error al actualizar la contraseña";
                ViewData["Email"] = email;
                ViewData["PalabraClave"] = palabraClave;
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                ViewData["Email"] = email;
                ViewData["PalabraClave"] = palabraClave;
                return View();
            }
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
                    return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> Register(string nombre, string email, string contraseña, string palabraClaveRecuperacion)
        {
            try
            {
                var usuarios = Crud<Usuario>.GetAll();
                if (usuarios.Any(u => u.Email == email))
                {
                    ViewData["ErrorMessage"] = "El correo electrónico ya está registrado.";
                    return View();
                }

                // Validar contraseña: al menos una mayúscula, un carácter especial y longitud mínima de 8 caracteres
                var passwordPattern = new Regex(@"^(?=.[A-Z])(?=.[\W]).{8,}$");
                if (!passwordPattern.IsMatch(contraseña))
                {
                    ViewData["ErrorMessage"] = "La contraseña debe comenzar con una mayúscula, contener al menos un carácter especial y tener al menos 8 caracteres.";
                    return View();
                }

                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Contraseña = contraseña,
                    Rol = "Cliente", // Rol por defecto
                    PalabraClaveRecuperacion = palabraClaveRecuperacion
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public IActionResult Perfil()
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Obtener el ID del usuario autenticado
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    TempData["ErrorMessage"] = "Error al obtener información del usuario";
                    return RedirectToAction("Login", "Account");
                }

                // Obtener el usuario real de la API
                var usuario = Crud<Usuario>.GetById(userId);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado";
                    return RedirectToAction("Login", "Account");
                }

                // Obtener canciones del usuario si es artista
                List<Cancion> canciones = new List<Cancion>();
                if (usuario.Rol?.ToLower() == "artista")
                {
                    try
                    {
                        var todasLasCanciones = Crud<Cancion>.GetAll();
                        canciones = todasLasCanciones?.Where(c => c.UsuarioId == userId).ToList() ?? new List<Cancion>();
                    }
                    catch (Exception)
                    {
                        canciones = new List<Cancion>();
                    }
                }

                // Obtener plan de suscripción si es cliente
                string planNombre = "Free";
                if (usuario.Rol?.ToLower() == "cliente")
                {
                    try
                    {
                        var suscripciones = Crud<Suscripcion>.GetAll();
                        var suscripcionActiva = suscripciones?.FirstOrDefault(s => s.UsuarioId == userId && s.Activa);

                        if (suscripcionActiva != null)
                        {
                            var planes = Crud<Plan>.GetAll();
                            var plan = planes?.FirstOrDefault(p => p.Id == suscripcionActiva.PlanId);
                            planNombre = plan?.Nombre ?? "Free";
                        }
                    }
                    catch (Exception)
                    {
                        planNombre = "Free";
                    }
                }

                // Asignar datos al ViewBag
                ViewBag.ProfileImage = "/images/default-avatar.jpg";
                ViewBag.Biography = $"Perfil de {usuario.Nombre}";
                ViewBag.Canciones = canciones;
                ViewBag.Plan = planNombre;
                ViewBag.TotalCanciones = canciones.Count;
                ViewBag.TotalReproducciones = canciones.Sum(c => c.numeroReproducciones);

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el perfil: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult EditProfileImage(IFormFile profileImage)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (profileImage != null && profileImage.Length > 0)
            {
                try
                {
                    // Validar que sea una imagen
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(profileImage.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        TempData["ErrorMessage"] = "Solo se permiten archivos de imagen (JPG, PNG, GIF)";
                        return RedirectToAction("Perfil");
                    }

                    // Obtener ID del usuario
                    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // Crear directorio si no existe
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generar nombre único para la imagen
                        var fileName = $"user_{userId}{extension}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Guardar la imagen
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            profileImage.CopyTo(stream);
                        }

                        TempData["SuccessMessage"] = "Imagen de perfil actualizada correctamente";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al actualizar la imagen: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se seleccionó ninguna imagen";
            }

            return RedirectToAction("Perfil");
        }

        private List<Cancion> GetSongsByArtist(string artistName)
        {
            // Método simulado para obtener canciones de un artista
            return new List<Cancion>
        {
            new Cancion { Titulo = "Canción 1", Artista = artistName },
            new Cancion { Titulo = "Canción 2", Artista = artistName }
        };
        }

        private string GetSubscriptionPlan(string userName)
        {
            // Método simulado para obtener el plan de suscripción de un cliente
            return "Premium"; // O "Free" si no está suscrito
        }
    }
}