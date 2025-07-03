using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Migrations;
using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.EntityFrameworkCore;
using static it.lucaporfiri.appweb.core.web.ViewModels.AbbonamentoDetailViewModel;
using static it.lucaporfiri.appweb.core.web.ViewModels.AtletaDetailViewModel;
namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziAbbonamento
    {
        private ContestoApp _context;
        public ServiziAbbonamento(ContestoApp context)
        {
            _context = context;
        }
        public Abbonamento? DaiAbbonamento(int? IdAbbonamento) 
        {
            if (IdAbbonamento == null) 
               return null;
            return _context.Abbonamento.FirstOrDefault(s => s.Id == IdAbbonamento);
        }
        public async Task<Abbonamento?> DaiAbbonamentoAsync(int? idAbbonamento)
        {
            if (idAbbonamento == null)
                return null;

            return await _context.Abbonamento.Include(a => a.Atleta).FirstOrDefaultAsync(s => s.Id == idAbbonamento);
        }
        public async Task<List<Abbonamento>> GetAbbonamentiAsync()
        {
             return await _context.Abbonamento.Include(a => a.Atleta).ToListAsync();  
        }
        public async Task CreaAbbonamento(Abbonamento abbonamento)
        {
            _context.Abbonamento.Add(abbonamento);
            await _context.SaveChangesAsync();
        }

        public async Task ModificaAbbonamento(Abbonamento abbonamento)
        {
            var entity = await _context.Abbonamento.FindAsync(abbonamento.Id);
            if (entity == null) return;
            entity.DataInizio = abbonamento.DataInizio;
            entity.DataFine = abbonamento.DataFine;
            entity.AtletaId = abbonamento.AtletaId;
            await _context.SaveChangesAsync();
        }

        public async Task EliminaAbbonamento(int id)
        {
            var abbonamento = await _context.Abbonamento.FindAsync(id);
            if (abbonamento != null)
            {
                _context.Abbonamento.Remove(abbonamento);
            }
            await _context.SaveChangesAsync();
        }
        public StatoAbbonamento CalcolaStatoAbbonamento(Abbonamento abbonamento)
        {
            if (abbonamento == null)
            {
                return StatoAbbonamento.NonDefinito;
            }
            var statoAbbonamento = abbonamento.DataInizio <= DateTime.Now && abbonamento.DataFine >= DateTime.Now;
            return statoAbbonamento ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }

    }
}
