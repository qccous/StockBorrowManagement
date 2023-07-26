using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Users;

public class CreateModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public CreateModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }

    public List<User> Users { get; set; }
    [BindProperty] public User SelectedUser { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var userExists = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.Equals(SelectedUser.Username));
        if (userExists == null)
        {
            _context.Users.Add(SelectedUser);
            await _context.SaveChangesAsync();
            SuccessMessage = "Tạo user thành công";
        }
        else
        {
            FailMessage = "Đã tồn tại user";
            return Page();
        }

        return Page();
    }
}