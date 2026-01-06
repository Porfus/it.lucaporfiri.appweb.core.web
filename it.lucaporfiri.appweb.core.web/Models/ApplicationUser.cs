using Microsoft.AspNetCore.Identity;

namespace it.lucaporfiri.appweb.core.web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nome { get; set; }
        public string? Cognome { get; set; }

        public bool PrimoAccesso { get; set; } = true;

        public int? AtletaId { get; set; }
        public Atleta? Atleta { get; set; }
    }
}
