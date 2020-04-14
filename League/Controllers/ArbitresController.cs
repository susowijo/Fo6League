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
    public class ArbitresController : Controller
    {
        private LeagueDBEntities db = new LeagueDBEntities();

        // GET: Arbitres
        public async Task<ActionResult> Index()
        {
            return View(await db.Arbitres.ToListAsync());
        }

        // GET: Arbitres/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arbitre arbitre = await db.Arbitres.FindAsync(id);
            if (arbitre == null)
            {
                return HttpNotFound();
            }
            return View(arbitre);
        }

        // GET: Arbitres/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Arbitres/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Arbitre arbitre, HttpPostedFileBase picture)
        {
            if (picture == null || picture.ContentLength == 0)
            {
                ModelState.AddModelError("", "la Photo es requis!");
                return View(arbitre);
            }

            byte[] b = new byte[picture.ContentLength];
            picture.InputStream.Read(b, 0, b.Length);
            arbitre.Photo = b;
            arbitre.PhotoMIME = picture.ContentType;
            if (ModelState.IsValid)
            {
                arbitre.Account.Role = "Arbitre";
                arbitre.Account.Password = db.Sp_Encrypt(arbitre.Account.Password).First();
                arbitre.Account.ConfirmPassword = arbitre.Account.Password;

                db.Arbitres.Add(arbitre);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch
                {
                    arbitre.Account.Password = "";
                    arbitre.Account.ConfirmPassword = "";
                    ModelState.AddModelError("Account.Pseudo", "Pseudo deja utilise, veuillez entrer un autre pseudo");
                    return View(arbitre);
                }
                return RedirectToAction("Index");
            }

            return View(arbitre);
        }

        // GET: Arbitres/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arbitre arbitre = await db.Arbitres.FindAsync(id);
            if (arbitre == null)
            {
                return HttpNotFound();
            }
            return View(arbitre);
        }

        // POST: Arbitres/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Address,PhoneNumber,Photo,PhotoMIME,Role")] Arbitre arbitre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(arbitre).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(arbitre);
        }

        // GET: Arbitres/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arbitre arbitre = await db.Arbitres.FindAsync(id);
            if (arbitre == null)
            {
                return HttpNotFound();
            }
            return View(arbitre);
        }

        // POST: Arbitres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Arbitre arbitre = await db.Arbitres.FindAsync(id);
            db.Arbitres.Remove(arbitre);
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
