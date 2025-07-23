using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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



        // Acción para mostrar el perfil del usuario
        public IActionResult Perfil()
        {
            // Obtenemos el nombre del usuario que está autenticado
            var userName = User.Identity.Name;

            // Recuperamos el usuario por su nombre (simulando el proceso aquí)
            var user = GetUserByName(userName);

            if (user == null)
            {
                return NotFound();
            }

            // Asignamos la imagen de perfil y biografía al ViewBag
            ViewBag.ProfileImage = "/images/default-avatar.jpg";  // Imagen por defecto
            ViewBag.Biography = "Biografía del artista o cliente";  // Biografía por defecto

            // Determinamos el rol del usuario
            if (user.Rol == "Artista")
            {
                // Si es un artista, obtenemos las canciones del artista
                var canciones = GetSongsByArtist(userName);
                ViewBag.Canciones = canciones;
            }
            else if (user.Rol == "Cliente")
            {
                // Si es un cliente, obtenemos el plan de suscripción
                var plan = GetSubscriptionPlan(userName); // Obtiene el plan de suscripción del cliente
                ViewBag.Plan = plan ?? "Free"; // Si no tiene plan, será "Free"
            }

            return View(user);  // Pasa el modelo de usuario para que se muestre
        }

        // Métodos para obtener el usuario, las canciones y el plan de suscripción
        private Usuario GetUserByName(string userName)
        {
            // Recuperar al usuario por su nombre (esto es solo un ejemplo)
            return new Usuario
            {
                Nombre = userName,
                Rol = "Usuario", // O "Cliente"
            };
        }




        [HttpPost]
        public IActionResult EditProfileImage(IFormFile profileImage)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                // Define un nombre único para la imagen, por ejemplo con GUID
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Guarda la imagen en el directorio
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }

                // Actualiza el modelo de usuario con la nueva imagen
                var userName = User.Identity.Name;
                var user = GetUserByName(userName);  // Este método obtiene el usuario
                string ProfileImage = "/images/" + fileName;

                // Redirigir al perfil actualizado
                return RedirectToAction("Perfil");
            }

            // Si no hay archivo, regresar al perfil con mensaje de error
            ModelState.AddModelError("", "No se pudo cargar la imagen");
            return View("Perfil", User);  // Deberías retornar la vista de perfil
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