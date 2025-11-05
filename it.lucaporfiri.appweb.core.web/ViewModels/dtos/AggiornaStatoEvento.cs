using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.ViewModels.dtos
{
    public class AggiornaStatoEvento
    {
        [Required]
        public int EventoId { get; set; }
       
        public int NuovoStato { get; set; }
    }
}
