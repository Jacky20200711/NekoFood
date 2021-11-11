using System;
using System.Collections.Generic;

namespace NekoFood.Models
{
    public partial class BentoShop
    {
        public int Id { get; set; }
        public string ShopGuid { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
