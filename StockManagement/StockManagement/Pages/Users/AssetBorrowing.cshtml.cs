using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Users;

public class AssetBorrowingModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public AssetBorrowingModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }

    public string? CurrentFilter { get; set; }
    public List<BorrowingAsset>? BorrowingAssets { get; set; }
    public int BorrowingAssetsCount { get; set; }
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? txtSearch, int p = 1, int s = 5)
    {
        var username = HttpContext.Session.GetString("username");
        if (username != null)
        {
            txtSearch = txtSearch?.ToLower();
            if (string.IsNullOrEmpty(txtSearch))
            {
                BorrowingAssets = await _context.BorrowingAssets.Include(x => x.Asset).Include(x => x.Borrower)
                    .Where(x => x.Borrower != null && x.Borrower.Username.Contains(username)).OrderBy(x => x.Id)
                    .ToListAsync();
            }
            else
            {
                BorrowingAssets = await _context.BorrowingAssets
                    .Where(x => x.Asset != null && x.Borrower != null &&
                                (x.Borrower.Username.Contains(txtSearch) || x.Asset.AssetName.Contains(txtSearch)))
                    .Include(x => x.Asset).Include(x => x.Borrower).OrderByDescending(x => x.Status)
                    .ToListAsync();
            }

            if (BorrowingAssets != null)
            {
                BorrowingAssetsCount = BorrowingAssets.Count;
                BorrowingAssets = BorrowingAssets.Skip((p - 1) * s).Take(s).ToList();
            }

            PageSize = s;
            PageNo = p;
            CurrentFilter = txtSearch;
        }

        return Page();
    }
    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove("userId");
        HttpContext.Session.Remove("username");
        HttpContext.Session.Remove("role");
        return Redirect("/Login");
    }
}