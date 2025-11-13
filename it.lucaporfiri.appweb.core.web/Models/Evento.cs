namespace it.lucaporfiri.appweb.core.web.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string? Titolo { get; set; }
        public string? Descrizione { get; set; }
        public DateTime DataScadenza { get; set; }
        
        public int? Priorita { get; set; }

        public int? OrdineInTipoEvento { get; set; }

        public int AtletaId { get; set; }
        public virtual Atleta? Atleta { get; set; }
        public StatoWorkflow Stato { get; set; }
        public TipoEvento Tipo { get; set; }

        public enum TipoEvento
        {
            ScadenzaScheda,
            GaraDaPreparare,
            AtletaDaContattare,
            AllenamentoPersonal,
            Altro
        }
        public enum StatoWorkflow
        {
            Inbox,
            DaFare,
            InCorso,
            DaValutare,
            Completato
        }     
    }
}
