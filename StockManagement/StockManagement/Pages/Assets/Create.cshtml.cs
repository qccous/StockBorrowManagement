using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Pages.Assets;

public class CreateModel : PageModel
{
    private readonly StockManagementContext _context;

    public CreateModel(StockManagementContext context)
    {
        _context = context;
    }

    [BindProperty] public List<IFormFile> File { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public IActionResult OnGet()
    {
        ViewData["CategoryName"] = new SelectList(_context.Categories, "CategoryName", "CategoryName");
        return Page();
    }

    [BindProperty] public Asset SelectedAsset { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var existAsset = _context.Assets.Where(x => x.AssetName.Contains(SelectedAsset.AssetName)).ToList();
            if (existAsset.Count > 0)
            {
                return Page();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!File.Any())
            {
                SelectedAsset.Image = "/OIP.jpg";
            }
            else
            {
                foreach (var t in File.Where(t => t.Length > 0))
                {
                    SelectedAsset.Image = File[0].FileName;
                    var fileName = Path.GetFileName(t.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await t.CopyToAsync(stream);
                }
            }


            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var listFileName = Directory.GetFiles(imageDirectory).Select(Path.GetFileName).ToList();
            ViewData["listFile"] = listFileName;

            SelectedAsset.Status = false;

            //  Retrieve the category for the asset
            var category = await _context.Categories.Where(x =>
                    x.CategoryName != null &&
                    x.CategoryName.Equals(SelectedAsset.Category.CategoryName))
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            SelectedAsset.Category = category;

            _context.Assets.Add(SelectedAsset);
            SuccessMessage = "Tạo sản phẩm thành công";
            await _context.SaveChangesAsync();

            return RedirectToPage("../Admin/StockBorrowManagement");
        }
        catch (Exception e)
        {
            FailMessage = "Rạo sản phẩm thất bại: " + e;
            throw;
        }
    }
}