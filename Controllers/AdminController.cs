using BookStore.Models;
using BookStore.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<DefaultUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListAllRole()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new()
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRole");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError( "", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var roles = await _roleManager.FindByIdAsync(id);
            if (roles == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{id}' was found";
                return View("Error");
            }

            EditRoleViewModel model = new()
            {
                Id = roles.Id,
                RoleName = roles.Name
            };

            foreach (var users in _userManager.Users) 
            {
                if (await _userManager.IsInRoleAsync(users, roles.Name))
                {
                    model.Users.Add(users.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var roles = await _roleManager.FindByIdAsync(model.Id);
            if (roles == null)
            {
                ViewData["ErrorMessage"] = $"No role with Id '{model.Id}' was found";
                return View("Error");
            }
            else 
            {
                roles.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(roles);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListAllRole");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }


        }
    }
}
