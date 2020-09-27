using System;
using System.Collections.Generic;

namespace qrmenuapp.Models
{
    public partial class Items
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CategoriaId { get; set; }
        public int MonedaId { get; set; }
        public decimal Price { get; set; }
        public bool IsSuggestion { get; set; }
        public string EmpresaName { get; set; }
        public string Descripcion { get; set; }
        public string UrlImagen { get; set; }
        public bool HasIva { get; set; }

        public virtual Categorias Categoria { get; set; }
        public virtual Empresas EmpresaNameNavigation { get; set; }
        public virtual Monedas Moneda { get; set; }
    }
}
