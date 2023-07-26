using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Admin;

public class StockBorrowManagementModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public StockBorrowManagementModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }

    public string NameSort { get; set; }
    public string DateSort { get; set; }
    public string? CurrentFilter { get; set; }
    public List<Asset>? Assets { get; set; }
    public Asset SelectedAsset { get; set; }
    public int AssetCount { get; set; }
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
            Assets = await _context.Assets.Where(x => x.Category.CategoryName != null && (x.Category.CategoryName != null && x.AssetName.Contains(txtSearch) || x.Category.CategoryName!.Contains(txtSearch)))
                .Include(x => x.Category).ToListAsync();
        }
        else
        {
            Assets = await _context.Assets.Include(x => x.Category).ToListAsync();
        }

        AssetCount = Assets.Count;
        Assets = Assets.Skip((p - 1) * s).Take(s).ToList();
        PageSize = s;
        PageNo = p;
        CurrentFilter = txtSearch;
        return Page();
    }


    public IActionResult OnPostDelete(int assetId)
    {
        try
        {
            var selectedAsset = _context.Assets.FirstOrDefault(x => x.Id.Equals(assetId));
            var borrowingAssetList = _context.BorrowingAssets.Where(x => x.AssetId.Equals(assetId)).ToList();
            if (selectedAsset is null) return RedirectToPage();
            foreach (var item in borrowingAssetList)
            {
                _context.Remove(item);
            }

            _context.Remove(selectedAsset);
            _context.SaveChanges();
            SuccessMessage = "Xóa thành công";
            return RedirectToPage();
        }
        catch (Exception e)
        {
            SuccessMessage = "Có lỗi xảy ra " + e;
            Console.WriteLine(e);
            throw;
        }
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove("userId");
        HttpContext.Session.Remove("username");
        HttpContext.Session.Remove("role");
        return Redirect("/Login");
    }
}