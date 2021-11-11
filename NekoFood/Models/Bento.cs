using System;
using System.Collections.Generic;

namespace NekoFood.Models
{
    public partial class Bento
    {
        public int Id { get; set; }
        public string ShopGuid { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Price { get; set; }
    }
}
