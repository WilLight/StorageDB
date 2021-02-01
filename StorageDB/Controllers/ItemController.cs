using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Services;
using StorageDB.Models;

namespace StorageDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemService _itemService;
        private readonly IValidationService _validationService;

        public ItemController(ILogger<ItemController> logger, IItemService itemService, IValidationService validationService)
        {
            _itemService = itemService;
            _validationService = validationService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ItemModel> Get()
        {
            return _itemService.GetAll();
        }

        public ActionResult<ItemModel> GetOne(Guid id)
        {
            var result = _itemService.GetOne(id);

            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<ItemModel> InsertOne(ItemModel item)
        {
            if (!_validationService.ValidateItem(item.Id))
                item.Id = Guid.NewGuid();
            else
                return BadRequest();
            var dto = _itemService.InsertOne(item);
            if (dto != default)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ItemModel> UpdateOne(ItemModel item)
        {
            if (_itemService.UpdateOne(item) != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}