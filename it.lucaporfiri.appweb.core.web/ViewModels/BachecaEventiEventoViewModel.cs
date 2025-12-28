namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class BachecaEventiEventoViewModel
    {
        public int Id { get; set; }
        public string? Titolo { get; set; }
        public string? Descrizione { get; set; } // Es. "Atleta: Mario Rossi"
        public string? PrioritaCssClass { get; set; } // Es. "priority-high"
        public bool IsUrgente { get; set; }
        public string? DataScadenzaLabel { get; set; } // Es. "Scade: Domani"
        public string? IconaTipoTask { get; set; } // Es. "event_busy"
        public bool IsCompletato { get; set; }
        public double Posizione { get; set; }
        public double PrioritaScore { get; set; }
    }
}
