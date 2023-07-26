using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages
{
    public class LoginModel : PageModel
    {
        private readonly StockManagementContext _context;

        public LoginModel(StockManagementContext context)
        {
            _context = context;
        }

        [BindProperty] public List<User> Users { get; set; } = default!;
        [BindProperty] public User UserLogging { get; set; } = default!;
        [BindProperty] public int Error { get; set; }

        public void OnGetAsync()
        {
            Users = _context.Users.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var selectedUser = await _context.Users.SingleOrDefaultAsync(x =>
                x.Username.Equals(UserLogging.Username) &&
                x.Password.Equals(UserLogging.Password));
            if (selectedUser is null) return RedirectToPage();
            if (selectedUser.RoleId == 1)
            {
                HttpContext.Session.SetString("username", selectedUser.Username);
                HttpContext.Session.SetString("userId", selectedUser.Id.ToString());
                HttpContext.Session.SetString("role", "admin");
                return RedirectToPage("Admin/StockBorrowManagement");
            }
            else
            {
                HttpContext.Session.SetString("username", selectedUser.Username);
                HttpContext.Session.SetString("userId", selectedUser?.Id.ToString() ?? string.Empty);
                HttpContext.Session.SetString("role", "user");
                return RedirectToPage("Users/AssetBorrowing");
            }
        }
    }
}