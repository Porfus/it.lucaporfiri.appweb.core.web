using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static it.lucaporfiri.appweb.core.web.ViewModels.AtletaDetailViewModel;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziAtleta
    {
        private ContestoApp _context;
        public ServiziAtleta(ContestoApp context)
        {
            _context = context;
        }
        public Atleta? DaiAtleta(int? IdAtleta)
        {
            if (IdAtleta == null)
                return null;
            return _context.Atleta.FirstOrDefault(s => s.Id == IdAtleta);
        }
        public async Task<List<Atleta>> GetAllAtletiAsync()
        {
            return await _context.Atleta.ToListAsync();
        }
        public async Task<List<Atleta>> GetAllAtletiConAbbonamentiAsync()
        {
            return await _context.Atleta.Include(a => a.Abbonamenti).ToListAsync();
        }
        public StatoAbbonamento CalcolaStatoUltimoAbbonamento(Atleta atleta)
        {
            if (atleta.Abbonamenti == null || !atleta.Abbonamenti.Any())
            {
                return StatoAbbonamento.NonDefinito;
            }
            var abbonamento = atleta.Abbonamenti
                .OrderByDescending(a => a.DataFine)
                .FirstOrDefault();

            if (abbonamento == null)
                return StatoAbbonamento.NonDefinito;

            bool attivo = abbonamento.DataInizio <= DateTime.Now && abbonamento.DataFine >= DateTime.Now;
            return attivo ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }
        public StatoAbbonamento CalcolaStatoAbbonamento(Atleta atleta) 
        {
            if (atleta.Abbonamenti == null || !atleta.Abbonamenti.Any())
            {
                return StatoAbbonamento.NonDefinito;
            }
            var statoAbbonamento = atleta.Abbonamenti.Any(s => s.DataInizio <= DateTime.Now && s.DataFine>= DateTime.Now);
            return statoAbbonamento ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }
        public int CalcolaEta(Atleta atleta)
        {
            return DateTime.Now.Year - atleta.AnnoDiNascita.Year -
                   (DateTime.Now.DayOfYear < atleta.AnnoDiNascita.DayOfYear ? 1 : 0);
        }
        public ICollection<Atleta> DaiAtleti()
        {
            return _context.Atleta.ToList();
        }
        public int DaiNumeroAtletiConAbbonamentiScaduti()
        {
            return _context.Atleta.Count(a =>
                a.Abbonamenti != null && !a.Abbonamenti.Any(ab => ab.DataInizio <= DateTime.Now && ab.DataFine >= DateTime.Now)
            );
        }
    }
}
