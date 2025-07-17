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
        private  readonly ServiziScheda serviziScheda;

        public HomeController(ILogger<HomeController> logger, ServiziAtleta serviziAtleta, ServiziAbbonamento serviziAbbonamento, ServiziScheda serviziScheda)
        {
            _logger = logger;
            this.serviziAtleta = serviziAtleta;
            this.serviziAbbonamento = serviziAbbonamento;
            this.serviziScheda = serviziScheda;
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
            vm.SchedeScadute = serviziScheda.DaiNumeroSchedeScadute();

            // Popolamento alert dinamici
            var atleti = serviziAtleta.DaiAtleti();
            foreach (var atleta in atleti)
            {
                // Controllo abbonamento
                var statoAbbonamento = serviziAtleta.CalcolaStatoUltimoAbbonamento(atleta);
                var schedaScaduta = atleta.Schede?.Any(s => serviziScheda.CalcolaStatoScheda(s) == StatoScheda.Scaduta) ?? false;

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
            vm.SchedeScadute = serviziAtleta.DaiNumeroAtletiConSchedeScadute();

            var atleti = serviziAtleta.DaiAtleti();
            foreach (var atleta in atleti)
            {
                var abbonamentoScaduto = serviziAtleta.CalcolaStatoAbbonamento(atleta) == StatoAbbonamento.Scaduto ? true : false;
                var schedaScaduta = serviziAtleta.CalcolaStatoScheda(atleta) == StatoScheda.Scaduta ? true : false;
                bool abbonamentoInScadenza = false;
                bool schedaInScadenza = false;
                if (abbonamentoScaduto == false) 
                {
                    var ultimoAbbonamento = serviziAtleta.DaiUltimoAbbonamentoAttivo(atleta);
                    if (ultimoAbbonamento != null && ultimoAbbonamento.DataFine < DateTime.Now.AddDays(6))
                    {
                        abbonamentoInScadenza = true;
                    }
                }
                if(schedaScaduta == false)
                {
                    var ultimaScheda = serviziAtleta.DaiUltimaSchedaAttiva(atleta);
                    if (ultimaScheda != null && ultimaScheda.DataFine < DateTime.Now.AddDays(6))
                    {
                        schedaInScadenza = true;
                    }
                }
                if (abbonamentoScaduto || schedaScaduta || schedaInScadenza || abbonamentoInScadenza)
                {
                    var tipoAlert = "";
                    if (abbonamentoScaduto && schedaScaduta)
                        tipoAlert = "critical";
                    else if (abbonamentoScaduto)
                        tipoAlert = "warning";
                    else if (schedaScaduta)
                        tipoAlert = "warning-giallo";
                    else if (abbonamentoInScadenza || schedaInScadenza)
                        tipoAlert = "info";

                    var dettagli = "";
                    if (atleta.Tipo == Atleta.TipoCliente.Personal)
                        dettagli += "Personal";
                    else if (atleta.Tipo == Atleta.TipoCliente.Corso)
                        dettagli += "Corso";
                    else if (atleta.Tipo == Atleta.TipoCliente.SolaScheda)
                        dettagli += "Solo Scheda";

                    vm.AlertAtleti.Add(new AlertAtletaViewModel
                    {
                        IdCliente = atleta.Id,
                        Nome = $"{atleta.Nome} {atleta.Cognome}",
                        Iniziali = $"{atleta.Nome?[0]}{atleta.Cognome?[0]}".ToUpper(),
                        Dettagli = dettagli,
                        Tipo = tipoAlert,
                        AbbonamentoScaduto = abbonamentoScaduto,
                        SchedaScaduta = schedaScaduta,
                        AbbonamentoInScadenza = abbonamentoInScadenza,
                        SchedaInScadenza= schedaInScadenza
                    });
                }
            }
            return View(vm);
        }

        public IActionResult Login()
        {
            return View();
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
