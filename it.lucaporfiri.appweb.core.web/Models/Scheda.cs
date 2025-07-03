using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Scheda
    {
        public int Id { get; set; }
        public string? Nome { get; set; } // Es. "Scheda Forza Gennaio", "Massa Estiva"
        public string? Descrizione { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        // Campi per la gestione del file
        public string? NomeFileOriginale { get; set; }
        public string? NomeFileArchiviato { get; set; }
        public string? ContentType { get; set; }
        // Chiave esterna
        [Required(ErrorMessage = "Il campo è obbligatorio.")]
        public int AtletaId { get; set; }
        public virtual Atleta? Cliente { get; set; } // Proprietà di navigazione
    }
}
