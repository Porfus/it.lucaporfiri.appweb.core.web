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
        
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public TipoCliente Tipo { get; set; }
        public StatoCliente Stato { get; set; }
        public virtual ICollection<Abbonamento> Abbonamenti { get; set; } = new List<Abbonamento>();
        public virtual ICollection<Scheda> Schede { get; set; } = new List<Scheda>();
        public virtual ICollection<Evento> Eventi { get; set; } = new List<Evento>();

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
