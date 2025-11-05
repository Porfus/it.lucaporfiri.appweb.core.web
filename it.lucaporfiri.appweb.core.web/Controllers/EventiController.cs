using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using it.lucaporfiri.appweb.core.web.ViewModels.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static it.lucaporfiri.appweb.core.web.Models.Eventi;

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

        public async Task<ActionResult> BachecaEventiAsync()
        {
            // definisce le colonne in modo programmatico 
            var statiWorkflow = (StatoWorkflow[])Enum.GetValues(typeof(StatoWorkflow));

            //trasforma gli stati in una lista di stringhe
            var nomiStatiWorkflow = statiWorkflow.Select(stato => stato.ToString()).ToList();

            //aggiorno gli eventi automatici
            await _serviziEvento.SincronizzaEventiAutomatici();


            //estrae tutti gli eventi non completati
            List<Eventi> eventiAttivi = _serviziEvento.GetEventiAttivi();

            //prioritizza gli eventi
            _serviziEvento.PrioritizzaEventi(eventiAttivi);

            // raggruppa i task per il loro stato attuale 
            var taskRaggruppatiPerStato = eventiAttivi.GroupBy(task => task.Stato).ToDictionary(g => g.Key, g => g.ToList());

            // costruisco il ViewModel
            BachecaEventiViewModel vm = new BachecaEventiViewModel()
            {
                Colonne = new List<BachecaEventiColonnaViewModel>()
            };

            foreach (var stato in statiWorkflow)
            {

                var colonna = new BachecaEventiColonnaViewModel
                {
                    Titolo = _serviziEvento.GetTitoloPerColonna(stato),
                    IdColonna = $"colonna-{stato.ToString().ToLower().Replace(" ", "-")}"
                };

                if (taskRaggruppatiPerStato.ContainsKey(stato))
                {
                    foreach (var evento in taskRaggruppatiPerStato[stato])
                    {
                        var eventoVm = new BachecaEventiEventoViewModel
                        {
                            Id = evento.Id,
                            Titolo = evento.Titolo,
                            Descrizione = evento.Descrizione,
                            PrioritaCssClass = _serviziEvento.GetCssClassPerPriorita(evento.Priorita ?? 0),
                            IsUrgente = (evento.DataScadenza - DateTime.Now).TotalDays < 2,
                            DataScadenzaLabel = _serviziEvento.FormattaDataScadenza(evento.DataScadenza),
                            IconaTipoTask = _serviziEvento.GetIconaPerTipoEvento(evento.Tipo),
                            IsCompletato = evento.Stato == StatoWorkflow.Completato
                        };
                        colonna.Eventi.Add(eventoVm);
                    }
                }                    
                vm.Colonne.Add(colonna);
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult AggiornaStatoEvento([FromBody] AggiornaStatoEvento model) 
        {
            if (!ModelState.IsValid) 
            {
                return Json(new { successo = false, messaggio = "Dati non validi." });
            }
            try
            {
                _serviziEvento.AggiornaStatoEvento(model.EventoId, model.NuovoStato);
                return Json(new { successo = true });
            }
            catch (Exception ex)
            {
                return Json(new { successo = false, messaggio = ex.Message });
            }
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
