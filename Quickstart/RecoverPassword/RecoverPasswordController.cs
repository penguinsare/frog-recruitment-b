using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crm.Authentication;
using crm.Models;
using crm.Services;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace crm.Quickstart.RecoverPassword
{
    public class RecoverPasswordController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IRecoveryCodeValidator _codeValidator;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RecoverPasswordController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IRecoveryCodeValidator codeValidator,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _codeValidator = codeValidator;
            _roleManager = roleManager;
        }
        // GET: RecoverPassword
        public ActionResult Validate()
        {
            return View();
        }

        // GET: RecoverPassword/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RecoverPassword/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecoverPassword/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Validate(RecoverPasswordViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user == null)
                    {
                        ModelState.AddModelError("error", "User cannot use this recovery method. Please use password recovery through email.");
                        return View();
                    }

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Any(r => r == DefinedRoles.Boss))
                    {
                        _userManager.PasswordHasher.HashPassword(user, "");
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("error", "User cannot use this recovery method. Please use password recovery through email.");
                        //return RedirectToAction("Validate", "RecoverPassword");
                        return View();
                    }
                    
                }

                return View();
            }
            catch(Exception ex)
            {
                var debugEx = ex;
                return View();
            }
        }

        // GET: RecoverPassword/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecoverPassword/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Validate));
            }
            catch
            {
                return View();
            }
        }

        // GET: RecoverPassword/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecoverPassword/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Validate));
            }
            catch
            {
                return View();
            }
        }
    }
}