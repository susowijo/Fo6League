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
    public class TrainersController : Controller
    {
        private LeagueDBEntities db = new LeagueDBEntities();

        // GET: Trainers
        public async Task<ActionResult> Index()
        {
            var trainers = db.Trainers.Include(t => t.Team);
            return View(await trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainer trainer = await db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            return View(trainer);
        }

        // GET: Trainers/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name");
            return View();
        }

        // POST: Trainers/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Trainer trainer, HttpPostedFileBase picture)
        {
            if (picture == null || picture.ContentLength == 0)
            {
                ModelState.AddModelError("", "la Photo es requis!");
                return View(trainer);
            }

            byte[] b = new byte[picture.ContentLength];
            picture.InputStream.Read(b, 0, b.Length);
            trainer.Photo = b;
            trainer.PhotoMIME = picture.ContentType;
            if (ModelState.IsValid)
            {
                trainer.Account.Role = "Trainer";
                trainer.Account.Password = db.Sp_Encrypt(trainer.Account.Password).First();
                trainer.Account.ConfirmPassword = trainer.Account.Password;

                db.Trainers.Add(trainer);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch
                {
                    trainer.Account.Password = "";
                    trainer.Account.ConfirmPassword = "";
                    ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", trainer.TeamID);
                    ModelState.AddModelError("Account.Pseudo", "Pseudo deja utilise, veuillez entrer un autre pseudo");
                    return View(trainer);
                }
                return RedirectToAction("Index");
            }

            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", trainer.TeamID);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainer trainer = await db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", trainer.TeamID);
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,PhoneNumber,Address,TeamID,Photo,PhotoMIME")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", trainer.TeamID);
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainer trainer = await db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Trainer trainer = await db.Trainers.FindAsync(id);
            db.Trainers.Remove(trainer);
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
