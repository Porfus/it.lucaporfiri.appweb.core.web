using it.lucaporfiri.appweb.core.web.Models;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class SchedaAllenamentoViewModel
    {
        public Scheda Scheda { get; set; } = new Scheda();

        public StatoScheda Stato { get; set; }

        public enum StatoScheda
        {
            Attiva,
            Scaduta,
            NonDefinita
        }
    }
}
