using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    public class AnunciosController : Controller
    {
        // GET: AnunciosController
        public ActionResult Index()
        {
            var data = Crud<Anuncio>.GetAll();
            return View(data);
        }

        // GET: AnunciosController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Anuncio>.GetById(id);
            return View(data);
        }

        // GET: AnunciosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnunciosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Anuncio data)
        {
            try
            {
                Crud<Anuncio>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: AnunciosController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Anuncio>.GetById(id);
            return View(data);
        }

        // POST: AnunciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Anuncio data)
        {
            try
            {
                Crud<Anuncio>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: AnunciosController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Anuncio>.GetById(id);
            return View(data);
        }

        // POST: AnunciosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Anuncio data)
        {
            try
            {
                Crud<Anuncio>.Delete(id);
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
