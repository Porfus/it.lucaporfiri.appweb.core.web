using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static it.lucaporfiri.appweb.core.web.ViewModels.AtletaDetailViewModel;
using static it.lucaporfiri.appweb.core.web.ViewModels.SchedaAllenamentoViewModel;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziAtleta
    {
        private ContestoApp _context;
        public ServiziAtleta(ContestoApp context)
        {
            _context = context;
        }
        public Atleta? DaiAtleta(int? IdAtleta)
        {
            if (IdAtleta == null)
                return null;
            return _context.Atleta.FirstOrDefault(s => s.Id == IdAtleta);
        }
        public async Task<List<Atleta>> GetAllAtletiAsync()
        {
            return await _context.Atleta.ToListAsync();
        }
        public async Task<List<Atleta>> GetAllAtletiConAbbonamentiAsync()
        {
            return await _context.Atleta.Include(a => a.Abbonamenti).ToListAsync();
        }
        public ICollection<Atleta> Ricerca(int skip, int pageSize, out int risultatiTotali, string? filtroNome = null)
        {
            IQueryable<Atleta> query = _context.Atleta.Include(a => a.Abbonamenti).Include(a => a.Schede);
            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(a => a.Nome.Contains(filtroNome) || a.Cognome.Contains(filtroNome));
            }
            risultatiTotali = query.Count();
            return query.Skip(skip).Take(pageSize).ToList();
        }
        public async Task CreaAtleta(Atleta atleta)
        {
            _context.Atleta.Add(atleta);
            await _context.SaveChangesAsync();
        }

        public async Task ModificaAtleta(Atleta atleta)
        {
            _context.Update(atleta);
            await _context.SaveChangesAsync();
        }

        public async Task EliminaAtleta(int id)
        {
            var atleta = _context.Atleta
                .Include(a => a.Schede)
                .Include(b => b.Abbonamenti)
                .FirstOrDefault(a => a.Id == id);

            if (atleta == null) return;

            // Remove related Schede if any
            if (atleta.Schede != null)
            {
                _context.Scheda.RemoveRange(atleta.Schede);
            }
            if (atleta.Abbonamenti != null)
            {
                _context.Abbonamento.RemoveRange(atleta.Abbonamenti);
            }
            // Safely remove the Atleta entity
            if (atleta != null)
            {
                _context.Atleta.Remove(atleta);
            }

            await _context.SaveChangesAsync();
        }
        public List<SelectListItem> DaiTipiAtleta()
        {
            return Enum.GetValues(typeof(Atleta.TipoCliente))
                .Cast<Atleta.TipoCliente>()
                .Select(tc => new SelectListItem
                {
                    Value = tc.ToString(),
                    Text = tc.ToString()
                })
                .ToList();
        }
        public List<SelectListItem> DaiStatiAtleta()
        {
            return Enum.GetValues(typeof(Atleta.StatoCliente))
                .Cast<Atleta.StatoCliente>()
                .Select(sc => new SelectListItem
                {
                    Value = sc.ToString(),
                    Text = sc.ToString()
                })
                .ToList();
        }

        public List<SelectListItem> DaiSelectListAtleti()
        {
            return _context.Atleta
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Nome} {a.Cognome}"
                })
                .ToList();
        }
        public StatoAbbonamento CalcolaStatoUltimoAbbonamento(Atleta atleta)
        {
            if (atleta.Abbonamenti == null || !atleta.Abbonamenti.Any())
            {
                return StatoAbbonamento.NonDefinito;
            }
            var abbonamento = atleta.Abbonamenti
                .OrderByDescending(a => a.DataFine)
                .FirstOrDefault();

            if (abbonamento == null)
                return StatoAbbonamento.NonDefinito;

            bool attivo = abbonamento.DataInizio <= DateTime.Now && abbonamento.DataFine >= DateTime.Now;
            return attivo ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }
        public StatoAbbonamento CalcolaStatoAbbonamento(Atleta atleta) 
        {
            if (atleta.Abbonamenti == null || !atleta.Abbonamenti.Any())
            {
                return StatoAbbonamento.NonDefinito;
            }
            var statoAbbonamento = atleta.Abbonamenti.Any(s => s.DataInizio <= DateTime.Now && s.DataFine>= DateTime.Now);
            return statoAbbonamento ? StatoAbbonamento.Valido : StatoAbbonamento.Scaduto;
        }
        public int CalcolaEta(Atleta atleta)
        {
            if (atleta.AnnoDiNascita.HasValue)
            {
                return DateTime.Now.Year - atleta.AnnoDiNascita.Value.Year -
                       (DateTime.Now.DayOfYear < atleta.AnnoDiNascita.Value.DayOfYear ? 1 : 0);
            }
            else
                return 0; 
        }
        public ICollection<Atleta> DaiAtleti()
        {
            return [.. _context.Atleta.Include(a => a.Abbonamenti).Include(a => a.Schede)];
        }
        public int DaiNumeroAtletiConAbbonamentiScaduti()
        {
            return _context.Atleta.Count(a =>
                a.Abbonamenti != null && a.Abbonamenti.Any() && !a.Abbonamenti.Any(ab => ab.DataInizio <= DateTime.Now && ab.DataFine >= DateTime.Now)
            );
        }
        public StatoScheda CalcolaStatoUltimaScheda(Atleta atleta)
        {
            if (atleta.Schede == null || !atleta.Schede.Any())
            {
                return StatoScheda.NonAttiva;
            }
            var scheda = atleta.Schede
                .OrderByDescending(s => s.DataFine)
                .FirstOrDefault();
            if (scheda == null)
                return StatoScheda.NonAttiva;
            if (scheda.DataInizio > DateTime.Now)
            {
                return StatoScheda.NonAttiva;
            }
            else if (scheda.DataFine >= DateTime.Now)
            {
                return StatoScheda.Attiva;
            }
            else
            {
                return StatoScheda.Scaduta;
            }
        }
    }
}
