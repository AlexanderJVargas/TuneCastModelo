using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TuneCastAPIConsumer;
using TuneCastModelo;

namespace TuneCast.MVC.Controllers
{
    public class PlanesClienteController : Controller
    {
        public PlanesClienteController()
        {

            Crud<Plan>.EndPoint = "https://localhost:7194/api/Planes";
            Crud<Suscripcion>.EndPoint = "https://localhost:7194/api/Suscripciones";
            Crud<Usuario>.EndPoint = "https://localhost:7194/api/Usuarios";
        }

        // GET: PlanesClienteController
        public ActionResult Index()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                // Obtener todas las suscripciones para encontrar la del usuario
                var suscripciones = Crud<Suscripcion>.GetAll();
                var suscripcionActual = suscripciones.FirstOrDefault(s => s.UsuarioId == usuarioId && s.Activa);

                // Obtener todos los planes disponibles
                var planesDisponibles = Crud<Plan>.GetAll();

                ViewBag.SuscripcionActual = suscripcionActual;
                ViewBag.PlanActual = suscripcionActual != null ?
                    planesDisponibles.FirstOrDefault(p => p.Id == suscripcionActual.PlanId) : null;
                ViewBag.UsuarioId = usuarioId;

                return View(planesDisponibles);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al cargar los datos: {ex.Message}";
                return View(new List<Plan>());
            }
        }

        [HttpPost]
        public async Task<ActionResult> CambiarPlan(int planId)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                // Obtener suscripción actual
                var suscripciones = Crud<Suscripcion>.GetAll();
                var suscripcionActual = suscripciones.FirstOrDefault(s => s.UsuarioId == usuarioId && s.Activa);

                if (suscripcionActual != null)
                {
                    // Desactivar suscripción actual
                    suscripcionActual.Activa = false;
                    suscripcionActual.FechaFin = DateTime.UtcNow;
                    Crud<Suscripcion>.Update(suscripcionActual.Id, suscripcionActual);
                }

                // Crear nueva suscripción
                var nuevaSuscripcion = new Suscripcion
                {
                    UsuarioId = usuarioId,
                    PlanId = planId,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow.AddMonths(1), // Suscripción por 1 mes
                    Activa = true
                };

                await Crud<Suscripcion>.Create(nuevaSuscripcion);

                TempData["SuccessMessage"] = "¡Plan cambiado exitosamente!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cambiar el plan: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public ActionResult ConfirmarCambio(int planId)
        {
            try
            {
                var plan = Crud<Plan>.GetById(planId);
                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el plan: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Historial()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                var suscripciones = Crud<Suscripcion>.GetAll()
                    .Where(s => s.UsuarioId == usuarioId)
                    .OrderByDescending(s => s.FechaInicio)
                    .ToList();

                var planes = Crud<Plan>.GetAll();
                ViewBag.Planes = planes;

                return View(suscripciones);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al cargar el historial: {ex.Message}";
                return View(new List<Suscripcion>());
            }
        }
    }
}
