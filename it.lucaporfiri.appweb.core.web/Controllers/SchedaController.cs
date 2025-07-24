using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class SchedaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ServiziScheda serviziScheda;
        private readonly ServiziAtleta serviziAtleta;

        public SchedaController(ServiziScheda serviziScheda, ServiziAtleta serviziAtleta, IWebHostEnvironment webHostEnvironment)
        {
            this.serviziScheda = serviziScheda;
            this.serviziAtleta = serviziAtleta;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Scheda
        public async Task<IActionResult> Index(bool soloScadute = false)
        {
            // Utilizzo di Task.Run per eseguire l'elaborazione in un thread in background
            var schede = await Task.Run(() => serviziScheda.DaiSchede());

            var vm = schede.Select(s => new SchedaAllenamentoViewModel
            {
                Scheda = s,
                Stato = serviziScheda.CalcolaStatoScheda(s),
            }).ToList();

            if (soloScadute == true) {
                var soloSchedeScadute = vm
                    .Where(s => s.Stato == SchedaAllenamentoViewModel.StatoScheda.Scaduta)
                    .GroupBy(s => s.Scheda.AtletaId)
                    .Select(g => g.OrderByDescending(s => s.Scheda.DataFine).First())
                    .ToList();
                var atletiConSchedeAttive = vm
                    .Where(s => s.Stato == SchedaAllenamentoViewModel.StatoScheda.Attiva)
                    .Select(s => s.Scheda.AtletaId)
                    .Distinct()
                    .ToHashSet();
                soloSchedeScadute = soloSchedeScadute
                    .Where(s => !atletiConSchedeAttive.Contains(s.Scheda.AtletaId))
                    .ToList();

                vm = soloSchedeScadute;
            }

            return View(vm);
        }

        // GET: Scheda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await serviziScheda.DaiSchedaAsync(id);
            if (scheda == null)
            {
                return NotFound();
            }
            ViewData["Cliente"] = serviziAtleta.DaiSelectListAtleti();
            SchedaAllenamentoViewModel vm = new SchedaAllenamentoViewModel();
            vm.Scheda = scheda;
            vm.Stato = serviziScheda.CalcolaStatoScheda(scheda);
            return View(vm);
        }

        // GET: Scheda/Create
        public async Task<IActionResult> Create(int? atletaId = null)
        {
            SchedaCreateViewModel vm = new SchedaCreateViewModel();
            if (atletaId.HasValue) 
            {
                var atleta = await serviziAtleta.DaiAtletaAsync(atletaId);
                if (atleta != null)
                {
                    vm.AtletaId = atleta.Id;
                    vm.NomeAtleta = $"{atleta.Nome} {atleta.Cognome}";
                }
            }
            if(vm.AtletaId == 0)
            {
                ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            }
            return View(vm);
        }

        // POST: Scheda/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SchedaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.DataFine < viewModel.DataInizio)
                {
                    ModelState.AddModelError("DataFine", "La data di fine deve essere successiva alla data di inizio.");
                    if (String.IsNullOrEmpty(viewModel.NomeAtleta))
                    {
                        ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
                    }
                    return View(viewModel);
                }
                var atleta = await serviziAtleta.DaiAtletaAsync(viewModel.AtletaId);
                if (atleta == null)
                {
                    ModelState.AddModelError("AtletaId", "Atleta non trovato.");
                }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("AtletaId", "Atleta non valido, riprova o controlla se l'atleta esiste.");
                    ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
                    return View(viewModel);
                }                

                var scheda = new Scheda
                {
                    Nome = viewModel.Nome,
                    Descrizione = viewModel.Descrizione,
                    DataInizio = viewModel.DataInizio,
                    DataFine = viewModel.DataFine,
                    AtletaId = viewModel.AtletaId,
                    Cliente = atleta
                };
                // --- Logica di gestione del file ---
                if (viewModel.FileCaricato != null && viewModel.FileCaricato.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede");
                    Directory.CreateDirectory(uploadsFolder);

                    string nomeFileUnico = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.FileCaricato.FileName);
                    string percorsoCompletoFile = Path.Combine(uploadsFolder, nomeFileUnico);

                    using (var fileStream = new FileStream(percorsoCompletoFile, FileMode.Create))
                    {
                        await viewModel.FileCaricato.CopyToAsync(fileStream);
                    }

                    scheda.NomeFileOriginale = Path.GetFileName(viewModel.FileCaricato.FileName);
                    scheda.NomeFileArchiviato = nomeFileUnico;
                    scheda.ContentType = viewModel.FileCaricato.ContentType;
                }
                // --- Fine logica file ---

                await serviziScheda.CreaSchedaAsync(scheda);
                return RedirectToAction("Details", "Atleta", new { id = viewModel.AtletaId });
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            return View(viewModel);
        }
            
        // GET: Scheda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await serviziScheda.DaiSchedaAsync(id);
            if (scheda == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            SchedaAllenamentoEditViewModel vm = new SchedaAllenamentoEditViewModel();
            vm.Scheda = scheda;
            return View(vm);
        }

        // POST: Scheda/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Scheda, NuovoFileScheda")] SchedaAllenamentoEditViewModel viewModel)
        {
            if (id != viewModel.Scheda.Id)
            {
                return NotFound();
            }
            var schedaVecchia = serviziScheda.DaiScheda(id);

            if (ModelState.IsValid)
            {
                try
                {
                    var schedaDaAggiornare = await serviziScheda.DaiSchedaAsync(id);
                    if (schedaDaAggiornare == null)
                    {
                        return NotFound();
                    }

                    if (viewModel.NuovoFileScheda != null && viewModel.NuovoFileScheda.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(schedaDaAggiornare.NomeFileArchiviato))
                        {
                            string percorsoVecchioFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede", schedaDaAggiornare.NomeFileArchiviato);
                            if (System.IO.File.Exists(percorsoVecchioFile))
                            {
                                System.IO.File.Delete(percorsoVecchioFile);
                            }
                        }

                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede");
                        Directory.CreateDirectory(uploadsFolder);
                        string nomeFileUnico = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.NuovoFileScheda.FileName);
                        string percorsoCompletoFile = Path.Combine(uploadsFolder, nomeFileUnico);

                        using (var fileStream = new FileStream(percorsoCompletoFile, FileMode.Create))
                        {
                            await viewModel.NuovoFileScheda.CopyToAsync(fileStream);
                        }

                        schedaDaAggiornare.NomeFileOriginale = Path.GetFileName(viewModel.NuovoFileScheda.FileName);
                        schedaDaAggiornare.NomeFileArchiviato = nomeFileUnico;
                        schedaDaAggiornare.ContentType = viewModel.NuovoFileScheda.ContentType;
                    }

                    schedaDaAggiornare.Nome = viewModel.Scheda.Nome;
                    schedaDaAggiornare.Descrizione = viewModel.Scheda.Descrizione;
                    schedaDaAggiornare.DataInizio = viewModel.Scheda.DataInizio;
                    schedaDaAggiornare.DataFine = viewModel.Scheda.DataFine;
                    schedaDaAggiornare.AtletaId = viewModel.Scheda.AtletaId;

                    await serviziScheda.AggiornaSchedaAsync(schedaDaAggiornare);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedaExists(viewModel.Scheda.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewModel.Scheda.Id });
            }

            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            return View(viewModel);
        }

        // GET: Scheda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await serviziScheda.DaiSchedaAsync(id); 
            if (scheda == null)
            {
                return NotFound();
            }
            SchedaAllenamentoViewModel vm = new SchedaAllenamentoViewModel();
            vm.Scheda = scheda;
            vm.Stato = serviziScheda.CalcolaStatoScheda(scheda);
            return View(vm);
        }

        // POST: Scheda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var scheda = await serviziScheda.DaiSchedaAsync(id);
            if (scheda != null)
            {
                // --- Logica di cancellazione file ---
                if (!string.IsNullOrEmpty(scheda.NomeFileArchiviato))
                {
                    string percorsoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede", scheda.NomeFileArchiviato);
                    if (System.IO.File.Exists(percorsoFile))
                    {
                        System.IO.File.Delete(percorsoFile);
                    }
                }
                await serviziScheda.EliminaSchedaAsync(id.Value);
                return RedirectToAction("Details", "Atleta", new { id = scheda.AtletaId });
            }
            
            // --- Fine logica cancellazione file ---

            return RedirectToAction(nameof(Index));
        }

        private bool SchedaExists(int id)
        {
            return serviziScheda.DaiScheda(id) != null;
        }
        public async Task<IActionResult> Visualizza(int id)
        {
            var scheda = await serviziScheda.DaiSchedaAsync(id);

            if (scheda == null || string.IsNullOrEmpty(scheda.NomeFileArchiviato))
            {
                return NotFound();
            }

            string percorsoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede", scheda.NomeFileArchiviato);

            if (!System.IO.File.Exists(percorsoFile))
            {
                return NotFound();
            }

            // Restituisce il file al browser. Il browser deciderà se mostrarlo o scaricarlo
            // in base al ContentType. Il NomeFileOriginale viene usato per il nome del download.
            return PhysicalFile(percorsoFile, scheda.ContentType, scheda.NomeFileOriginale);
        }
    }
}
