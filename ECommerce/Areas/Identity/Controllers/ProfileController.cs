using ECommerce.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            // ------ Manual mapping ------
            //var userVM = new ApplicationUserVM();
            //userVM.Name = user.Name;
            //userVM.Email = user.Email;
            //userVM.Phone = user.PhoneNumber;
            //userVM.Address = user.Address;

            // ------ Mapping using Mapster ------
            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }

        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            user.Name = applicationUserVM.Name;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update user successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description)); 
                TempData["Error_Notification"] = errors;
            }
            else
            {
                TempData["Successful_Notification"] = "update user successfully";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
