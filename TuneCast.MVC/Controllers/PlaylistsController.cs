using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TuneCastAPIConsumer;
using TuneCastModelo;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TuneCast.MVC.Controllers
{
    public class PlaylistsController : Controller
    {
        // GET: PlaylistsController
        public ActionResult Index()
        {
            var data = Crud<Playlist>.GetAll();
            return View(data);
        }

        // GET: PlaylistsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // GET: PlaylistsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaylistsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Playlist data)
        {
            try
            {
                Crud<Playlist>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlaylistsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // POST: PlaylistsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Playlist data)
        {
            try
            {
                Crud<Playlist>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlaylistsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // POST: PlaylistsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Playlist data)
        {
            try
            {
                Crud<Playlist>.Delete(id);
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
