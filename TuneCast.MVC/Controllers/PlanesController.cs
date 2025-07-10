using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    public class PlanesController : Controller
    {
        private const double IVA_PERCENTAGE = 0.15;
        public PlanesController()
        {


            Crud<Plan>.EndPoint = "https://localhost:7194/api/Planes";
        }
        //[Authorize(Roles = "Administrador")]
        public ActionResult Dashboard()
        {
            try
            {
                var planes = Crud<Plan>.GetAll();
                return View(planes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el dashboard: {ex.Message}";
                return View(new List<Plan>());
            }
        }
        // GET: PlanesController
        //[Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            try
            {
                var planes = Crud<Plan>.GetAll();

                // Agregar mensaje de éxito si existe
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"];
                }

                return View(planes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los planes: {ex.Message}";
                return View(new List<Plan>());
            }
        }

        // GET: PlanesController/Details/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Details(int id)
        {
            try
            {
                var plan = Crud<Plan>.GetById(id);
                if (plan == null)
                {
                    TempData["ErrorMessage"] = "El plan solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }
                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el plan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PlanesController/Create
        //[Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View(new Plan());
        }

        // POST: PlanesController/Create
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Create(Plan plan)
        {
            try
            {
                // Validar datos de entrada
                if (!ModelState.IsValid)
                {
                    return View(plan);
                }

                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(plan.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El nombre del plan es requerido.");
                    return View(plan);
                }

                if (plan.Precio <= 0)
                {
                    ModelState.AddModelError("Precio", "El precio debe ser mayor a cero.");
                    return View(plan);
                }

                if (plan.Precio > 1000)
                {
                    ModelState.AddModelError("Precio", "El precio no puede ser mayor a $1000.");
                    return View(plan);
                }

                if (string.IsNullOrWhiteSpace(plan.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción es requerida.");
                    return View(plan);
                }

                // Verificar si ya existe un plan con el mismo nombre
                var planesExistentes = Crud<Plan>.GetAll();
                if (planesExistentes.Any(p => p.Nombre.Equals(plan.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Nombre", "Ya existe un plan con este nombre.");
                    return View(plan);
                }

                // Calcular precio con IVA (el precio ingresado es el precio base)
                var precioBase = plan.Precio;
                plan.Precio = Math.Round(precioBase * (1 + IVA_PERCENTAGE), 2, MidpointRounding.AwayFromZero);

                // Crear el plan
                var planCreado = await Crud<Plan>.Create(plan);

                if (planCreado?.Id > 0)
                {
                    TempData["SuccessMessage"] = $"Plan '{plan.Nombre}' creado exitosamente. Precio base: ${precioBase:F2}, Precio final: ${plan.Precio:F2}";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear el plan. Intente nuevamente.");
                    plan.Precio = precioBase; // Restaurar precio base para la vista
                    return View(plan);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el plan: {ex.Message}");
                return View(plan);
            }
        }

        // GET: PlanesController/Edit/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            try
            {
                var plan = Crud<Plan>.GetById(id);
                if (plan == null)
                {
                    TempData["ErrorMessage"] = "El plan solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }
                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el plan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlanesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id, Plan plan)
        {
            try
            {
                // Verificar que el ID coincida
                if (id != plan.Id)
                {
                    TempData["ErrorMessage"] = "Error de validación: ID no coincide.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar datos de entrada
                if (!ModelState.IsValid)
                {
                    return View(plan);
                }

                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(plan.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El nombre del plan es requerido.");
                    return View(plan);
                }

                if (plan.Precio <= 0)
                {
                    ModelState.AddModelError("Precio", "El precio debe ser mayor a cero.");
                    return View(plan);
                }

                if (plan.Precio > 1000)
                {
                    ModelState.AddModelError("Precio", "El precio no puede ser mayor a $1000.");
                    return View(plan);
                }

                if (string.IsNullOrWhiteSpace(plan.Descripcion))
                {
                    ModelState.AddModelError("Descripcion", "La descripción es requerida.");
                    return View(plan);
                }

                // Verificar si ya existe otro plan con el mismo nombre
                var planesExistentes = Crud<Plan>.GetAll();
                if (planesExistentes.Any(p => p.Nombre.Equals(plan.Nombre, StringComparison.OrdinalIgnoreCase) && p.Id != plan.Id))
                {
                    ModelState.AddModelError("Nombre", "Ya existe otro plan con este nombre.");
                    return View(plan);
                }

                // Obtener el plan original para comparación
                var planOriginal = Crud<Plan>.GetById(id);
                if (planOriginal == null)
                {
                    TempData["ErrorMessage"] = "El plan que intenta editar ya no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // Si el precio viene como precio final (con IVA), mantenerlo
                // Si viene como precio base, calcularlo
                var precioOriginalBase = Math.Round(planOriginal.Precio / (1 + IVA_PERCENTAGE), 2, MidpointRounding.AwayFromZero);
                var nuevoPrecioBase = plan.Precio;

                // Si el precio ingresado es diferente al precio final original, 
                // asumimos que es un precio base y calculamos el IVA
                if (Math.Abs(plan.Precio - planOriginal.Precio) > 0.01)
                {
                    plan.Precio = Math.Round(nuevoPrecioBase * (1 + IVA_PERCENTAGE), 2, MidpointRounding.AwayFromZero);
                }

                // Actualizar el plan
                bool actualizado = Crud<Plan>.Update(id, plan);

                if (actualizado)
                {
                    TempData["SuccessMessage"] = $"Plan '{plan.Nombre}' actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el plan. Intente nuevamente.");
                    return View(plan);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el plan: {ex.Message}");
                return View(plan);
            }
        }

        // GET: PlanesController/Delete/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                var plan = Crud<Plan>.GetById(id);
                if (plan == null)
                {
                    TempData["ErrorMessage"] = "El plan solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }
                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el plan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PlanesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Plan plan)
        {
            try
            {
                // Verificar que el plan existe antes de eliminarlo
                var planExistente = Crud<Plan>.GetById(id);
                if (planExistente == null)
                {
                    TempData["ErrorMessage"] = "El plan que intenta eliminar ya no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // TODO: Verificar si el plan tiene suscripciones activas
                // En una implementación completa, aquí verificaríamos si hay usuarios suscritos
                // y manejaríamos esa situación apropiadamente

                bool eliminado = Crud<Plan>.Delete(id);

                if (eliminado)
                {
                    TempData["SuccessMessage"] = $"Plan '{planExistente.Nombre}' eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo eliminar el plan. Puede que tenga suscripciones activas.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el plan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize] // Cualquier usuario autenticado puede ver planes para suscribirse
        public ActionResult PlanesDisponibles()
        {
            try
            {
                var planes = Crud<Plan>.GetAll();
                return View(planes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar los planes: {ex.Message}";
                return View(new List<Plan>());
            }
        }

        [HttpGet]
        public JsonResult CalcularPrecio(double precioBase)
        {
            try
            {
                if (precioBase <= 0)
                {
                    return Json(new { success = false, message = "El precio debe ser mayor a cero." });
                }

                if (precioBase > 1000)
                {
                    return Json(new { success = false, message = "El precio no puede ser mayor a $1000." });
                }

                var iva = Math.Round(precioBase * IVA_PERCENTAGE, 2, MidpointRounding.AwayFromZero);
                var precioFinal = Math.Round(precioBase + iva, 2, MidpointRounding.AwayFromZero);

                return Json(new
                {
                    success = true,
                    precioBase = Math.Round(precioBase, 2, MidpointRounding.AwayFromZero),
                    iva = iva,
                    precioFinal = precioFinal,
                    porcentajeIva = IVA_PERCENTAGE * 100
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al calcular precio: {ex.Message}" });
            }
        }
        private double ObtenerPrecioBase(double precioFinal)
        {
            return Math.Round(precioFinal / (1 + IVA_PERCENTAGE), 2, MidpointRounding.AwayFromZero);
        }

        private double CalcularPrecioFinal(double precioBase)
        {
            return Math.Round(precioBase * (1 + IVA_PERCENTAGE), 2, MidpointRounding.AwayFromZero);
        }


        public JsonResult EstadisticasPlanes()
        {
            try
            {
                var planes = Crud<Plan>.GetAll();

                var estadisticas = new
                {
                    totalPlanes = planes.Count,
                    precioPromedio = planes.Any() ? Math.Round(planes.Average(p => p.Precio), 2) : 0,
                    planMasCaro = planes.Any() ? planes.OrderByDescending(p => p.Precio).First().Nombre : "N/A",
                    planMasBarato = planes.Any() ? planes.OrderBy(p => p.Precio).First().Nombre : "N/A",
                    ingresosPotenciales = Math.Round(planes.Sum(p => p.Precio), 2)
                };

                return Json(new { success = true, data = estadisticas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

