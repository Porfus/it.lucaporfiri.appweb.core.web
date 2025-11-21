using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Helpers;
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
using static it.lucaporfiri.appweb.core.web.Models.Evento;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class EventiController : Controller
    {
        private readonly ContestoApp _context;
        private readonly ServiziEvento _serviziEvento;
        private readonly ServiziAtleta _serviziAtleta;

        public EventiController(ContestoApp context, ServiziEvento serviziEvento, ServiziAtleta serviziAtleta)
        {
            _context = context;
            _serviziEvento = serviziEvento;
            _serviziAtleta = serviziAtleta;
        }

        // GET: BachecaEventi
        [ActionName("BachecaEventi")]
        public async Task<ActionResult> BachecaEventiAsync()
        {
            // definisce le colonne in modo programmatico 
            var statiWorkflow = (StatoWorkflow[])Enum.GetValues(typeof(StatoWorkflow));

            //trasforma gli stati in una lista di stringhe
            var nomiStatiWorkflow = statiWorkflow.Select(stato => stato.ToString()).ToList();

            //aggiorno gli eventi automatici
            await _serviziEvento.SincronizzaEventiAutomatici();

            //inizializza le posizioni mancanti degli eventi (default di priorità)
            _serviziEvento.InizializzaPosizioniMancanti();

            //estrae tutti gli eventi non completati
            List<Evento> eventiAttivi = _serviziEvento.GetEventiAttivi();

            //estrae eventi completati non scaduti negli ultimi 7 giorni
            List<Evento> eventiCompletatiRecente = _serviziEvento.GetEventiCompletatiRecenti(7);

            //prioritizza gli eventi
            _serviziEvento.PrioritizzaEventi(eventiAttivi);

            // raggruppa i task per il loro stato attuale
            eventiAttivi.AddRange(eventiCompletatiRecente);
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
                    //ordina i task all'interno della colonna in base all'ordine definito
                    foreach (var evento in taskRaggruppatiPerStato[stato].OrderBy(e => e.Posizione))
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
                            IsCompletato = evento.Stato == StatoWorkflow.Completato,
                            Posizione = evento.Posizione ?? 0
                        };
                        colonna.Eventi.Add(eventoVm);
                    }
                }                    
                vm.Colonne.Add(colonna);
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AggiornaStatoEvento([FromBody] AggiornaStatoEvento model) 
        {
            if (!ModelState.IsValid) 
            {
                return Json(new { successo = false, messaggio = "Dati non validi." });
            }
            try
            {
                await _serviziEvento.AggiornaStatoEvento(model.EventoId, model.NuovoStato, model.NuovaPosizione);
                return Json(new { successo = true });
            }
            catch (Exception ex)
            {
                return Json(new { successo = false, messaggio = ex.Message });
            }
        }

        //GET: Eventi/CreaEventoManuale
        public IActionResult CreaEventoManuale() 
        {
            CreaEventoManualeViewModel vm = new CreaEventoManualeViewModel();
            //List<Atleta> atletiAttivi = _serviziAtleta.GetAtletiAttivi();
            //vm.OpzioniAtleti = atletiAttivi.Select(atleta => new SelectListItem
            //{
            //    // Testo visualizzato: es. "Mario Rossi"
            //    Text = $"{atleta.Nome} {atleta.Cognome}",

            //    // Valore dell'opzione: l'ID dell'atleta
            //    Value = atleta.Id.ToString()

            //}).ToList();
            return PartialView("_Partial/_CreaEventoManuale", vm);

        }

        // POST: Eventi/CreaEventoManuale
        [HttpPost]
        public async Task<IActionResult> CreaEventoManualeAsync(CreaEventoManualeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var nuovoEvento = new Evento
                {
                    Titolo = viewModel.Titolo,
                    DataScadenza = viewModel.DataScadenza,
                    Tipo = viewModel.TipoEventoSelezionato,
                    Stato = StatoWorkflow.Inbox,
                    //AtletaId = viewModel.AtletaSelezionatoId,
                    Descrizione = viewModel.Descrizione
                };

                await _serviziEvento.CreaEventoAsync(nuovoEvento);
                var eventoCardViewModel = new BachecaEventiEventoViewModel 
                {
                    Id = nuovoEvento.Id,
                    Titolo = nuovoEvento.Titolo,
                    Descrizione = nuovoEvento.Descrizione,
                    PrioritaCssClass = _serviziEvento.GetCssClassPerPriorita(nuovoEvento.Priorita ?? 0),
                    IsUrgente = (nuovoEvento.DataScadenza - DateTime.Now).TotalDays < 2,
                    DataScadenzaLabel = _serviziEvento.FormattaDataScadenza(nuovoEvento.DataScadenza),
                    IconaTipoTask = _serviziEvento.GetIconaPerTipoEvento(nuovoEvento.Tipo),
                    IsCompletato = nuovoEvento.Stato == StatoWorkflow.Completato
                };

                string nuovoCardHtml = await this.RenderViewToStringAsync("_Partial/_TemplateEventoCard", eventoCardViewModel);
                // Restituisce un JSON di successo con l'HTML del nuovo cartellino evento
                return Json(new
                {
                    successo = true,
                    nuovoCardHtml = nuovoCardHtml
                });
            }

            // Se il modello non è valido, restituisce la Partial View di nuovo
            // con i messaggi di errore. L'AJAX la userà per rimpiazzare il form.
            // Dobbiamo ripopolare la dropdown!
            viewModel.OpzioniTipoEvento = Enum.GetValues(typeof(TipoEvento))
                                        .Cast<TipoEvento>()
                                        .Where(tipo => tipo != TipoEvento.ScadenzaScheda)
                                        .Select(tipo => new SelectListItem
                                        {
                                            Text = tipo.ToString(),
                                            Value = ((int)tipo).ToString()
                                        }).ToList();

            //List<Atleta> atletiAttivi = _serviziAtleta.GetAtletiAttivi();
            //viewModel.OpzioniAtleti = atletiAttivi.Select(atleta => new SelectListItem
            //{
            //    Text = $"{atleta.Nome} {atleta.Cognome}",
            //    Value = atleta.Id.ToString()
            //}).ToList();

            return PartialView("_CreaEventoManuale", viewModel);
        }

        //GET: Eventi/OrdinaEventi
        public async Task<IActionResult> OrdinaEventi(bool dataScadenza = false)
        {
            await _serviziEvento.OrdinaEventiAsync(dataScadenza);

            return RedirectToAction("BachecaEventi");
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
        public async Task<IActionResult> Create([Bind("Id,Titolo,Descrizione,DataScadenza,Priorita,AtletaId,Stato")] Evento eventi)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titolo,Descrizione,DataScadenza,Priorita,AtletaId,Stato")] Evento eventi)
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
