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

        [Route("api/menu/item/{itemId}/user/{userId}/isLiked/{isLiked}")]
        public IActionResult UpdateLike(int itemId, long userId, bool isLiked)
        {
            if(isLiked){
                var itemUser = db.ItemUserLike.FirstOrDefault(c =>  c.ItemId == itemId && c.UserId == userId );
                if(itemUser == null){
                    
                    db.ItemUserLike.Add(new ItemUserLike {ItemId = itemId, UserId = userId});
                    db.SaveChanges();
                }

            }else{

                var itemUser = db.ItemUserLike.FirstOrDefault(c =>  c.ItemId == itemId && c.UserId == userId );

                if(itemUser != null){

                    db.ItemUserLike.Remove(itemUser);
                    db.SaveChanges();

                }
            }
            return Json(new {isLiked});
        }

        [Route("api/menu/name/{name}/user/{userId}")]
        public IActionResult Get(string name, long userId)
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
                x.Id,
                x.Title,
                Categoria = x.Categoria.Descripcion,
                Moneda = x.Moneda.Descripcion,
                x.Price,
                x.IsSuggestion,
                x.EmpresaName,
                x.Descripcion,
                x.UrlImagen,
                x.HasIva,
                IsLiked = db.ItemUserLike.FirstOrDefault( c =>  c.ItemId == x.Id && c.UserId == userId ) == null ? false : true,
                likeCount =  db.ItemUserLike.Where(c => c.ItemId == x.Id).Count()
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
