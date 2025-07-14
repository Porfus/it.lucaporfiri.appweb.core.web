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
                Stato = serviziScheda.CalcolaStatoScheda(s)
            }).ToList();

            if (soloScadute == true) {
                vm = vm.Where(s => s.Stato == SchedaAllenamentoViewModel.StatoScheda.Scaduta).ToList();
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
        public IActionResult Create()
        {
            ViewBag.AtletaId = serviziAtleta.DaiSelectListAtleti();
            return View();
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
                var scheda = new Scheda
                {
                    Nome = viewModel.Nome,
                    Descrizione = viewModel.Descrizione,
                    DataInizio = viewModel.DataInizio,
                    DataFine = viewModel.DataFine,
                    AtletaId = viewModel.AtletaId
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

                await serviziScheda.AggiungiSchedaAsync(scheda);
                return RedirectToAction(nameof(Index));
            }
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
            SchedaAllenamentoViewModel vm = new SchedaAllenamentoViewModel();
            vm.Scheda = scheda;
            vm.Stato = serviziScheda.CalcolaStatoScheda(scheda);
            return View(vm);
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
                  await serviziScheda.AggiornaSchedaAsync(scheda);
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
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            SchedaAllenamentoViewModel vm = new SchedaAllenamentoViewModel();
            vm.Scheda = scheda;
            vm.Stato = serviziScheda.CalcolaStatoScheda(scheda);
            return View(vm);
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
            if (scheda == null)
            {
                return NotFound();
            }
            // --- Logica di cancellazione file ---
            if (!string.IsNullOrEmpty(scheda.NomeFileArchiviato))
            {
                string percorsoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "schede", scheda.NomeFileArchiviato);
                if (System.IO.File.Exists(percorsoFile))
                {
                    System.IO.File.Delete(percorsoFile);
                }
            }
            // --- Fine logica cancellazione file ---
            await serviziScheda.EliminaSchedaAsync(id.Value);

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
