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
    public class DeliveryController : BaseController
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;
        private readonly ILiteDbDeliveryRepository _dbDeliveryService;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public DeliveryController(ILogger<DeliveryController> logger, ILiteDbItemRepository dbItemService, ILiteDbDeliveryRepository dbDeliveryService, ILiteDbStorageRepository dbStorageService)
        {
            _dbItemService = dbItemService;
            _dbDeliveryService = dbDeliveryService;
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<DeliveryModel> Get()
        {
            return _dbDeliveryService.FindAll().OrderBy(item => item.Id);
        }

        [HttpGet]
        public ActionResult<DeliveryModel> GetOne(Guid id)
        {
            var result = _dbDeliveryService.FindOne(id);

            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> InsertOne(DeliveryModel delivery)
        {
            var dto = _dbDeliveryService.FindOne(delivery.Id);

            if (dto == null)
                delivery.Id = Guid.NewGuid();
            else
                return BadRequest(dto);

            // Validate reference ids.
            if (_dbStorageService.FindOne(delivery.StorageId) == null)
                return BadRequest("StorageId does not point to existing storage");

            if (delivery.ItemId != default)
                if (_dbItemService.FindOne(delivery.ItemId) == null)
                    return BadRequest("There is no Item with such ItemId");
            
            var id = _dbDeliveryService.Insert(delivery);

            if (id != default)
                return CreatedAtAction("GetOne", _dbDeliveryService.FindOne(id));
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> UpdateOne(DeliveryModel delivery)
        {
            // Validate reference ids.
            if (_dbStorageService.FindOne(delivery.StorageId) == null)
                return BadRequest("StorageId does not point to existing storage");

            if (delivery.ItemId != default)
                if (_dbItemService.FindOne(delivery.ItemId) == null)
                    return BadRequest("There is no Item with such ItemId");
            
            if (_dbDeliveryService.Update(delivery))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> GenerateOne()
        {
            var rng = new Random();
            var Delivery = new DeliveryModel();

            Delivery.Id = Guid.NewGuid();
            Delivery.StorageId = _dbStorageService.FindAll().First<StorageModel>().Id;
            Delivery.ItemId = _dbItemService.FindAll().First<ItemModel>().Id;
            Delivery.DeliveryDate = DateTime.Today.AddDays(rng.Next(5));
            Delivery.ItemCount = 10;
            Delivery.ToStorage = true;

            return InsertOne(Delivery);
        }
    }
}