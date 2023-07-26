using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Admin;

public class UserManagementModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public UserManagementModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }

    public List<User> Users { get; set; }
    public User SelectedUser { get; set; }
    public string? CurrentFilter { get; set; }
    public int UserCount { get; set; }
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }


    public async Task<IActionResult> OnGetAsync(string? txtSearch, int p = 1, int s = 5)
    {
        // var username = HttpContext.Session.GetString("username");
        // if (username == null)
        // {
        //     return RedirectToPage("/Login");
        // }
        txtSearch = txtSearch?.ToLower();
        if (txtSearch != null)
        {
            Users = await _context.Users.Where(x => x.Username.Contains(txtSearch)).Include(x => x.Role)
                .ToListAsync();
        }
        else
        {
            Users = await _context.Users.Include(x => x.Role).ToListAsync();
        }

        UserCount = Users.Count;
        Users = Users.Skip((p - 1) * s).Take(s).ToList();
        PageSize = s;
        PageNo = p;
        CurrentFilter = txtSearch;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int userId)
    {
        var selectedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
        try
        {
            if (selectedUser != null)
            {
                var orderOfUser = await _context.BorrowingAssets.Where(x => x.BorrowerId == selectedUser.Id)
                    .ToListAsync();
                foreach (var order in orderOfUser)
                {
                    _context.BorrowingAssets.Remove(order);
                }

                _context.Users.Remove(selectedUser);
                await _context.SaveChangesAsync();
                SuccessMessage = "Xóa thành công";
            }
        }
        catch (Exception e)
        {
            FailMessage = "Có lỗi xảy ra " + e;
            throw;
        }

        return RedirectToPage();
    }
}