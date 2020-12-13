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
    public class DeliveryController : ControllerBase
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;
        private readonly ILiteDbDeliveryRepository _dbDeliveryService;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public int ItemsPerCell(float itemSize)
        {
            var result = (int)MathF.Floor(1 / itemSize);
            if (result < 1)
                result = 1;
            return result;
        }

        public int ItemsCapacity(int itemsCount, Guid itemId)
        {
            ItemModel item = null;
            if (itemId != default)
                item = _dbItemService.FindOne(itemId);
            if (item == null)
                return itemsCount;
            else
                return itemsCount * ItemsPerCell(item.Size);
        }

        public DeliveryController(ILogger<DeliveryController> logger, ILiteDbItemRepository dbItemService, ILiteDbDeliveryRepository dbDeliveryService, ILiteDbStorageRepository dbStorageService)
        {
            _dbItemService = dbItemService;
            _dbDeliveryService = dbDeliveryService;
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        public Dictionary<DateTime, int> DeliveryItemCountSumDictionary(Guid storageId, Guid deliveryId = default)
        {
            var deliveryItemCountChangeDictionary = new Dictionary<DateTime, int>();
            var deliveries = _dbDeliveryService.FindAllInStorage(storageId)
                .Where(x => x.Id != deliveryId)
                .OrderBy(x => x.DeliveryDate);
            int deliverySum = 0;
            
            foreach (var delivery in deliveries)
            {
                if (delivery.ToStorage)
                    deliveryItemCountChangeDictionary.Add(delivery.DeliveryDate, deliverySum += ItemsCapacity(delivery.ItemCount, delivery.ItemId));
                else
                    deliveryItemCountChangeDictionary.Add(delivery.DeliveryDate, deliverySum -= ItemsCapacity(delivery.ItemCount, delivery.ItemId));
            }

            return deliveryItemCountChangeDictionary;
        }

        public bool DeliveryIsOverStorageCapacity(DeliveryModel delivery, bool update = false)
        {
            Dictionary<DateTime, int> deliveryItemCountSumDictionary;
            var storageCapacity = _dbStorageService.FindOne(delivery.StorageId).Capacity;

            if (update)
                deliveryItemCountSumDictionary = DeliveryItemCountSumDictionary(delivery.StorageId, delivery.Id);
            else
                deliveryItemCountSumDictionary = DeliveryItemCountSumDictionary(delivery.StorageId);

            DateTime dateAnchor = deliveryItemCountSumDictionary.First().Key;
            int deliveryItemCountSum = 0;

            foreach (var deliveryEntry in deliveryItemCountSumDictionary)
            {
                if (deliveryEntry.Key > dateAnchor)
                {
                    if (deliveryEntry.Key > delivery.DeliveryDate)
                        break;
                    else
                        deliveryItemCountSum = deliveryEntry.Value;
                }
            }

            if (delivery.ToStorage)
                deliveryItemCountSum += ItemsCapacity(delivery.ItemCount, delivery.ItemId);
            else
                deliveryItemCountSum -= ItemsCapacity(delivery.ItemCount, delivery.ItemId);

            deliveryItemCountSumDictionary.Add(delivery.DeliveryDate, deliveryItemCountSum);

            // TODO: join two dictionaries and check for excess over storage capacity

            return false;
        }

        [HttpGet]
        public IEnumerable<DeliveryModel> Get()
        {
            return _dbDeliveryService.FindAll().OrderBy(item => item.Id);
        }

        [HttpGet]
        public ActionResult<DeliveryModel> GetOne([FromBody] Guid id)
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
            
            if (_dbDeliveryService.UpdateOne(delivery))
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