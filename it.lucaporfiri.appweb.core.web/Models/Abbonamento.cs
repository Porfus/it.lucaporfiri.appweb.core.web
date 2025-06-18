namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Abbonamento
    {
        public int Id { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }

        //Chiave esterna per collegarlo al Cliente
        public int AtletaId { get; set; }
        public virtual required Atleta Atleta { get; set; } // Proprietà di navigazione

    }
}