using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HWA3.Models;

namespace HWA3.Controllers
{
    public class authorsController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: authors
        public async Task<ActionResult> Index()
        {
            return View(await db.authors.ToListAsync());
        }

        // GET: authors/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "authorId,name,surname")] author author)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction("Maintain", "Home");
            }

            return RedirectToAction("Maintain", "Home");
        }

        // GET: authors/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "authorId,name,surname")] author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Maintain", "Home");
            }
            return RedirectToAction("Maintain", "Home");
        }

        // GET: authors/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = await db.authors.FindAsync(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            author author = await db.authors.FindAsync(id);
            db.authors.Remove(author);
            await db.SaveChangesAsync();
            return RedirectToAction("Maintain", "Home");
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
