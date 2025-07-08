using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using static it.lucaporfiri.appweb.core.web.ViewModels.AtletaDetailViewModel;
using static it.lucaporfiri.appweb.core.web.ViewModels.SchedaAllenamentoViewModel;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ServiziAtleta serviziAtleta;
        private readonly ServiziAbbonamento serviziAbbonamento;
        private  readonly ServiziScheda ServiziScheda;

        public HomeController(ILogger<HomeController> logger, ServiziAtleta serviziAtleta, ServiziAbbonamento serviziAbbonamento, ServiziScheda serviziScheda)
        {
            _logger = logger;
            this.serviziAtleta = serviziAtleta;
            this.serviziAbbonamento = serviziAbbonamento;
            this.ServiziScheda = serviziScheda;
        }

        //DEPRECATO
        public IActionResult IndexOLD()
        {
            DashboardViewModel vm = new DashboardViewModel
            {
                AlertAtleti = new List<AlertAtletaViewModel>() // Ensure AlertAtleti is initialized
            };

            vm.AtletiTotale = serviziAtleta.DaiAtleti().Count();
            vm.AtletiAttivi = serviziAtleta.DaiAtleti().Count(s => s.Stato == Atleta.StatoCliente.Attivo);
            vm.AbbonamentiScaduti = serviziAtleta.DaiNumeroAtletiConAbbonamentiScaduti();
            vm.SchedeScadute = ServiziScheda.DaiNumeroSchedeScadute();

            // Popolamento alert dinamici
            var atleti = serviziAtleta.DaiAtleti();
            foreach (var atleta in atleti)
            {
                // Controllo abbonamento
                var statoAbbonamento = serviziAtleta.CalcolaStatoUltimoAbbonamento(atleta);
                var schedaScaduta = atleta.Schede?.Any(s => ServiziScheda.CalcolaStatoScheda(s) == StatoScheda.Scaduta) ?? false;

                if (statoAbbonamento == StatoAbbonamento.Scaduto || schedaScaduta)
                {
                    var tipo = statoAbbonamento == StatoAbbonamento.Scaduto && schedaScaduta ? "critical"
                              : statoAbbonamento == StatoAbbonamento.Scaduto || schedaScaduta ? "warning"
                              : "";

                    var dettagli = "";
                    if (atleta.Tipo == Atleta.TipoCliente.Personal)
                        dettagli += "Personal";
                    else if (atleta.Tipo == Atleta.TipoCliente.Corso)
                        dettagli += "Corso";
                    else if (atleta.Tipo == Atleta.TipoCliente.SolaScheda)
                        dettagli += "Solo Scheda";

                    if (statoAbbonamento == StatoAbbonamento.Scaduto)
                        dettagli += " • Abbonamento scaduto";
                    if (schedaScaduta)
                        dettagli += " • Scheda scaduta";

                    vm.AlertAtleti.Add(new AlertAtletaViewModel
                    {
                        IdCliente = atleta.Id,
                        Nome = $"{atleta.Nome} {atleta.Cognome}",
                        Iniziali = $"{atleta.Nome?[0]}{atleta.Cognome?[0]}".ToUpper(),
                        Dettagli = dettagli,
                        Tipo = tipo
                    });
                }
            }
            return View(vm);
        }
        public IActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel
            {
                AlertAtleti = new List<AlertAtletaViewModel>() // Ensure AlertAtleti is initialized
            };

            vm.AtletiTotale = serviziAtleta.DaiAtleti().Count();
            vm.AtletiAttivi = serviziAtleta.DaiAtleti().Count(s => s.Stato == Atleta.StatoCliente.Attivo);
            vm.AbbonamentiScaduti = serviziAtleta.DaiNumeroAtletiConAbbonamentiScaduti();
            vm.SchedeScadute = ServiziScheda.DaiNumeroSchedeScadute();

            // Popolamento alert dinamici
            var atleti = serviziAtleta.DaiAtleti();
            foreach (var atleta in atleti)
            {
                // Controllo abbonamento
                var statoAbbonamento = serviziAtleta.CalcolaStatoUltimoAbbonamento(atleta);
                var schedaScaduta = atleta.Schede?.Any(s => ServiziScheda.CalcolaStatoScheda(s) == StatoScheda.Scaduta) ?? false;

                if (statoAbbonamento == StatoAbbonamento.Scaduto || schedaScaduta)
                {
                    var tipo = statoAbbonamento == StatoAbbonamento.Scaduto && schedaScaduta ? "critical"
                              : statoAbbonamento == StatoAbbonamento.Scaduto || schedaScaduta ? "warning"
                              : "";

                    var dettagli = "";
                    if (atleta.Tipo == Atleta.TipoCliente.Personal)
                        dettagli += "Personal";
                    else if (atleta.Tipo == Atleta.TipoCliente.Corso)
                        dettagli += "Corso";
                    else if (atleta.Tipo == Atleta.TipoCliente.SolaScheda)
                        dettagli += "Solo Scheda";

                    if (statoAbbonamento == StatoAbbonamento.Scaduto)
                        dettagli += " • Abbonamento scaduto";
                    if (schedaScaduta)
                        dettagli += " • Scheda scaduta";

                    vm.AlertAtleti.Add(new AlertAtletaViewModel
                    {
                        IdCliente = atleta.Id,
                        Nome = $"{atleta.Nome} {atleta.Cognome}",
                        Iniziali = $"{atleta.Nome?[0]}{atleta.Cognome?[0]}".ToUpper(),
                        Dettagli = dettagli,
                        Tipo = tipo
                    });
                }
            }
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
