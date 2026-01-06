using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.ViewModels
{
    public class PrimoAccessoViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public required string PasswordAttuale { get; set; } 

        [Required]
        [DataType(DataType.Password)]
        public required string NuovaPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NuovaPassword", ErrorMessage = "Le password non coincidono.")]
        public required string ConfermaPassword { get; set; }
    }
}
