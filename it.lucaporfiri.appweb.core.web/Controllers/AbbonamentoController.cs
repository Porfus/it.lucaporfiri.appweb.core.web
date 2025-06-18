using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AbbonamentoController : Controller
    {
        private readonly ContestoApp _context;
        private readonly ServiziAbbonamento _serviziAbbonamento;
        private readonly ILogger<AbbonamentoController> logger;

        public AbbonamentoController(ContestoApp context, ILogger<AbbonamentoController> _logger)
        {
            this.logger = _logger;
            _context = context;
            _serviziAbbonamento = new ServiziAbbonamento(context);
        }

        // GET: Abbonamento
        public async Task<IActionResult> Index()
        {
            var contestoApp = _context.Abbonamento.Include(a => a.Atleta);
            return View(await contestoApp.ToListAsync());
        }

        // GET: Abbonamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamento
                .Include(a => a.Atleta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (abbonamento == null)
            {
                return NotFound();
            }

            return View(abbonamento);
        }

        // GET: Abbonamento/Create
        public IActionResult Create()
        {
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id");
            AbbonamentoCreateViewModel vm = new AbbonamentoCreateViewModel();
            return View(vm);
        }

        // POST: Abbonamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DataInizio,DataFine,AtletaId")] AbbonamentoCreateViewModel vm)
        {
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "NomeCompleto", vm.AtletaId); // Uso NomeCompleto per essere più user-friendly
            if (ModelState.IsValid)
            {
                if (vm.DataFine < vm.DataInizio)
                {
                    ModelState.AddModelError("DataFine", "La data di fine deve essere successiva alla data di inizio.");
                    return View(vm);
                }
                var atleta = await _context.Atleta.FindAsync(vm.AtletaId);
                if (atleta == null)
                {
                    ModelState.AddModelError("AtletaId", "Atleta non trovato.");
                    return View(vm);
                }
                var nuovoAbbonamento = new Abbonamento
                {
                    DataInizio = vm.DataInizio,
                    DataFine = vm.DataFine,
                    AtletaId = vm.AtletaId,
                    Atleta = atleta
                };
                _context.Add(nuovoAbbonamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Abbonamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamento.FindAsync(id);
            if (abbonamento == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", abbonamento.AtletaId);
            return View(abbonamento);
        }

        // POST: Abbonamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataInizio,DataFine,AtletaId")] Abbonamento abbonamento)
        {
            if (id != abbonamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(abbonamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbbonamentoExists(abbonamento.Id))
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
            ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", abbonamento.AtletaId);
            return View(abbonamento);
        }

        // GET: Abbonamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await _context.Abbonamento
                .Include(a => a.Atleta)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (abbonamento == null)
            {
                return NotFound();
            }

            return View(abbonamento);
        }

        // POST: Abbonamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var abbonamento = await _context.Abbonamento.FindAsync(id);
            if (abbonamento != null)
            {
                _context.Abbonamento.Remove(abbonamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AbbonamentoExists(int id)
        {
            return _context.Abbonamento.Any(e => e.Id == id);
        }
    }
}
