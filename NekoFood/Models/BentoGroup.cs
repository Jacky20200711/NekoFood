using System;
using System.Collections.Generic;

namespace NekoFood.Models
{
    public partial class BentoGroup
    {
        public int Id { get; set; }
        public string Creator { get; set; } = null!;
        public string GroupGuid { get; set; } = null!;
        public string ShopGuid { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
