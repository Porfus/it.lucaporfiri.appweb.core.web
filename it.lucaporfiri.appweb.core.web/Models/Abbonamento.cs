using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Abbonamento
    {
        public int Id { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        //Chiave esterna per collegarlo al Cliente
        [Required(ErrorMessage = "Il campo Atleta è obbligatorio.")]
        public int AtletaId { get; set; }
        public virtual Atleta? Atleta { get; set; }  // Proprietà di navigazione
    }
}