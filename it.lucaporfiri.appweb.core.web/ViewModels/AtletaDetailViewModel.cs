using it.lucaporfiri.appweb.core.web.Models;
using System.ComponentModel.DataAnnotations;
using static it.lucaporfiri.appweb.core.web.Models.Atleta;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AtletaDetailViewModel
    {
        public int Id { get; set; } 
        public string? NomeCompleto { get; set; }
        public int? Eta { get; set; } // Età calcolata in base alla data di nascita
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataIscrizioneAtleta { get; set; }
        public TipoCliente Tipo { get; set; } // Tipo di atleta (Personal, Corso, Sola Scheda)
        public StatoCliente Stato { get; set; } 
        public StatoAbbonamento StatoUltimoAbbonamento { get; set; } // La nostra proprietà calcolata!
        public ICollection<AbbonamentoDetailViewModel> CronologiaAbbonamenti { get; set; } = new List<AbbonamentoDetailViewModel>();// Lista degli abbonamenti dell'atleta
        public ICollection<SchedaAllenamentoViewModel> CronologiaSchede { get; set; } = new List<SchedaAllenamentoViewModel>(); // Lista delle schede dell'atleta
        public enum StatoAbbonamento
        {
            Valido,
            Scaduto,
            NonDefinito
        }
    }
}
