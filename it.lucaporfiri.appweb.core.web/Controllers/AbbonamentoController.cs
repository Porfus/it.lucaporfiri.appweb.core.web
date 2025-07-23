using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AbbonamentoController : Controller
    {    
        private readonly ServiziAbbonamento serviziAbbonamento;
        private readonly ServiziAtleta serviziAtleta;

        public AbbonamentoController(ServiziAbbonamento serviziAbbonamento, ServiziAtleta serviziAtleta)
        {
            this.serviziAbbonamento = serviziAbbonamento;
            this.serviziAtleta = serviziAtleta;
        }

        // GET: Abbonamento
        public async Task<IActionResult> Index(bool soloScaduti = false)
        {
            var abbonamenti = await serviziAbbonamento.GetAbbonamentiAsync();/*_context.Abbonamento.Include(a => a.Atleta)*/;
            var vm = abbonamenti.Select(s => new AbbonamentoDetailViewModel
            {
                abbonamento = s,
                statoAbbonamento = s.Atleta != null? serviziAbbonamento.CalcolaStatoAbbonamento(s) : AtletaDetailViewModel.StatoAbbonamento.NonDefinito
            }).ToList();
            if (soloScaduti == true)
            {
                var soloScadutiVm = vm
                    .Where(s => s.statoAbbonamento == AtletaDetailViewModel.StatoAbbonamento.Scaduto)
                    .GroupBy(s => s.abbonamento.AtletaId)
                    .Select(g => g.OrderByDescending(s => s.abbonamento.DataFine).First())
                    .ToList();

                // Escludi gli atleti che hanno almeno un abbonamento attivo
                var atletiConAbbonamentoAttivo = vm
                    .Where(s => s.statoAbbonamento == AtletaDetailViewModel.StatoAbbonamento.Valido)
                    .Select(s => s.abbonamento.AtletaId)
                    .Distinct()
                    .ToHashSet();

                soloScadutiVm = soloScadutiVm
                    .Where(s => !atletiConAbbonamentoAttivo.Contains(s.abbonamento.AtletaId))
                    .ToList();

                vm = soloScadutiVm;
            }
            return View(vm);
        }

        // GET: Abbonamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var abbonamento = await serviziAbbonamento.DaiAbbonamentoAsync(id);
            if (abbonamento == null)
            {
                return NotFound();
            }

            return View(abbonamento);
        }

        // GET: Abbonamento/Create
        public async Task<IActionResult> Create(int? atletaId = null)
        {            
            AbbonamentoCreateViewModel vm = new AbbonamentoCreateViewModel();

            if (atletaId.HasValue)
            {            
                var atleta = await serviziAtleta.DaiAtletaAsync(atletaId);

                if (atleta != null)
                {
                    vm.AtletaId = atleta.Id;
                    vm.NomeAtleta = $"{atleta.Nome} {atleta.Cognome}";
                }
            }
            if (vm.AtletaId == 0)
            {
                ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            }
            return View(vm);
        }

        // POST: Abbonamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AbbonamentoCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.DataFine < vm.DataInizio)
                {
                    ModelState.AddModelError("DataFine", "La data di fine deve essere successiva alla data di inizio.");
                    return View(vm);
                }
                var atleta = await serviziAtleta.DaiAtletaAsync(vm.AtletaId);
                if (atleta == null)
                {
                    ModelState.AddModelError("AtletaId", "Atleta non trovato.");
                }
                if (!ModelState.IsValid)
                {
                    ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
                    return View(vm);
                }
                var nuovoAbbonamento = new Abbonamento
                {
                    Nome = vm.NomeAbbonamento,
                    DataInizio = vm.DataInizio,
                    DataFine = vm.DataFine,
                    AtletaId = vm.AtletaId,
                    Atleta = atleta
                };
                await serviziAbbonamento.CreaAbbonamento(nuovoAbbonamento);
                return RedirectToAction("Details", "Atleta", new { id = vm.AtletaId });
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            return View(vm);
        }

        // GET: Abbonamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await serviziAbbonamento.DaiAbbonamentoAsync(id);/*_context.Abbonamento.FindAsync(id)*/;
            if (abbonamento == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();/*new SelectList(_context.Atleta, "Id", "Id", abbonamento.AtletaId)*/;
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
                // Fix for CS8601: Possibile assegnazione di riferimento Null.
                try
                {
                   await serviziAbbonamento.ModificaAbbonamento(abbonamento);
 
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
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti(); /*new SelectList(_context.Atleta, "Id", "Id", abbonamento.AtletaId)*/;
            return View(abbonamento);
        }

        // GET: Abbonamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var abbonamento = await serviziAbbonamento.DaiAbbonamentoAsync(id);
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
            var abbonamento = await serviziAbbonamento.DaiAbbonamentoAsync(id);
            await serviziAbbonamento.EliminaAbbonamento(id);
            if (abbonamento != null) 
            {
                return RedirectToAction("Details", "Atleta", new { id = abbonamento.AtletaId });
            }
            else
                return RedirectToAction(nameof(Index));
        }

        private bool AbbonamentoExists(int id)
        {
            return serviziAbbonamento.DaiAbbonamento(id) != null;
        }
    }
}
