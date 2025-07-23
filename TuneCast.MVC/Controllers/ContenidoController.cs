using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using System.IO;
using System.Threading.Tasks;

namespace TuneCast.MVC.Controllers
{
    public class ContenidoController : Controller
    {

        public IActionResult Crear()
        {
            return View();
        }

        // POST: ContenidoController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cancion cancion, IFormFile archivo)
        {
            if (archivo != null && archivo.Length > 0)
            {
                // Validar si el archivo tiene una extensión válida
                var extensionesPermitidas = new[] { ".mp3", ".wav", ".ogg" };
                var extension = Path.GetExtension(archivo.FileName).ToLower();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Solo se permiten archivos MP3, WAV o OGG.");
                    return View();
                }

                // Guardar el archivo en el directorio de canciones en el servidor
                var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "canciones", archivo.FileName);
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Asignar la ruta del archivo a la canción
                cancion.RutaArchivo = "/canciones/" + archivo.FileName;

                // Llamada al API para guardar la canción
                try
                {
                    var cancionCreada = await Crud<Cancion>.Create(cancion);  // Llamada al método Create del Crud
                    if (cancionCreada != null)
                    {
                        return RedirectToAction("Index", "Canciones");  // Redirigir al índice de canciones
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error al subir la canción.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al subir la canción: {ex.Message}");
                }
            }
            else
            {
                ModelState.AddModelError("", "El archivo no es válido.");
            }
            return View(cancion);
        }
    }
}