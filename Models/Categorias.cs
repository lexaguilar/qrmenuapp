using System;
using System.Collections.Generic;

namespace qrmenuapp.Models
{
    public partial class Categorias
    {
        public Categorias()
        {
            Items = new HashSet<Items>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
