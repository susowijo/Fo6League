using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using League.Models;

namespace League.Controllers
{
    public class ManagersController : Controller
    {
        private LeagueDBEntities db = new LeagueDBEntities();

        // GET: Managers
        public async Task<ActionResult> Index()
        {
            return View(await db.Managers.ToListAsync());
        }

        // GET: Managers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // GET: Managers/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name");
            return View();
        }

        // POST: Managers/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Manager manager, HttpPostedFileBase picture)
        {
            if (picture == null || picture.ContentLength == 0)
            {
                ModelState.AddModelError("", "la Photo es requis!");
                return View(manager);
            }

            byte[] b = new byte[picture.ContentLength];
            picture.InputStream.Read(b, 0, b.Length);
            manager.Photo = b;
            manager.PhotoMIME = picture.ContentType;
            if (ModelState.IsValid)
            {
                manager.Account.Role = "Manager";
                manager.Account.Password = db.Sp_Encrypt(manager.Account.Password).First();
                manager.Account.ConfirmPassword = manager.Account.Password;

                db.Managers.Add(manager);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch
                {
                    manager.Account.Password = "";
                    manager.Account.ConfirmPassword = "";
                    ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", manager.TeamID);
                    ModelState.AddModelError("Account.Pseudo", "Pseudo deja utilise, veuillez entrer un autre pseudo");
                    return View(manager);
                }
                return RedirectToAction("Index");
            }

            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", manager.TeamID);
            return View(manager);
        }

        // GET: Managers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // POST: Managers/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,PhoneNumber,Photo,PhotoMIME,TeamID")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manager).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(manager);
        }

        // GET: Managers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = await db.Managers.FindAsync(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }

        // POST: Managers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Manager manager = await db.Managers.FindAsync(id);
            db.Managers.Remove(manager);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
