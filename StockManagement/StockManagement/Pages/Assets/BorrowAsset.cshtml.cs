using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockManagement.Models;

namespace StockManagement.Pages.Assets;

public class BorrowAssetModel : PageModel
{
    private readonly StockManagementContext _context;

    public BorrowAssetModel(StockManagementContext context)
    {
        _context = context;
    }

    [BindProperty] public int AssetId { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public List<User> Users { get; set; }

    public void OnGet(int assetId)
    {
        AssetId = assetId;
        Users = _context.Users.ToList();
    }

    public IActionResult OnPost(int userId, DateTime dueDate, int quantity)
    {
        try
        {
            var asset = _context.Assets.Find(AssetId);
            if (asset != null)
            {
                if (asset.Quantity == 0)
                {
                    return RedirectToPage("/Admin/StockBorrowManagement");
                }

                asset.Quantity = asset.Quantity - quantity;
                asset.Status = true;
            }

            var borrowingAsset = new BorrowingAsset
            {
                BorrowDate = DateTime.Now,
                AssetId = AssetId,
                Status = true,
                Amount = 1,
                DueDate = dueDate,
                Quantity = quantity,
                BorrowerId = userId,
                TotalPricee = asset!.Price * quantity
            };

            _context.SaveChanges();
            _context.BorrowingAssets.Add(borrowingAsset);
            _context.SaveChanges();
            SuccessMessage = "Thao tác thành công";
            return RedirectToPage("/Admin/StockBorrowManagement");
        }
        catch (Exception e)
        {
            FailMessage = "Thao tác thất bại: " + e;
            throw;
        }
    }
}