using it.lucaporfiri.appweb.core.web.Models;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class SchedaAllenamentoEditViewModel
    {
        public Scheda Scheda { get; set; } = new Scheda();
        public StatoScheda Stato { get; set; }

        public IFormFile? NuovoFileScheda { get; set; }
        public enum StatoScheda
        {
            Attiva,
            Scaduta,
            NonDefinita
        }
    }
}
