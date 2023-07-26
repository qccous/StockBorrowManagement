using System;
using System.Collections.Generic;

namespace StockManagement.Models
{
    public partial class User
    {
        public User()
        {
            BorrowingAssets = new HashSet<BorrowingAsset>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public string Addrress { get; set; } = null!;
        public bool? Status { get; set; }
        public bool Gender { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<BorrowingAsset> BorrowingAssets { get; set; }
    }
}
