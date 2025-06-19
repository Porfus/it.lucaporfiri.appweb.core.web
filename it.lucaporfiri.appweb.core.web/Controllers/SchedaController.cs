using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class SchedaController : Controller
    {
        private readonly ContestoApp _context;

        public SchedaController(ContestoApp context)
        {
            _context = context;
        }

        // GET: Scheda
        public async Task<IActionResult> Index()
        {
            var contestoApp = _context.Scheda.Include(s => s.Cliente);
            return View(await contestoApp.ToListAsync());
        }

        // GET: Scheda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await _context.Scheda
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheda == null)
            {
                return NotFound();
            }

            return View(scheda);
        }

        // GET: Scheda/Create
        public IActionResult Create()
        {
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id");
            return View();
        }

        // POST: Scheda/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descrizione,DataInizio,DataFine,AtletaId")] Scheda scheda)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);
            return View(scheda);
        }

        // GET: Scheda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await _context.Scheda.FindAsync(id);
            if (scheda == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);
            return View(scheda);
        }

        // POST: Scheda/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descrizione,DataInizio,DataFine,AtletaId")] Scheda scheda)
        {
            if (id != scheda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedaExists(scheda.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);
            return View(scheda);
        }

        // GET: Scheda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await _context.Scheda
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheda == null)
            {
                return NotFound();
            }

            return View(scheda);
        }

        // POST: Scheda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheda = await _context.Scheda.FindAsync(id);
            if (scheda != null)
            {
                _context.Scheda.Remove(scheda);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchedaExists(int id)
        {
            return _context.Scheda.Any(e => e.Id == id);
        }
    }
}
