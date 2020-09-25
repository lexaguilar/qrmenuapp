using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using qrmenuapp.Models;

namespace qrmenuapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly QrMenuContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, QrMenuContext _db)
        {
            db = _db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy(string name)
        {
            var empresa = db.Empresas.Find(name);
            ViewData["Title"] = empresa.DescripcionName;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("api/menu/get")]
        public IActionResult getMenu(int skip, int take, IDictionary<string, string> values)
        {

            IQueryable<Empresas> empresas = db.Empresas
            .OrderByDescending(x => x.Name);

            var items = empresas.Skip(skip).Take(take);

            return Json(new
            {
                items,
                totalCount = empresas.Count()
            });

        }

        [Route("api/menu/post")]
        public IActionResult Post([FromBody] Empresas empresa)
        {

            db.Empresas.Add(empresa);
            db.SaveChanges();
            return Json(empresa);

        }

        [Route("api/menu/put")]
        public IActionResult Put([FromBody] Empresas empresa)
        {

            var empresaOld = db.Empresas.Find(empresa.Name);
            empresaOld.DescripcionName = empresa.DescripcionName;

            db.SaveChanges();

            return Json(empresa);

        }

        [Route("api/item/all")]
        public IActionResult all(string empresaName,int skip, int take)
        {

            var items = db.Items.Where(x => x.EmpresaName == empresaName);
            return Json(new
            {
                items,
                totalCount = items.Count()
            });
            //return Json(item);

        }

        [Route("api/item/get")]
        public IActionResult getItem(int id)
        {

            var item = db.Items.Find(id);
            return Json(item);

        }

        [Route("api/item/post")]
        public IActionResult Post([FromBody] Items item)
        {

            db.Items.Add(item);
            db.SaveChanges();
            return Json(item);

        }

        [Route("api/item/put")]
        public IActionResult Put([FromBody] Items item)
        {

            var itemOld = db.Items.Find(item.Id);
            itemOld.Price = item.Price;
            itemOld.CategoriaId = item.CategoriaId;
            itemOld.MonedaId = item.MonedaId;
            itemOld.IsSuggestion = item.IsSuggestion;
            itemOld.Title = item.Title;
            itemOld.UrlImagen = item.UrlImagen;
            itemOld.Descripcion = item.Descripcion;

            db.SaveChanges();

            return Json(item);

        }

        [Route("api/categorias")]
        public IActionResult Categorias()
        {          

            return Json(db.Categorias);

        }

        [Route("api/monedas")]
        public IActionResult Monedas()
        {          

            return Json(db.Monedas);

        }
    }
}
