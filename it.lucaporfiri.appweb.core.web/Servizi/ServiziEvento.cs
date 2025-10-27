using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziEvento
    {
        private readonly ContestoApp _context;
        public ServiziEvento(Data.ContestoApp context)
        {
            _context = context;
        }

        public void SincronizzaEventiAutomatici()
        {
            //Gestione della sincronizzazione automatica delle scadenze delle Schede di allenamento

            //recupero tutti gli atleti attivi
            var atletiAttiviId = _context.Atleta
                .Where(a => a.Stato == Atleta.StatoCliente.Attivo)
                .Select(a => a.Id)
                .ToList();

            foreach (var atletaId in atletiAttiviId) 
            {
                var ultimaScheda = _context.Scheda
                    .Where(e => e.AtletaId == atletaId)
                    .OrderByDescending(e => e.DataFine)
                    .FirstOrDefault();

                if (ultimaScheda != null  && ultimaScheda.DataFine <= DateTime.UtcNow.AddDays(7)) 
                {
                    // Controlla se un task di questo tipo esiste già per questo atleta
                    var eventoEsistente = _context.Eventi.FirstOrDefault(e =>
                        e.AtletaId == atletaId &&
                        e.Tipo == Models.Eventi.TipoEvento.ScadenzaScheda &&
                        e.DataScadenza == ultimaScheda.DataFine &&
                        e.Stato != Eventi.StatoWorkflow.Completato);

                    var atleta = _context.Atleta.FirstOrDefault(a => a.Id == atletaId);

                    if (eventoEsistente == null && atleta != null)
                    {
                        var nuovoEvento = new Models.Eventi
                        {
                            AtletaId = atletaId,
                            Tipo = Models.Eventi.TipoEvento.ScadenzaScheda,
                            DataScadenza = ultimaScheda.DataFine,
                            Titolo = $"Scadenza Scheda di Allenamento di {atleta.Nome + atleta.Cognome}",
                            Descrizione = $"La scheda di allenamento n. {ultimaScheda.Id} è scaduta / è in scadenza il {ultimaScheda.DataFine.ToShortDateString()}.",
                            Stato = Eventi.StatoWorkflow.Inbox,
                            Priorita = CalcolaPrioritaIniziale(ultimaScheda.DataFine) // Funzione da creare
                        };
                        _context.Eventi.Add(nuovoEvento);
                    }
                }

            }
        }
        private int CalcolaPrioritaIniziale(DateTime dataScadenzaScheda) {
            return 0;
        }
    }
}
