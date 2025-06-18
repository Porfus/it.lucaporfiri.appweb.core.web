using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;

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
            return _context.Scheda.FirstOrDefault(s => s.Id == IdScheda);
        }
        public int DaiNumeroSchedeScadute()
        {
            return _context.Scheda.Count(s => s.DataFine < DateTime.Now);
        }
    }
}
