using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;  // Para usar el Crud<T>
using TuneCastModelo;  // Para usar el modelo Cancion

namespace TuneCast.MVC.Controllers
{
    public class ReproduccionController : Controller
    {
        // GET: ReproduccionController/Reproducir
        public IActionResult Reproducir(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);  // Obtener la canción por su ID
            if (cancion == null)
            {
                return NotFound();
            }

            return View(cancion);  // Pasar la canción a la vista para reproducir
        }
    }
}