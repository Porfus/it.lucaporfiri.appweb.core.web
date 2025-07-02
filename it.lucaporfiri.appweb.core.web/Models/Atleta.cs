using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Atleta
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }

        [DataType(DataType.Date)]
        public DateTime AnnoDiNascita { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataInizioIscrizione { get; set; }
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
