using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class SchedaCreateViewModel
    {
        public int Id { get; set; } // Utile per la modifica
        public string? Nome { get; set; }
        public string? Descrizione { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime DataFine { get; set; } = DateTime.Today.AddMonths(1); // Valore di default

        [Required(ErrorMessage = "Devi selezionare un atleta.")]
        public int AtletaId { get; set; }

        // Proprietà per ricevere il file dal form HTML
        public IFormFile? FileCaricato { get; set; }
    }
}
