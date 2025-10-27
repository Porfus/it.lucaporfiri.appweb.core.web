using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class EventiController : Controller
    {
        private readonly ContestoApp _context;
        private readonly ServiziEvento _serviziEvento;

        public EventiController(ContestoApp context, ServiziEvento serviziEvento)
        {
            _context = context;
            _serviziEvento = serviziEvento;
        }

        public ActionResult TaskBoard() 
        {
            _serviziEvento.SincronizzaEventiAutomatici();
            return View();
        }

        // GET: Eventi
        public async Task<IActionResult> Index()
        {
            var contestoApp = _context.Eventi.Include(e => e.Atleta);
            return View(await contestoApp.ToListAsync());
        }

        // GET: Eventi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventi = await _context.Eventi
                .Include(e => e.Atleta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventi == null)
            {
                return NotFound();
            }

            return View(eventi);
        }

        // GET: Eventi/Create
        public IActionResult Create()
        {
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Cognome");
            return View();
        }

        // POST: Eventi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titolo,Descrizione,DataScadenza,Priorita,AtletaId,Stato")] Eventi eventi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Cognome", eventi.AtletaId);
            return View(eventi);
        }

        // GET: Eventi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventi = await _context.Eventi.FindAsync(id);
            if (eventi == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Cognome", eventi.AtletaId);
            return View(eventi);
        }

        // POST: Eventi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titolo,Descrizione,DataScadenza,Priorita,AtletaId,Stato")] Eventi eventi)
        {
            if (id != eventi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventiExists(eventi.Id))
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
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Cognome", eventi.AtletaId);
            return View(eventi);
        }

        // GET: Eventi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventi = await _context.Eventi
                .Include(e => e.Atleta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventi == null)
            {
                return NotFound();
            }

            return View(eventi);
        }

        // POST: Eventi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventi = await _context.Eventi.FindAsync(id);
            if (eventi != null)
            {
                _context.Eventi.Remove(eventi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventiExists(int id)
        {
            return _context.Eventi.Any(e => e.Id == id);
        }
    }
}
