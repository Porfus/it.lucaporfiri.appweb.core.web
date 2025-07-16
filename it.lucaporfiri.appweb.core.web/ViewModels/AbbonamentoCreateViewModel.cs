using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AbbonamentoCreateViewModel
    {
        public string? NomeAbbonamento { get; set; }
        public DateTime DataInizio { get; set; } = DateTime.Today;
        public DateTime DataFine { get; set; }

        [Required(ErrorMessage = "Devi selezionare un atleta.")]
        public int AtletaId { get; set; }
    }
}
