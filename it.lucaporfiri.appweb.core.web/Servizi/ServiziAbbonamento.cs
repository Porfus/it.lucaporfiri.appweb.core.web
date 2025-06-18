using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Migrations;
using it.lucaporfiri.appweb.core.web.Models;

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
    }
}
