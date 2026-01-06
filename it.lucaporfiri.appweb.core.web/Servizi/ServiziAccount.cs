using it.lucaporfiri.appweb.core.web.Data;
using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.ViewModels;

namespace it.lucaporfiri.appweb.core.web.Servizi
{
    public class ServiziAccount
    {
        private readonly ContestoApp _context;
        public ServiziAccount(ContestoApp context)
        {
            _context = context;
        }

        public string GeneraPasswordTemporanea()
        {
            return "PasswordTemp123!";
        }

        public ApplicationUser CreaUserIdentity(AtletaCreateViewModel atleta)
        {
            return new ApplicationUser
            {
                UserName = atleta.Email, // Usiamo l'email come username
                Email = atleta.Email,
                Nome = atleta.Nome,
                Cognome = atleta.Cognome,
                PrimoAccesso = true,
                EmailConfirmed = true // Per ora lo consideriamo confermato dato che lo crea il coach
            };
        }
    }
}
