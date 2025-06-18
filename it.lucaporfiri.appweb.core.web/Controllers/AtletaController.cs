using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.ViewModels;
using it.lucaporfiri.appweb.core.web.Servizi;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AtletaController : Controller
    {
        private readonly ContestoApp _context;
        private readonly ServiziAtleta _serviziAtleta; // Add this field

        public AtletaController(ContestoApp context)
        {
            _context = context;
            _serviziAtleta = new ServiziAtleta(context); // Initialize the instance
        }
        // GET: Abbonamento
        public async Task<IActionResult> Index()
        {
            var contestoApp = _context.Atleta;
            return View(await contestoApp.ToListAsync());
        }
        // GET: Atleta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var atleta = await _context.Atleta.Include(a => a.Abbonamenti)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (atleta == null)
            {
                return NotFound();
            }

            // Create the ViewModel
            var vm = new AtletaDetailViewModel
            {
                Id = atleta.Id,
                NomeCompleto = $"{atleta.Nome} {atleta.Cognome}",
                Eta = _serviziAtleta.CalcolaEta(atleta),
                Email = atleta.Email,
                Telefono = atleta.Telefono,
                DataIscrizioneAtleta = atleta.DataInizioIscrizione,
                Tipo = atleta.Tipo,
                Stato = atleta.Stato,
                Abbonamento = _serviziAtleta.CalcolaStatoAbbonamento(atleta),
                Abbonamenti = atleta.Abbonamenti,
                Schede = atleta.Schede
            };

            return View(vm);
        }

        // GET: Atleta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Atleta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Cognome,AnnoDiNascita,Email,Telefono,DataInizioIscrizione,Tipo,Stato")] Atleta atleta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(atleta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(atleta);
        }

        // GET: Atleta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var atleta = await _context.Atleta.FindAsync(id);
            if (atleta == null)
            {
                return NotFound();
            }
            return View(atleta);
        }

        // POST: Atleta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cognome,AnnoDiNascita,Email,Telefono,DataInizioAbbonamento,DataFineAbbonamento,Tipo,Stato")] Atleta atleta)
        {
            if (id != atleta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(atleta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AtletaExists(atleta.Id))
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
            return View(atleta);
        }

        // GET: Atleta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var atleta = await _context.Atleta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (atleta == null)
            {
                return NotFound();
            }

            return View(atleta);
        }

        // POST: Atleta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var atleta = await _context.Atleta.FindAsync(id);
            if (atleta != null)
            {
                _context.Atleta.Remove(atleta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AtletaExists(int id)
        {
            return _context.Atleta.Any(e => e.Id == id);
        }
    }
}
