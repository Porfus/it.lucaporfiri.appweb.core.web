using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static it.lucaporfiri.appweb.core.web.ViewModels.SchedaAllenamentoViewModel;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziScheda
    {
        private ContestoApp _context;
        public ServiziScheda(ContestoApp context)
        {
            _context = context;
        }
        public async Task <Scheda?> DaiSchedaAsync(int? IdScheda)
        {
            if (IdScheda == null)
                return null;
            return await _context.Scheda.Include(s => s.Cliente).FirstOrDefaultAsync(s => s.Id == IdScheda);
        }
        public Scheda? DaiScheda(int? IdScheda)
        {
            if (IdScheda == null)
                return null;
            return _context.Scheda.Include(s => s.Cliente).FirstOrDefault(s => s.Id == IdScheda);
        }

        public StatoScheda CalcolaStatoScheda(Scheda scheda)
        {
            var stato = scheda.DataInizio > DateTime.Today
            ? StatoScheda.NonDefinita
            : scheda.DataFine > DateTime.Today ? StatoScheda.Attiva
            : StatoScheda.Scaduta;
            return stato;
        }
        public List<Scheda> DaiSchede()
        {
            var schede = _context.Scheda.Include(s => s.Cliente).ToList(); 
            return schede;
        }
        public async Task CreaSchedaAsync(Scheda scheda)
        {
            _context.Scheda.Add(scheda);
            await _context.SaveChangesAsync();
        }

        public async Task AggiornaSchedaAsync(Scheda scheda)
        {
            _context.Scheda.Update(scheda);
            await _context.SaveChangesAsync();
        }
        public async Task EliminaSchedaAsync(int id)
        {
            var scheda = await _context.Scheda.FindAsync(id);
            if (scheda != null)
            {
                _context.Scheda.Remove(scheda);
            }
            await _context.SaveChangesAsync();
        }

        public int DaiNumeroSchedeScadute()
        {
            return _context.Scheda.Count(s => s.DataFine < DateTime.Now);
        }
    }
}
