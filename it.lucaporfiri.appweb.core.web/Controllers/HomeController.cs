using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public IActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel();
            vm.AtletiTotale = serviziAtleta.DaiAtleti().Count();
            vm.AtletiAttivi = serviziAtleta.DaiAtleti().Count(s=> s.Stato == Atleta.StatoCliente.Attivo);
            vm.AbbonamentiScaduti = serviziAtleta.DaiNumeroAtletiConAbbonamentiScaduti();
            vm.SchedeScadute = ServiziScheda.DaiNumeroSchedeScadute();

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
