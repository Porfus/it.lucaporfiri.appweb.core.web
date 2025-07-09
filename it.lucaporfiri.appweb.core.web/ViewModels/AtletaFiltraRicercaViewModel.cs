using static it.lucaporfiri.appweb.core.web.Models.Atleta;
using static it.lucaporfiri.appweb.core.web.ViewModels.AtletaDetailViewModel;
using static it.lucaporfiri.appweb.core.web.ViewModels.SchedaAllenamentoViewModel;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AtletaFiltraRicercaViewModel
    {
        public int Id { get; set; }
        public string? NomeCompleto { get; set; }
        public TipoCliente Tipo { get; set; }
        public StatoAbbonamento StatoAbbonamento { get; set; }
        public StatoScheda StatoScheda { get; set; }


    }
}
