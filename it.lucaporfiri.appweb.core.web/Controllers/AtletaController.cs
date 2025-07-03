using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AtletaController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //private readonly ContestoApp _context;
        private readonly ServiziAtleta serviziAtleta;
        
        public AtletaController(/*ContestoApp _context,*/ ServiziAtleta serviziAtleta1)
        {
            //this._context = _context;
            //this._logger = logger;
            serviziAtleta = serviziAtleta1; 
        }
        // GET: Abbonamento
        public async Task<IActionResult> Index(bool soloAttivi=false)
        {
            var atleti = await serviziAtleta.GetAllAtletiAsync();
            if (soloAttivi)
            {
                atleti = atleti.Where(a => a.Stato == Atleta.StatoCliente.Attivo).ToList();
            }
            return View(atleti);
        }
        // GET: Atleta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var atleta =await Task.Run(() =>serviziAtleta.DaiAtleta(id));
            // _context.Atleta.Include(a => a.Abbonamenti)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            if (atleta == null)
            {
                return NotFound();
            }

            // Create the ViewModel
            var vm = new AtletaDetailViewModel
            {
                Id = atleta.Id,
                NomeCompleto = $"{atleta.Nome} {atleta.Cognome}",
                Eta = serviziAtleta.CalcolaEta(atleta).Equals(0)? serviziAtleta.CalcolaEta(atleta): null,
                Email = atleta.Email,
                Telefono = atleta.Telefono,
                DataIscrizioneAtleta = atleta.DataInizioIscrizione.HasValue? atleta.DataInizioIscrizione.Value : DateTime.MinValue,
                Tipo = atleta.Tipo,
                Stato = atleta.Stato,
                Abbonamento = serviziAtleta.CalcolaStatoAbbonamento(atleta),
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
                //_context.Add(atleta);
                //await _context.SaveChangesAsync();
                await serviziAtleta.CreaAtleta(atleta);
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

            var atleta = await Task.Run(() => serviziAtleta.DaiAtleta(id)); /*_context.Atleta.FindAsync(id);*/
            if (atleta == null)
            {
                return NotFound();
            }
            ViewBag.Tipo = new SelectList(serviziAtleta.DaiTipiAtleta(), "Value", "Text", atleta.Tipo);
            ViewBag.Stato = new SelectList(serviziAtleta.DaiStatiAtleta(), "Value", "Text", atleta.Stato);
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
                    //_context.Update(atleta);
                    //await _context.SaveChangesAsync();
                    await serviziAtleta.ModificaAtleta(atleta);
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
            var atleta = await Task.Run(()=>serviziAtleta.DaiAtleta(id));
            // _context.Atleta
            //    .FirstOrDefaultAsync(m => m.Id == id);
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
            //var atleta = await _context.Atleta.FindAsync(id);
            //if (atleta != null)
            //{
            //    _context.Atleta.Remove(atleta);
            //}

            //await _context.SaveChangesAsync();
            await serviziAtleta.EliminaAtleta(id);
            return RedirectToAction(nameof(Index));
        }

        private bool AtletaExists(int id)
        {
            return serviziAtleta.DaiAtleta(id)!=null;
        }

    }
}
