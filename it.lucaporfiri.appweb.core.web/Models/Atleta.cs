using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Atleta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Il campo Nome è obbligatorio.")]
        public required string Nome { get; set; }
        [Required(ErrorMessage = "Il campo Cognome è obbligatorio.")]
        public required string Cognome { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AnnoDiNascita { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DataInizioIscrizione { get; set; }
        public TipoCliente Tipo { get; set; }
        public StatoCliente Stato { get; set; }
        public virtual ICollection<Abbonamento>? Abbonamenti { get; set; }
        public virtual ICollection<Scheda>? Schede { get; set; }

        public enum TipoCliente
        {
            Personal,
            Corso,
            SolaScheda
        }
        public enum StatoCliente
        {
            Attivo,
            NonAttivo,
            NonDefinito
        }

    }
}
