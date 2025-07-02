using it.lucaporfiri.appweb.core.web.Models;
using System.ComponentModel.DataAnnotations;
using static it.lucaporfiri.appweb.core.web.Models.Atleta;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AtletaDetailViewModel
    {
        public int Id { get; set; } 
        public string? NomeCompleto { get; set; }
        public int Eta { get; set; } // Età calcolata in base alla data di nascita
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataIscrizioneAtleta { get; set; }
        public TipoCliente Tipo { get; set; } // Tipo di atleta (Personal, Corso, Sola Scheda)
        public StatoCliente Stato { get; set; } // Tipo di atleta (Personal, Corso, Sola Scheda)
        public StatoAbbonamento Abbonamento { get; set; } // La nostra proprietà calcolata!
        public ICollection<Abbonamento>? Abbonamenti { get; set; } // Lista degli abbonamenti dell'atleta
        public ICollection<Scheda>? Schede { get; set; } // Lista delle schede dell'atleta
        public enum StatoAbbonamento
        {
            Valido,
            Scaduto,
            NonDefinito
        }
    }
}
