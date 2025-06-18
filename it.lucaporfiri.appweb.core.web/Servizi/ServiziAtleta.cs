using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Mvc;
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
        // Il service può calcolare lo stato
        public StatoAbbonamento CalcolaStatoAbbonamento(Atleta atleta)
        {
            if (atleta.Abbonamenti == null || !atleta.Abbonamenti.Any())
            {
                return StatoAbbonamento.NonDefinito;
            }

            bool hasAbbonamentoAttivo = atleta.Abbonamenti
                .Any(a => a.DataInizio <= DateTime.Now && a.DataFine >= DateTime.Now);

            return hasAbbonamentoAttivo ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }
        public int CalcolaEta(Atleta atleta)
        {
            return DateTime.Now.Year - atleta.AnnoDiNascita.Year -
                   (DateTime.Now.DayOfYear < atleta.AnnoDiNascita.DayOfYear ? 1 : 0);
        }
    }
}
