using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class Category
    {
        public Category()
        {
            Assets = new HashSet<Asset>();
        }

        public int Id { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
    }
}
