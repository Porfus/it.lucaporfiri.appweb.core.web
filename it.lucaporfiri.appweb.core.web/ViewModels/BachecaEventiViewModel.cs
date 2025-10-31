using it.lucaporfiri.appweb.core.web.Models;
using static it.lucaporfiri.appweb.core.web.Models.Eventi;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class BachecaEventiViewModel
    {
        public List<BachecaEventiColonnaViewModel> Colonne { get; set; }
        public BachecaEventiViewModel()
        {
            Colonne = new List<BachecaEventiColonnaViewModel>();
        }
    }


    
}
