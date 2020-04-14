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
    public class MatchesController : Controller
    {
        private LeagueDBEntities db = new LeagueDBEntities();

        // GET: Matches
        public async Task<ActionResult> Index()
        {
            var matches = db.Matches.Include(m => m.Day).Include(m => m.Team).Include(m => m.Stadium).Include(m => m.Team1);
            return View(await matches.ToListAsync());
        }

        // GET: Matches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: Matches/Create
        public async Task<ActionResult> Create()
        {
            db.Matches.RemoveRange(db.Matches);
            db.SaveChanges();

            List<Match> matches = new List<Match>();
            var teams = db.Teams.ToList();
            int[] indices = new int[2];
            Random random = new Random();
            
            // sort match
            while(teams.Count >= 2)
            {
                int team1 = random.Next(0,teams.Count);
                int team2;
                do
                {
                    team2 = random.Next(0,teams.Count);
                } while (team1 == team2);
                matches.Add(new Match { TeamID = teams[team1].ID, TeamID2 = teams[team2].ID });

                if(team1 < team2)
                {
                    teams.RemoveAt(team1);
                    teams.RemoveAt(team2 -1);
                }
                else
                {
                    teams.RemoveAt(team2);
                    teams.RemoveAt(team1 - 1);
                }
            }

            var arbitres = db.Arbitres.ToList();
            var stadia = db.Stadia.ToList();
            for (int i=0; i<matches.Count; i++)
            {
                // define arbitres
                if(i < arbitres.Count)
                {
                    matches[i].Arbitres.Add(arbitres[i]);
                }
                else if(arbitres.Count > 0)
                {
                    matches[i].Arbitres.Add(arbitres[random.Next(0,arbitres.Count - 1)]);
                }

                // define stadia
                if(i < stadia.Count)
                {
                    matches[i].Stadium = stadia[i];
                }
                else if(stadia.Count > 0)
                {
                    matches[i].Stadium = stadia[random.Next(0,stadia.Count - 1)];
                }
            }
            
            foreach(var i in matches)
            {
                db.Matches.Add(i);
            }

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: Matches/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
       
        // GET: Matches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.DayID = new SelectList(db.Days, "ID", "Name", match.DayID);
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", match.TeamID);
            ViewBag.StadiumID = new SelectList(db.Stadia, "ID", "City", match.StadiumID);
            ViewBag.TeamID2 = new SelectList(db.Teams, "ID", "Name", match.TeamID2);
            return View(match);
        }

        // POST: Matches/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,MatchDate,NumberOfSpectators,TeamID,TeamID2,StadiumID,DayID")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DayID = new SelectList(db.Days, "ID", "Name", match.DayID);
            ViewBag.TeamID = new SelectList(db.Teams, "ID", "Name", match.TeamID);
            ViewBag.StadiumID = new SelectList(db.Stadia, "ID", "City", match.StadiumID);
            ViewBag.TeamID2 = new SelectList(db.Teams, "ID", "Name", match.TeamID2);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Match match = await db.Matches.FindAsync(id);
            db.Matches.Remove(match);
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
