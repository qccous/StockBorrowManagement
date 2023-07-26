using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Admin;

public class BorrowHistoryManagementModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public BorrowHistoryManagementModel(StockManagementContext context)
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
        txtSearch = txtSearch?.ToLower();
        if (string.IsNullOrEmpty(txtSearch))
        {
            BorrowingAssets = await _context.BorrowingAssets.Include(x => x.Asset).Include(x => x.Borrower)
                .OrderByDescending(x => x.Status)
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
        return Page();
    }

    public async Task<IActionResult> OnPostUpdate(int selectedBorrowAssetId)
    {
        try
        {
            var selectedBorrowAsset = _context.BorrowingAssets.FirstOrDefault(x => x.Id == selectedBorrowAssetId);
            var selectedAsset =
                _context.Assets.FirstOrDefault(x => selectedBorrowAsset != null && x.Id == selectedBorrowAsset.AssetId);
            if (selectedBorrowAsset == null) return RedirectToPage();
            selectedBorrowAsset.Status = false;
            _context.BorrowingAssets.Update(selectedBorrowAsset);
            if (selectedAsset != null)
            {
                selectedAsset.Quantity = selectedAsset.Quantity + selectedBorrowAsset.Quantity;
                _context.Assets.Update(selectedAsset);
            }

            await _context.SaveChangesAsync();
            SuccessMessage = "Trả thành công";
            return RedirectToPage();
        }
        catch (Exception e)
        {
            FailMessage = "Trả thất bại\n {e.ToString()}";
            Console.WriteLine(e);
            throw;
        }
    }
}