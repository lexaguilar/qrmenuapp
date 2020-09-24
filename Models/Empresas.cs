using System;
using System.Collections.Generic;

namespace qrmenuapp.Models
{
    public partial class Empresas
    {
        public Empresas()
        {
            Items = new HashSet<Items>();
        }

        public string Name { get; set; }
        public string DescripcionName { get; set; }
        public string UrlImagen { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
