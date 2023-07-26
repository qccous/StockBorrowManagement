using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class Asset
    {
        public Asset()
        {
            BorrowingAssets = new HashSet<BorrowingAsset>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string AssetName { get; set; } = null!;
        public bool? Status { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public string? Specification { get; set; }
        public int Quantity { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<BorrowingAsset> BorrowingAssets { get; set; }
    }
}
