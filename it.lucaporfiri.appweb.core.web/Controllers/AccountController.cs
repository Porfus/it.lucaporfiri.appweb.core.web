using it.lucaporfiri.appweb.core.web.Models;
using it.lucaporfiri.appweb.core.web.Servizi;
using it.lucaporfiri.appweb.core.web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ServiziAccount _serviziAccount;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ServiziAccount serviziAccount)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _serviziAccount = serviziAccount;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) 
        {
            if (ModelState.IsValid) 
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.Ricordami, lockoutOnFailure: false);
                if (result.Succeeded) 
                {
                    if (user != null) 
                    {
                        if (user.PrimoAccesso)
                        {
                            return RedirectToAction("CambiaPasswordPrimoAccesso");
                        }
                        if (await _userManager.IsInRoleAsync(user, "Coach"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "AtletaArea"); 
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Tentativo di accesso non valido");
            }
            return View(model);
            
        }

        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CambiaPasswordPrimoAccesso()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (!user.PrimoAccesso)
            {
                if (await _userManager.IsInRoleAsync(user, "Coach"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Dashboard", "AtletaArea");
                }
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaPasswordPrimoAccesso(PrimoAccessoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null) return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.PasswordAttuale, model.NuovaPassword);

            if (result.Succeeded)
            {
                user.PrimoAccesso = false;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                return RedirectToAction("Dashboard", "AtletaArea"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }
}
