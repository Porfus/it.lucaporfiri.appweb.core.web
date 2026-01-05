using System.ComponentModel.DataAnnotations;
using static it.lucaporfiri.appweb.core.web.Models.Atleta;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AtletaCreateViewModel
    {

        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public required string Nome { get; set; }
        [Required(ErrorMessage = "Il campo Cognome è obbligatorio.")]
        public required string Cognome { get; set; }
        public DateTime? AnnoDiNascita { get; set; }

        [Required(ErrorMessage = "Il campo Email è obbligatorio.")]
        public required string Email { get; set; }
        public string? Telefono { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DataInizioIscrizione { get; set; }
        public TipoCliente Tipo { get; set; }
        public StatoCliente Stato { get; set; } 
    }
}
