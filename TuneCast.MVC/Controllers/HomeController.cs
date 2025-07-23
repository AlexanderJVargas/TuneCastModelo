using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TuneCast.MVC.Models;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)  // Verifica si el usuario no está autenticado
            {
                return RedirectToAction("Login", "Account");  // Redirige a la página de login
            }

            // Obtener canciones
            var canciones = Crud<Cancion>.GetAll(); // Recuperar canciones de la API
            return View(canciones);

            // Obtén la lista de usuarios con el rol de "Artista"
            var artistas = GetUsuariosRol("Artista");
            return View(artistas);

        }
        private List<Usuario> GetUsuariosRol(string rol)
        {
            var data = Crud<Usuario>.GetAll();
            var artistas = data.Where(u => u.Rol == rol).ToList();

            return artistas;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}