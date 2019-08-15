using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quiz6Q2;

namespace Quiz6Q2.Controllers
{
    public class GetUserSongs
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
    public class SongDownloads
    {
        public string Title { get; set; }
        public int Downloads { get; set; }
    }
    public class SongsController : Controller
    {
        private ItunesCopyEntities db = new ItunesCopyEntities();

        // GET: Songs
        public ActionResult Index()
        {
            var songs = db.Songs.Include(s => s.Artist);
            return View(songs.ToList());
        }

        public ActionResult MostPopularSong()
        {
            Dictionary<int, int> SongPop = new Dictionary<int, int>();
            foreach(var song in db.Songs)
            {
                SongPop.Add(song.Id, song.UserSongs.Count);
            }
            var mostPop = SongPop.OrderByDescending(x => x.Value).First();
            return View(db.Songs.FirstOrDefault(x=>x.Id == mostPop.Key));
        }

        public ActionResult UserSongs(int id)
        {
            var songs = db.Database.SqlQuery<GetUserSongs>("EXEC GetUserSongs @UserId", new SqlParameter("UserId", id)).ToList();
            return View(songs);

        }

        public ActionResult SongDownloads(int id)
        {
            var song = db.Database.SqlQuery<SongDownloads>("EXEC GetSongDownloads @SongId", new SqlParameter("SongId", id)).ToList().First();
            return View(song);

        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,ArtistId")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,ArtistId")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();
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
