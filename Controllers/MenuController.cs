using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using qrmenuapp.Models;

namespace qrmenuapp.Controllers
{
    [Route("[controller]")]
    public class MenuController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly QrMenuContext db;
        private readonly ILogger<HomeController> _logger;

        public MenuController(ILogger<HomeController> logger, QrMenuContext _db)
        {
            db = _db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(string name)
        {

            var menuClient = db.Empresas
            .Include(x => x.Items)
            .ThenInclude(y => y.Categoria)
            .Include(x => x.Items)
            .ThenInclude(y => y.Moneda)
            .FirstOrDefault(x => x.Name == name);

            if(menuClient==null)
                return NotFound();
            
            var items = menuClient.Items.Select(x => new
            {
                x.Title,
                Categoria = x.Categoria.Descripcion,
                Moneda = x.Moneda.Descripcion,
                x.Price,
                x.IsSuggestion,
                x.EmpresaName,
                x.Descripcion,
                x.UrlImagen,
                x.HasIva
            });


            return Json(new {
                Name=menuClient.Name,
                DescripcionName=menuClient.DescripcionName,
                UrlImagen=menuClient.UrlImagen,
                items
            });
        }
    }
}
