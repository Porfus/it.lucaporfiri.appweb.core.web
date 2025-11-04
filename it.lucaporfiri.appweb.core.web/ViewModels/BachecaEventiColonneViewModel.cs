namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class BachecaEventiColonnaViewModel
    {
        public string? Titolo { get; set; }
        public string? IdColonna { get; set; }
        public List<BachecaEventiEventoViewModel> Eventi { get; set; }
        public int ConteggioTask => Eventi.Count;
        public BachecaEventiColonnaViewModel()
        {
            Eventi = new List<BachecaEventiEventoViewModel>(); 
        }
    }
}
