namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class AlertAtletaViewModel
    {
        public required int IdCliente { get; set; }
        public required string Nome { get; set; }
        public required string Iniziali { get; set; }
        public required string Dettagli { get; set; }
        public required string Tipo { get; set; } // "critical", "warning", "info"
    }
}
