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

        [Route("api/menu/item/{itemId}/user/{userId}/valoration/{valoration}")]
        public async Task<IActionResult> SetValoration(int itemId, long userId, int valoration)
        {
            var itemValoration = db.ItemValoration.FirstOrDefault(x => x.ItemId == itemId && x.UserId == userId);
            if (itemValoration == null)
            {
                var newItemValoration = new ItemValoration{
                    ItemId = itemId, UserId = userId, Valoration = valoration
                };

                db.ItemValoration.Add(newItemValoration);
                await db.SaveChangesAsync();

            }else{
                itemValoration.Valoration = valoration;
                await db.SaveChangesAsync();
            }

            var item = db.Items.FirstOrDefault(x => x.Id == itemId);
            if(item != null){
                var empresa = db.Empresas.FirstOrDefault(x => x.Name == item.EmpresaName);
                if(empresa != null){

                    var result = from c in db.Items 
                    join v in db.ItemValoration on c.Id equals v.ItemId 
                    where c.EmpresaName == empresa.Name
                    select v;

                    empresa.Rating = result.Average(x => x.Valoration);
                    db.SaveChangesAsync();
                }
            }

            return Json(new {valoration});
        }

        [Route("api/menu/item/{itemId}/user/{userId}/valoration")]
        public IActionResult GetValorationUser(int itemId, long userId){
            var itemValoration = db.ItemValoration.FirstOrDefault( c =>  c.ItemId == itemId && c.UserId == userId);

            return Json(new {
                valoration = itemValoration == null ? 0 : itemValoration.Valoration
            });
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
                likeCount =  db.ItemUserLike.Where(c => c.ItemId == x.Id).Count(),
                Valoration = db.ItemValoration.FirstOrDefault(c => c.ItemId == x.Id) == null ? 0 : db.ItemValoration.Where(c => c.ItemId == x.Id).Average(x => x.Valoration)
            });


            return Json(new {
                Name=menuClient.Name,
                DescripcionName=menuClient.DescripcionName,
                Rating = menuClient.Rating,
                UrlImagen=menuClient.UrlImagen,
                items
            });
        }
    }
}
