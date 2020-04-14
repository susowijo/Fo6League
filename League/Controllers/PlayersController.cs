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
    public class PlayersController : Controller
    {
        private LeagueDBEntities db = new LeagueDBEntities();

        // GET: Players
        public async Task<ActionResult> Index()
        {
            var players = db.Players.Include(p => p.Team);
            return View(await players.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name");
            return View();
        }

        // POST: Players/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Player player, HttpPostedFileBase picture)
        {
            if (picture == null || picture.ContentLength == 0)
            {
                ModelState.AddModelError("", "la Photo es requis!");
                return View(player);
            }

            byte[] b = new byte[picture.ContentLength];
            picture.InputStream.Read(b, 0, b.Length);
            player.Photo = b;
            player.PhotoMIME = picture.ContentType;

            player.Age = DateTime.Now.Year - player.BirthDate.Year;
            
            if (ModelState.IsValid)
            {
                player.Account.Role = "Player";
                player.Account.Password = db.Sp_Encrypt(player.Account.Password).First();
                player.Account.ConfirmPassword = player.Account.Password;

                db.Players.Add(player);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch
                {
                    player.Account.Password = "";
                    player.Account.ConfirmPassword = "";
                    ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
                    ModelState.AddModelError("Account.Pseudo", "Pseudo deja utilise, veuillez entrer un autre pseudo");
                    return View(player);
                }
                
                return RedirectToAction("Index");
            }

            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            return View(player);
        }

        // POST: Players/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Post,PhoneNumber,BirthDate,Nation,TeamID,Dorsa,Pseudo,Password,Photo,PhotoMIME,Poids,Taille")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", player.TeamID);
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Player player = await db.Players.FindAsync(id);
            db.Players.Remove(player);
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
