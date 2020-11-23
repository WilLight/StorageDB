using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Data;
using StorageDB.Models;

namespace StorageDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ItemController : ControllerBase
    {
        private static readonly string[] Names = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ItemController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;

        public ItemController(ILogger<ItemController> logger, ILiteDbItemRepository dbItemService)
        {
            _dbItemService = dbItemService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ItemModel> Get()
        {
            return _dbItemService.FindAll().OrderBy(item => item.Name);
        }

        [HttpGet]
        public ActionResult<ItemModel> GetOne(Guid id)
        {
            var result = _dbItemService.FindOne(id);
            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<ItemModel> InsertOne(ItemModel item)
        {
            var dto = _dbItemService.FindOne(item.Id);
            if (dto == null)
                item.Id = Guid.NewGuid();
            else
                return BadRequest(dto);
            var id = _dbItemService.Insert(item);
            if (id != default)
                return CreatedAtAction("GetOne", _dbItemService.FindOne(id));
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ItemModel> UpdateOne(ItemModel item)
        {
            if (_dbItemService.Update(item))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ItemModel> GenerateOne()
        {
            var rng = new Random();
            var item = new ItemModel();
            item.Id = Guid.NewGuid();
            item.Name = Names[rng.Next(Names.Length)];
            item.Size = (float)rng.Next(170) / 3;
            var id = _dbItemService.Insert(item);
            if (id != default)
                return CreatedAtAction("GetOne", _dbItemService.FindOne(id));
            else
                return BadRequest();
        }
    }
}