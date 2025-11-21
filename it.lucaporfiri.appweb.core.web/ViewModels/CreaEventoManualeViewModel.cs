using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static it.lucaporfiri.appweb.core.web.Models.Evento;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class CreaEventoManualeViewModel
    {
        //OLD selezione atleta nella creazione evento
        public int? AtletaSelezionatoId { get; set; }

        public List<SelectListItem> OpzioniAtleti = new List<SelectListItem>();

        public TipoEvento TipoEventoSelezionato { get; set; }

        [Required(ErrorMessage = "Il tipo di evento è obbligatoria.")]
        public List<SelectListItem> OpzioniTipoEvento = new List<SelectListItem>
        {
            new SelectListItem { Value = ((int)TipoEvento.GaraDaPreparare).ToString(), Text = "Gara da Preparare" },
            new SelectListItem { Value = ((int)TipoEvento.AtletaDaContattare).ToString(), Text = "Atleta da Contattare" },
            new SelectListItem { Value = ((int)TipoEvento.AllenamentoPersonal).ToString(), Text = "Allenamento Personal" },
            new SelectListItem { Value = ((int)TipoEvento.Altro).ToString(), Text = "Altro" }
        };

        [Required(ErrorMessage = "La data di scadenza è obbligatoria.")]
        public DateTime DataScadenza { get; set; }
        public string? Titolo { get; set; } = string.Empty;

        public string? Descrizione { get; set; } = string.Empty;
    }
}
