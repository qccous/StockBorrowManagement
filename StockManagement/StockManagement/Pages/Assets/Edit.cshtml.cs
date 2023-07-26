using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Assets;

public class EditModel : PageModel
{
    private readonly StockManagementContext _context;
    private readonly IConfiguration _configuration;

    public EditModel(StockManagementContext context)
    {
        _context = context;
    }

    [BindProperty] public Asset Asset { get; set; } = default!;
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? assetId)
    {
        if (assetId == null)
        {
            return NotFound();
        }

        var asset = await _context.Assets.FirstOrDefaultAsync(m => m.Id == assetId);
        if (asset == null)
        {
            return NotFound();
        }

        Asset = asset;
        ViewData["CategoryName"] = new SelectList(_context.Categories, "CategoryName", "CategoryName");
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

            // Retrieve the existing asset from the database
            var existingAsset = await _context.Assets
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == Asset.Id);

            if (existingAsset == null)
            {
                return NotFound();
            }

            // Update the properties of the existing asset
            existingAsset.AssetName = Asset.AssetName;
            // Update other asset properties as needed

            // Update the category of the asset
            existingAsset.CategoryId = Asset.CategoryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(Asset.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage = "Cập nhật thành công";
            //   return RedirectToPage("../Admin/StockBorrowManagement");
            return RedirectToPage();
        }
        catch (Exception e)
        {
            FailMessage = "Cập nhật thất bại: " + e;
            throw;
        }
    }

    private bool AssetExists(int id)
    {
        return (_context.Assets?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}