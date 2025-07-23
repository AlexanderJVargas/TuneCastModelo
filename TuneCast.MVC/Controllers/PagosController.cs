using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace TuneCast.MVC.Controllers
{
    public class PagosController : Controller
    {
        // GET: PagosController
        public ActionResult Index()
        {
            var listaPagos = Crud<Pago>.GetAll();
            return View(listaPagos);
        }

        // GET: PagosController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Pago>.GetById(id);
            return View(data);
        }

        // GET: PagosController/Create 
        public ActionResult Create(int? planId)
        {
            try
            {
                var planes = Crud<Plan>.GetAll();
                Plan planSeleccionado = null;

                //SI VIENE UN PLANID, BUSCAR ESE PLAN ESPECÍFICO
                if (planId.HasValue && planId > 0)
                {
                    planSeleccionado = planes?.FirstOrDefault(p => p.Id == planId.Value);
                }

                //SI NO SE ENCUENTRA EL PLAN, USAR EL PRIMERO
                if (planSeleccionado == null)
                {
                    planSeleccionado = planes?.FirstOrDefault();
                }

                if (planSeleccionado != null)
                {
                    // Inicializar con el monto del plan SELECCIONADO
                    var nuevoPago = new Pago
                    {
                        FechaPago = DateTime.Now,
                        Monto = planSeleccionado.Precio //Precio del plan correcto
                    };

                    // Enviar información del plan SELECCIONADO a la vista
                    ViewData["PlanNombre"] = planSeleccionado.Nombre;
                    ViewData["PlanPrecio"] = planSeleccionado.Precio;
                    ViewData["PlanId"] = planSeleccionado.Id;

                    // Inicializar ViewData para campos de tarjeta
                    ViewData["NumeroTarjeta"] = "";
                    ViewData["FechaExpiracion"] = "";
                    ViewData["CVV"] = "";
                    ViewData["MetodoPago"] = "Tarjeta de Crédito";

                    return View(nuevoPago);
                }
                else
                {
                    // inicializar con 0 si no hay planes
                    var nuevoPago = new Pago
                    {
                        FechaPago = DateTime.Now,
                        Monto = 0
                    };

                    ViewData["PlanNombre"] = "No hay planes disponibles";
                    ViewData["PlanPrecio"] = 0;
                    ViewData["PlanId"] = 0;
                    ViewData["NumeroTarjeta"] = "";
                    ViewData["FechaExpiracion"] = "";
                    ViewData["CVV"] = "";
                    ViewData["MetodoPago"] = "Tarjeta de Crédito";

                    return View(nuevoPago);
                }
            }
            catch (Exception ex)
            {
                // En caso de error, inicializar con valores por defecto
                var nuevoPago = new Pago
                {
                    FechaPago = DateTime.Now,
                    Monto = 0
                };

                ViewData["NumeroTarjeta"] = "";
                ViewData["FechaExpiracion"] = "";
                ViewData["CVV"] = "";
                ViewData["MetodoPago"] = "Tarjeta de Crédito";
                ViewData["PlanNombre"] = "Error al cargar plan";
                ViewData["PlanPrecio"] = 0;
                ViewData["PlanId"] = 0;

                return View(nuevoPago);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Pago pago, string numeroTarjeta, string fechaExpiracion, string cvv, string metodoPago)
        {
            //OBTENER AUTOMÁTICAMENTE EL USUARIO LOGUEADO
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    pago.UsuarioId = userId; //Asignar automáticamente el ID del usuario logueado
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo identificar el usuario actual.");
                    ViewData["NumeroTarjeta"] = numeroTarjeta;
                    ViewData["FechaExpiracion"] = fechaExpiracion;
                    ViewData["CVV"] = cvv;
                    ViewData["MetodoPago"] = metodoPago;
                    return View(pago);
                }
            }
            else
            {
                // Si no está autenticado, redirigir al login
                return RedirectToAction("Login", "Account");
            }

            // Preservar datos del formulario para repoblación
            ViewData["NumeroTarjeta"] = numeroTarjeta;
            ViewData["FechaExpiracion"] = fechaExpiracion;
            ViewData["CVV"] = cvv;
            ViewData["MetodoPago"] = metodoPago;

            // Validación básica del modelo
            if (!ModelState.IsValid)
            {
                return View(pago);
            }

            // Validación de datos de tarjeta
            if (string.IsNullOrEmpty(numeroTarjeta) || numeroTarjeta.Length < 16)
            {
                ModelState.AddModelError("", "El número de tarjeta debe tener al menos 16 dígitos");
                return View(pago);
            }

            if (string.IsNullOrEmpty(fechaExpiracion) || fechaExpiracion.Length != 5 || !fechaExpiracion.Contains("/"))
            {
                ModelState.AddModelError("", "La fecha de expiración debe tener formato MM/AA");
                return View(pago);
            }

            if (string.IsNullOrEmpty(cvv) || cvv.Length != 3)
            {
                ModelState.AddModelError("", "El CVV debe tener exactamente 3 dígitos");
                return View(pago);
            }

            try
            {
                // Simulación de procesamiento de pago
                var random = new Random();
                var pagoAprobado = random.Next(1, 11) <= 8; // 80% de probabilidad

                if (!pagoAprobado)
                {
                    ModelState.AddModelError("", "Pago rechazado: Fondos insuficientes");
                    return View(pago);
                }

                // Completar información del pago
                pago.FechaPago = DateTime.UtcNow;
                pago.MetodoPago = metodoPago;

                // Guarda en la base de datos
                var pagoGuardado = await Crud<Pago>.Create(pago);

                if (pagoGuardado == null || pagoGuardado.Id == 0)
                {
                    ModelState.AddModelError("", "No se pudo completar el registro del pago");
                    return View(pago);
                }

                // Redirección a confirmación
                return RedirectToAction("PagoExitoso", new
                {
                    monto = pago.Monto,
                    ultimosDigitos = numeroTarjeta.Substring(numeroTarjeta.Length - 4)
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                return View(pago);
            }
        }

        // GET: Confirmación de pago exitoso
        public ActionResult PagoExitoso(decimal monto, string ultimosDigitos)
        {
            ViewBag.Monto = monto;
            ViewBag.UltimosDigitos = ultimosDigitos;
            return View();
        }

        // GET: PagosController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Pago>.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(data);
                }

                Crud<Pago>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return View(data);
            }
        }

        // GET: PagosController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Pago>.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        // POST: PagosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Pago data)
        {
            try
            {
                Crud<Pago>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
                return View(data);
            }
        }
    }
}