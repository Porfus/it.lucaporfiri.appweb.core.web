using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziScheda
    {
        private ContestoApp _context;
        public ServiziScheda(ContestoApp context)
        {
            _context = context;
        }
        public Scheda? DaiScheda(int? IdScheda)
        {
            if (IdScheda == null)
                return null;
            return _context.Scheda.Include(s => s.Cliente).FirstOrDefault(s => s.Id == IdScheda);
        }

        public List<Scheda> DaiSchede()
        {
            var schede = _context.Scheda.Include(s => s.Cliente).ToList(); 
            return schede;
        }
        public async Task AggiungiSchedaAsync(Scheda scheda)
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
