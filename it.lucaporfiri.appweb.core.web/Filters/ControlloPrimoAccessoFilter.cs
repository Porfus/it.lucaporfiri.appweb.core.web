using it.lucaporfiri.appweb.core.web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace it.lucaporfiri.appweb.core.web.Filters
{
    public class ControlloPrimoAccessoFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ControlloPrimoAccessoFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userPrincipal = context.HttpContext.User;
            if (userPrincipal.Identity != null && userPrincipal.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(userPrincipal);
                if (user != null && user.PrimoAccesso) 
                {
                    //NON è già sulla pagina di cambio password o di logout (per evitare loop infiniti)
                    var path = context.HttpContext.Request.Path.Value?.ToLower();
                    if (path != null && !path.Contains("/account/cambiapasswordprimoaccesso") &&
                    !path.Contains("/account/logout")) 
                    {
                        context.Result = new RedirectToActionResult("CambiaPasswordPrimoAccesso", "Account", null);
                        return;
                    }
                }
            }
            await next();
        }
    }
}
