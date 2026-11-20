using ECommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Utilities.DbSeeding
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole(CD.SUPER_ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(CD.ADMIN));
                    await _roleManager.CreateAsync(new IdentityRole(CD.EMPLOYEE));
                    await _roleManager.CreateAsync(new IdentityRole(CD.CUSTOMER));

                    await _userManager.CreateAsync(new ApplicationUser()
                    {
                        UserName = "SuperAdmin",
                        Email = "superadmin@eraasoft.com",
                        Name = "SuperAdmin",
                        EmailConfirmed = true
                    }, "SuperAdmin@123");

                    var user = await _userManager.FindByNameAsync("SuperAdmin");
                    await _userManager.AddToRoleAsync(user, CD.SUPER_ADMIN);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

    }
}
