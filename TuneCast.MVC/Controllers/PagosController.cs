using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    public class PagosController : Controller
    {
        // GET: PagosController
        public ActionResult Index()
        {
            var data = Crud<Pago>.GetAll();
            return View(data);
        }

        // GET: PagosController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Pago>.GetById(id);
            return View(data);
        }

        // GET: PagosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago data)
        {
            try
            {
                Crud<Pago>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PagosController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Pago>.GetById(id);
            return View(data);
        }

        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago data)
        {
            try
            {
                Crud<Pago>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PagosController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Pago>.GetById(id);
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
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
    }
}
