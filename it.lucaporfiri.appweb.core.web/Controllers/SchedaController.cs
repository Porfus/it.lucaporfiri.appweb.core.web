using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class SchedaController : Controller
    {
        //private readonly ContestoApp _context;
        private readonly ServiziScheda serviziScheda;
        private readonly ServiziAtleta serviziAtleta;

        public SchedaController(/*ContestoApp context,*/ServiziScheda serviziScheda, ServiziAtleta serviziAtleta)
        {
            //_context = context;
            this.serviziScheda = serviziScheda;
            this.serviziAtleta = serviziAtleta;
        }

        // GET: Scheda
        public async Task<IActionResult> Index()
        {
            // Utilizzo di Task.Run per eseguire l'elaborazione in un thread in background
            var schede = await Task.Run(() => serviziScheda.DaiSchede());
            return View(schede);
        }

        // GET: Scheda/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var scheda = await _context.Scheda
            //    .Include(s => s.Cliente)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var scheda = await Task.Run(() =>serviziScheda.DaiScheda(id));
            if (scheda == null)
            {
                return NotFound();
            }
            ViewData["Cliente"] = serviziAtleta.DaiSelectListAtleti();
            return View(scheda);
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
        public async Task<IActionResult> Create([Bind("Id,Nome,Descrizione,DataInizio,DataFine,AtletaId")] Scheda scheda)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(scheda);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["AtletaId"] = new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);
            //return View(scheda);
            if (ModelState.IsValid)
            {
                await serviziScheda.AggiungiSchedaAsync(scheda);
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();
            return View(scheda);
        }

        // GET: Scheda/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await Task.Run(() => serviziScheda.DaiScheda(id)); /*await _context.Scheda.FindAsync(id);*/
            if (scheda == null)
            {
                return NotFound();
            }
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti(); /*new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);*/
            return View(scheda);
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
                    //_context.Update(scheda);
                    //await _context.SaveChangesAsync();
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
            ViewData["AtletaId"] = serviziAtleta.DaiSelectListAtleti();/*new SelectList(_context.Atleta, "Id", "Id", scheda.AtletaId);*/ 
            return View(scheda);
        }

        // GET: Scheda/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheda = await Task.Run(() => serviziScheda.DaiScheda(id)); // Ensure this is awaited and not null.
            if (scheda == null)
            {
                return NotFound();
            }

            return View(scheda);
        }

        // POST: Scheda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await serviziScheda.EliminaSchedaAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool SchedaExists(int id)
        {
            return serviziScheda.DaiScheda(id) != null;
                /*_context.Scheda.Any(e => e.Id == id)*/
        }
    }
}
