//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using TuneCastAPIConsumer;
//using TuneCastModelo;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace TuneCast.MVC.Controllers
//{
//    public class SubscripcionesController : Controller
//    {
//        // GET: SubscripcionesController
//        public ActionResult Index()
//        {
//            var data = Crud<Suscripcion>.GetAll(); 
//            return View(data);
//        }

//        // GET: SubscripcionesController/Details/5
//        public ActionResult Details(int id)
//        {
//            var data = Crud<Suscripcion>.GetById(id);
//            return View(data);
//        }

//        // GET: SubscripcionesController/Create
//        public ActionResult Create()
//        {
//            return View(   );
//        }

//        // POST: SubscripcionesController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Suscripcion data)
//        {
//            try
//            {
//                Crud<Suscripcion>.Create(data);
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", ex.Message);
//                return View(data);
//            }
//        }

//        // GET: SubscripcionesController/Edit/5
//        public ActionResult Edit(int id)
//        {
//            var data = Crud<Suscripcion>.GetById(id);
//            return View(data);
//        }

//        // POST: SubscripcionesController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(int id, Suscripcion data)
//        {
//            try
//            {
//                Crud<Suscripcion>.Update(id, data);
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", ex.Message);
//                return View(data);
//            }
//        }

//        // GET: SubscripcionesController/Delete/5
//        public ActionResult Delete(int id)
//        {
//            var data = Crud<Suscripcion>.GetById(id);
//            return View(data);
//        }

//        // POST: SubscripcionesController/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(int id, Suscripcion data)
//        {
//            try
//            {
//                Crud<Suscripcion>.Delete(id);
//                return RedirectToAction(nameof(Index));
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError("", ex.Message);
//                return View(data);
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TuneCastModelo; // Asegúrate de que este espacio de nombres contiene tu modelo Plan

namespace TuneCast.MVC.Controllers
{
    [Authorize]
    public class SubscripcionesController : Controller
    {
        // Lista temporal de planes (en un sistema real esto vendría de tu API)
        private static List<Plan> _planes = new List<Plan>
        {
            new Plan { Id = 1, Nombre = "Personal", Precio = 1.0, Descripcion = "Acceso básico" },
            new Plan { Id = 2, Nombre = "Empresarial", Precio = 3.50, Descripcion = "Acceso completo" },
            new Plan { Id = 3, Nombre = "Familiar", Precio = 35.0, Descripcion = "Para toda la familia" }
        };

        [HttpGet]
        public IActionResult Subscribe()
        {
            return View(_planes);
        }

        [HttpPost]
        public IActionResult Subscribe(int planId)
        {
            // Aquí normalmente llamarías a tu API para registrar la suscripción
            // Por ahora solo mostramos un mensaje de éxito

            TempData["SuccessMessage"] = $"¡Suscripción al plan {planId} realizada con éxito! (Simulación)";
            return RedirectToAction("Index", "Home");
        }
    }

    // Clase Plan temporal (deberías reemplazarla con tu modelo real cuando integres con la API)
 
}
