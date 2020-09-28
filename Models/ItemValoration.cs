using System;
using System.Collections.Generic;

namespace qrmenuapp.Models
{
    public partial class ItemValoration
    {
        public int ItemId { get; set; }
        public long UserId { get; set; }
        public int Valoration { get; set; }
    }
}
