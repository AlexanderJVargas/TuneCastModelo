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
        //[Authorize(Roles = "Administrador")]
        public ActionResult Dashboard()
        {
            var planes = Crud<Plan>.GetAll();
            return View(planes);
        }
        // GET: PlanesController
        //[Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var data = Crud<Plan>.GetAll();
            return View(data);
        }

        // GET: PlanesController/Details/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Details(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // GET: PlanesController/Create
        //[Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlanesController/Create
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public ActionResult Create(Plan data)
        {
            try
            {
                // Calcular precio con IVA
                data.Precio = data.Precio * 1.15;
                Crud<Plan>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlanesController/Edit/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // POST: PlanesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id, Plan data)
        {
            try
            {
                Crud<Plan>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlanesController/Delete/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            var data = Crud<Plan>.GetById(id);
            return View(data);
        }

        // POST: PlanesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Plan data)
        {
            try
            {
                Crud<Plan>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
        [Authorize] // Cualquier usuario autenticado puede ver planes para suscribirse
        public ActionResult PlanesDisponibles()
        {
            var planes = Crud<Plan>.GetAll();
            return View(planes);
        }
    }
}
