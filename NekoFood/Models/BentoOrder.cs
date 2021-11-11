using System;
using System.Collections.Generic;

namespace NekoFood.Models
{
    public partial class BentoOrder
    {
        public int Id { get; set; }
        public string Payer { get; set; } = null!;
        public string GroupGuid { get; set; } = null!;
        public string ShopGuid { get; set; } = null!;
        public string BentoName { get; set; } = null!;
        public int Number { get; set; }
        public int TotalCost { get; set; }
        public int IsChecked { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public string? Remark { get; set; }
    }
}
