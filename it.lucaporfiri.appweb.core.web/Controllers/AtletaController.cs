using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AtletaController : Controller
    {
        private readonly ServiziAtleta serviziAtleta;
        private readonly ServiziAbbonamento serviziAbbonamento;
        private readonly ServiziScheda serviziScheda;
        public AtletaController(ServiziAtleta serviziAtleta, ServiziAbbonamento serviziAbbonamento, ServiziScheda serviziScheda)
        {
            this.serviziAtleta = serviziAtleta;
            this.serviziAbbonamento = serviziAbbonamento;
            this.serviziScheda = serviziScheda;
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

        [HttpPost]
        public IActionResult FiltraRicerca()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();

            string? filtroNome = Request.Form["filtroNome"].FirstOrDefault();

            string? colonnaOrdinamento = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            string? versoOrdinamento = Request.Form["order[0][dir]"].FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            int risultatiTotali = 0;
            IEnumerable<AtletaFiltraRicercaViewModel> atleti = serviziAtleta.Ricerca(skip, pageSize, out risultatiTotali, colonnaOrdinamento, versoOrdinamento, filtroNome);
            return Json(new
            {
                draw,
                recordsFiltered = risultatiTotali,
                recordsTotal = risultatiTotali,
                data = atleti
            });
        }

        // GET: Atleta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var atleta =await serviziAtleta.DaiAtletaAsync(id);
            if (atleta == null)
            {
                return NotFound();
            }
            
            var vm = new AtletaDetailViewModel
            {
                Id = atleta.Id,
                NomeCompleto = $"{atleta.Nome} {atleta.Cognome}",
                Eta = serviziAtleta.CalcolaEta(atleta),
                Email = atleta.Email,
                Telefono = atleta.Telefono,
                DataIscrizioneAtleta = atleta.DataInizioIscrizione.HasValue? atleta.DataInizioIscrizione.Value : DateTime.MinValue,
                Tipo = atleta.Tipo,
                Stato = atleta.Stato,
                StatoUltimoAbbonamento = serviziAtleta.CalcolaStatoAbbonamento(atleta),

                CronologiaAbbonamenti = atleta.Abbonamenti.Select(abbonamentoDb => new AbbonamentoDetailViewModel
                {
                    abbonamento = abbonamentoDb,
                    statoAbbonamento = serviziAbbonamento.CalcolaStatoAbbonamento(abbonamentoDb)
                }).ToList(),

                CronologiaSchede = atleta.Schede.Select(schedaDb => new SchedaAllenamentoViewModel
                {
                    Scheda = schedaDb,
                    Stato = serviziScheda.CalcolaStatoScheda(schedaDb)
                }).ToList()
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
                return RedirectToAction(nameof(Details), new { id = atleta.Id});
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cognome,AnnoDiNascita,Email,Telefono,DataInizioIscrizione,Tipo,Stato")] Atleta atleta)
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
                return RedirectToAction(nameof(Details), new {id = atleta.Id});
            }
            return View(atleta);
        }
        [HttpPost]
        public async Task<IActionResult> AggiornaStato(int id, string newState)
        {
            var atleta = serviziAtleta.DaiAtleta(id);
            if (atleta == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid) 
            {
                if (Enum.TryParse<Atleta.StatoCliente>(newState, out var stato))
                {
                    atleta.Stato = stato;
                    try
                    {
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
                    return RedirectToAction(nameof(Details), new { id });
                }

            }

            return RedirectToAction("Details", new { id });
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
