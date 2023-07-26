using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Users;

public class EditModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public EditModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }

    [BindProperty] public new User User { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? userId)
    {
        if (userId == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        User = user;
        ViewData["RoleName"] = new SelectList(_context.Roles, "RoleName", "RoleName");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // return RedirectToPage("../Admin/UserManagement");
            SuccessMessage = "Cập nhật thành công";
            return Page();
        }
        catch (Exception e)
        {
            FailMessage = "Cập nhật thất bại: " + e;
            throw;
        }
    }

    private bool UserExists(int id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}