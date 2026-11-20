using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN} , {CD.ADMIN}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users);
        }
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            var isSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN);

            if (isSuperAdmin)
            {
                TempData["Error_Notification"] = "You can't Lock Super Admin";
                return RedirectToAction(nameof(Index));
            }
            if(user.LockoutEnd == null || DateTime.UtcNow > user.LockoutEnd)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(4));
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
