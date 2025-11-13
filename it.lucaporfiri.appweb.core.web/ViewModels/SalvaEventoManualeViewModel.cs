using it.lucaporfiri.appweb.core.web.Models;
using static it.lucaporfiri.appweb.core.web.Models.Evento;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class SalvaEventoManualeViewModel
    {
        public TipoEvento titolo { get; set; }

        public DateTime dataScadenza { get; set; }

        public Atleta? atleta { get; set; }

        public String? descrizione { get; set; } 
    }
}
