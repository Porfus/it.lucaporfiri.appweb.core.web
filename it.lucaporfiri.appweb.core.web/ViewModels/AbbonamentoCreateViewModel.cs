namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AbbonamentoCreateViewModel
    {
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        //Chiave esterna per collegarlo al Cliente
        public int AtletaId { get; set; }
    }
}
