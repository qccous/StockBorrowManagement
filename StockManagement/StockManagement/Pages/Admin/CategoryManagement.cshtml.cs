using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StockManagement.Models;

namespace StockManagement.Pages.Admin;

public class CategoryManagementModel : PageModel
{
    private readonly StockManagementContext _context;

#pragma warning disable CS8618
    public CategoryManagementModel(StockManagementContext context)
#pragma warning restore CS8618
    {
        _context = context;
    }


    public List<Category> Categories { get; set; }
    [BindProperty] public Category SelectedCategory { get; set; } = default!;
    public string? CurrentFilter { get; set; }
    public int CategoriesCount { get; set; }
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    [TempData] public string SuccessMessage { get; set; }
    [TempData] public string FailMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? txtSearch, int p = 1, int s = 5)
    {
        if (string.IsNullOrEmpty(txtSearch))
        {
            Categories = await _context.Categories.ToListAsync();
        }
        else
        {
            Categories = await _context.Categories
                .Where(x => x.CategoryName != null && x.CategoryName.Contains(txtSearch)).ToListAsync();
        }

        CategoriesCount = Categories.Count;
        Categories = Categories.Skip((p - 1) * s).Take(s).ToList();
        PageSize = s;
        PageNo = p;
        CurrentFilter = txtSearch;
        return Page();
    }

    public IActionResult OnPostDelete(int categoryId)
    {
        try
        {
            var selectedCate = _context.Categories.FirstOrDefault(x => x.Id.Equals(categoryId));
            var assetOfCate = _context.Assets.Where(x => x.CategoryId.Equals(categoryId)).ToList();
            if (selectedCate is null) return RedirectToPage();
            foreach (var asset in assetOfCate)
            {
                var orderAssetOfCate = _context.BorrowingAssets.Where(x => x.AssetId == asset.Id).ToList();
                foreach (var order in orderAssetOfCate)
                {
                    _context.BorrowingAssets.Remove(order);
                }

                _context.Assets.Remove(asset);
            }

            _context.Categories.Remove(selectedCate);
            _context.SaveChanges();
            SuccessMessage = "Xóa thành công";
        }
        catch (Exception e)
        {
            FailMessage = "Xóa thất bại\n {e.ToString()}";
            Console.WriteLine(e);
            throw;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(int? categoryId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var categoryToUpdate = await _context.Categories.FirstAsync(e => e.Id == categoryId);

            categoryToUpdate.CategoryName = SelectedCategory.CategoryName;
            await _context.SaveChangesAsync();

            SuccessMessage = "Cập nhật thành công";
        }
        catch (Exception e)
        {
            FailMessage = $"Cập nhật thất bại\n {e.ToString()}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var categoryToAdd = new Category()
            {
                CategoryName = SelectedCategory.CategoryName
            };
            _context.Categories.Add(categoryToAdd);
            await _context.SaveChangesAsync();

            SuccessMessage = "Tạo mới thành công";
        }
        catch (Exception e)
        {
            FailMessage = $"Tạo mới thất bại\n {e.ToString()}";
        }

        return RedirectToPage();
    }
}