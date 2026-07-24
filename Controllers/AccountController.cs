using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffCore_RD.Models;

namespace StaffCore_RD.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // ── REGISTER ─────────────────────────────────────────────────────────

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Primer usuario en el sistema → rol Administrador; los demás → Viewer
                bool esPrimerUsuario = _userManager.Users.Count() == 1;
                string rol = esPrimerUsuario ? "Administrador" : "Viewer";
                await _userManager.AddToRoleAsync(user, rol);

                // Iniciar sesión automáticamente tras el registro
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Agregar errores de Identity al ModelState para mostrarlos en la vista
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ── LOGIN ─────────────────────────────────────────────────────────────

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // lockoutOnFailure: true → bloquea la cuenta tras 5 intentos fallidos (RN-C2)
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Redirigir a la ruta que se intentaba visitar, o al Home si no hay ReturnUrl
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                // Mensaje específico para cuenta bloqueada (RN-C3: debe ser distinto al de credenciales incorrectas)
                ModelState.AddModelError(string.Empty,
                    "Cuenta bloqueada temporalmente. Intenta de nuevo en unos minutos.");
                return View(model);
            }

            // Credenciales incorrectas (sin indicar cuál de los dos campos falló, por seguridad)
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // ── LOGOUT ────────────────────────────────────────────────────────────

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // El logout SIEMPRE vía POST — nunca por GET (RN-C4: evita CSRF en cierre de sesión)
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
