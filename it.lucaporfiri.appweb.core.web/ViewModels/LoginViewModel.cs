using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Ricordami")]
        public bool Ricordami { get; set; }
    }
}
