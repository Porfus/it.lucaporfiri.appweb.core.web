using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using static it.lucaporfiri.appweb.core.web.Models.Evento;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziEvento
    {
        private readonly ContestoApp _context;
        public ServiziEvento(Data.ContestoApp context)
        {
            _context = context;
        }

        public async Task SincronizzaEventiAutomatici()
        {
            //Gestione della sincronizzazione automatica delle scadenze delle Schede di allenamento

            //recupero tutti gli atleti attivi
            var atletiAttiviId = _context.Atleta
                .Where(a => a.Stato == Atleta.StatoCliente.Attivo)
                .Select(a => a.Id)
                .ToList();

            if (!atletiAttiviId.Any())
                return;

            // Recupero l'ultima scheda per ogni atleta (una query con group)
            var ultimeSchede = await _context.Scheda
                .Where(s => atletiAttiviId.Contains(s.AtletaId))
                .GroupBy(s => s.AtletaId)
                .Select(g => g.OrderByDescending(s => s.DataFine).FirstOrDefault())
                .ToListAsync();

            // Precarico eventi esistenti di tipo ScadenzaScheda per questi atleti, per evitare query per ogni atleta
            var eventiEsistenti = await _context.Eventi
                .Where(e => atletiAttiviId.Contains(e.AtletaId ?? 0)
                            && e.Tipo == Evento.TipoEvento.ScadenzaScheda
                            && e.Stato != Evento.StatoWorkflow.Completato)
                .ToListAsync();
            var nuoviEventi = new List<Evento>();

            foreach (var scheda in ultimeSchede.Where(s => s != null))
            {
                if (scheda != null && scheda.DataFine <= DateTime.UtcNow.AddDays(7))
                {
                    var atletaId = scheda.AtletaId;

                    // controlla se esiste già un evento identico in memoria
                    bool esiste = eventiEsistenti.Any(e =>
                        e.AtletaId == atletaId &&
                        e.DataScadenza == scheda.DataFine);

                    if (!esiste)
                    {
                        var atleta = await _context.Atleta.FirstOrDefaultAsync(a => a.Id == atletaId);
                        if (atleta == null)
                            continue; // potrebbe essere stato rimosso tra le query

                        var nuovoEvento = new Evento
                        {
                            AtletaId = atletaId,
                            Tipo = Evento.TipoEvento.ScadenzaScheda,
                            DataScadenza = scheda.DataFine,
                            Titolo = $"Preparazione Nuova Scheda",
                            Descrizione = $"Atleta: {atleta.Nome} {atleta.Cognome}",
                            Stato = Evento.StatoWorkflow.Inbox,
                            Priorita = CalcolaPrioritaIniziale(scheda.DataFine, Evento.TipoEvento.ScadenzaScheda)
                        };
                        nuoviEventi.Add(nuovoEvento);
                    }
                }
            }

            if (nuoviEventi.Any())
            {
                _context.Eventi.AddRange(nuoviEventi);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    // Log e rilancio con inner preserved
                    throw new InvalidOperationException("Errore durante la sincronizzazione degli eventi.", dbEx);
                }
            }
        }

        //Calcolo della priorità iniziale in base alla data di scadenza e al tipo di evento
        private int CalcolaPrioritaIniziale(DateTime dataScadenzaScheda, TipoEvento tipoEvento)
        {
            int PriorityScore = 0;
            int pesoEvento = 0;
            int fattoreUrgenza = 0;
            int giorniRimanenti = 0;
            switch (tipoEvento)
            {
                case TipoEvento.ScadenzaScheda:
                    pesoEvento = 30; break;

                case TipoEvento.GaraDaPreparare:
                    pesoEvento = 100; break;

                case TipoEvento.AtletaDaContattare:
                    pesoEvento = 50; break;

                case TipoEvento.AllenamentoPersonal:
                    pesoEvento = 40; break;

                default:
                    pesoEvento = 10; break;
            }

            giorniRimanenti = (int)(dataScadenzaScheda - DateTime.UtcNow).TotalDays;

            fattoreUrgenza = 1 / Math.Max(giorniRimanenti, 1);

            PriorityScore = pesoEvento * fattoreUrgenza;
            return PriorityScore;
        }

        public List<Evento> GetEventiAttivi()
        {
            return _context.Eventi
                .Where(e => e.Stato != Evento.StatoWorkflow.Completato)
                .ToList();
        }

        public void PrioritizzaEventi(List<Evento> eventiAttivi)
        {
            foreach (var evento in eventiAttivi)
            {
                int prioritaCalcolata = CalcolaPrioritaIniziale(evento.DataScadenza, evento.Tipo);
                evento.Priorita = prioritaCalcolata;
            }
        }

        public string GetCssClassPerPriorita(int pesoPriorita)
        {
            var prioritacss = "";
            if (pesoPriorita < 20)
            {
                prioritacss = "priority-low";
            }
            else if (pesoPriorita >= 20 && pesoPriorita < 70)
            {
                prioritacss = "priority-medium";
            }
            else if (pesoPriorita >= 70)
            {
                prioritacss = "priority-high";
            }
            return prioritacss;
        }

        public string FormattaDataScadenza(DateTime dataScadenza)
        {
            var giorniRimanenti = (dataScadenza - DateTime.Now).TotalDays;
            if (giorniRimanenti > 7)
            {
                return $"Entro: {dataScadenza.ToString("dd MMM yyyy")}";
            }
            else if (giorniRimanenti >= 1 && giorniRimanenti <= 7)
            {
                return $"Scade tra {Math.Ceiling(giorniRimanenti)} giorni";
            }
            else if (giorniRimanenti >= 0 && giorniRimanenti < 1)
            {
                return "Scade Oggi";
            }
            else
            {
                return $"Scaduto {Math.Abs(Math.Ceiling(giorniRimanenti))} giorni fa";
            }
        }

        public string GetIconaPerTipoEvento(TipoEvento tipo)
        {
            var icona = "";
            switch (tipo)
            {
                case TipoEvento.ScadenzaScheda:
                    icona = "file-text"; break;
                case TipoEvento.GaraDaPreparare:
                    icona = "dumbbell"; break;
                case TipoEvento.AtletaDaContattare:
                    icona = "phone"; break;
                case TipoEvento.AllenamentoPersonal:
                    icona = "calendar_xmark"; break;
                default:
                    icona = "note"; break;
            }
            return icona;
        }

        public void AggiornaStatoEvento(int eventoId, int nuovoStato, double nuovaPosizione)
        {
            var evento = _context.Eventi.Where(e => e.Id == eventoId).FirstOrDefault();
            if (evento != null)
            {
                evento.Stato = (StatoWorkflow)nuovoStato;
                evento.Posizione = nuovaPosizione;
                _context.SaveChanges();
            }
        }

        public string GetTitoloPerColonna(StatoWorkflow stato)
        {
            String titoloStato;
            if (stato.Equals(StatoWorkflow.DaFare))
            {
                titoloStato = "Da Fare In Settimana";
            }
            else if (stato.Equals(StatoWorkflow.InCorso))
            {
                titoloStato = "In Corso / Oggi";
            }
            else if (stato.Equals(StatoWorkflow.DaValutare))
            {
                titoloStato = "Da Valutare";
            }
            else
            {
                titoloStato = stato.ToString();
            }
            return titoloStato;
        }

        public async Task CreaEventoAsync(Evento nuovoEvento)
        {
            if (nuovoEvento == null) throw new ArgumentNullException(nameof(nuovoEvento));

            // Validazione minima lato servizio (consigliato usare DTO + ModelValidator)
            try
            {
                // se vuoi usare DataAnnotations direttamente sull'entity:
                ModelValidatorExtensions.ValidateObject(nuovoEvento);
            }
            catch (ValidationException vex)
            {
                throw new InvalidOperationException("Dati evento non validi: " + vex.Message, vex);
            }

            // Se è stato specificato un AtletaId, verificarne l'esistenza
            if (nuovoEvento.AtletaId.HasValue)
            {
                bool atletaEsiste = await _context.Atleta.AnyAsync(a => a.Id == nuovoEvento.AtletaId.Value);
                if (!atletaEsiste)
                    throw new InvalidOperationException($"Atleta con Id {nuovoEvento.AtletaId.Value} non esiste.");
            }

            try
            {
                _context.Eventi.Add(nuovoEvento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Errore durante la creazione dell'evento: " + dbEx.Message);
            }
        }

        public async Task OrdinaEventiAsync(bool dataScadenza = false)
        {
            List<Evento> eventiAttivi = GetEventiAttivi();
            if (eventiAttivi == null || eventiAttivi.Count == 0)
            {
                return;
            }
            // Prelevo dal contesto gli eventi corrispondenti in modo che siano tracciati
            var ids = eventiAttivi.Select(e => e.Id).ToList();
            var eventiTracciati = await _context.Eventi
                                        .Where(e => ids.Contains(e.Id))
                                        .ToListAsync();

            // Raggruppa per stato
            var raggruppaEventiPerStato = eventiTracciati
                                    .GroupBy(e => e.Stato)
                                    .ToDictionary(g => g.Key, g => g.ToList());

            var statoEventi = (StatoWorkflow[])Enum.GetValues(typeof(StatoWorkflow));

            foreach (var stato in statoEventi)
            {
                if (!raggruppaEventiPerStato.ContainsKey(stato))
                    continue;

                List<Evento> lista = raggruppaEventiPerStato[stato];

                IEnumerable<Evento> ordinata;
                if (dataScadenza)
                {
                    // Ordina per DataScadenza asc (null in fondo) e poi Priorita desc (null -> 0)
                    ordinata = lista
                        .OrderBy(e => e.DataScadenza == default ? DateTime.MaxValue : e.DataScadenza)
                        .ThenByDescending(e => e.Priorita ?? 0);
                }
                else
                {
                    // Ordina per Priorita desc (null -> 0) e tie-breaker DataScadenza asc
                    ordinata = lista
                        .OrderBy(e => e.Priorita ?? int.MaxValue)
                        .ThenBy(e => e.DataScadenza == default ? DateTime.MaxValue : e.DataScadenza);
                }

                int index = 1;
                foreach (var evento in ordinata)
                {
                    evento.Posizione = index++;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il salvataggio delle modifiche: {ex.Message}");
            }
        }
        public void InizializzaPosizioniMancanti()
        {
            // Prendi tutti gli eventi che non hanno una posizione
            var eventiDaOrdinare = _context.Eventi.Where(e => e.Posizione == null).ToList(); // o null se il double è nullable

            if (eventiDaOrdinare.Any())
            {
                // Trova la posizione massima attualmente esistente per dare continuità
                double maxPosizione = (double)(_context.Eventi.Any() ? _context.Eventi.Max(e => e.Posizione) ?? 0 : 0);

                // Ordina i nuovi eventi secondo la tua logica di default (priorità, scadenza)
                var eventiOrdinatiDiDefault = eventiDaOrdinare.OrderByDescending(e => e.Priorita)
                                                              .ThenBy(e => e.DataScadenza);

                // Assegna loro una posizione progressiva e salva
                double i = 1.0;
                foreach (var evento in eventiOrdinatiDiDefault)
                {
                    evento.Posizione = maxPosizione + i;
                    i++;
                }

                _context.SaveChanges();
            }
        }
    }
}
