using System;
using System.Collections.Generic;

namespace NekoFood.Models
{
    public partial class UserAccount
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? UserGroup { get; set; }
    }
}
