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
    public class DeliveryController : ControllerBase
    {
        private readonly ILogger<DeliveryController> _logger;
        private readonly IOrderService _orderService;
        private readonly IValidationService _validationService;

        public DeliveryController(ILogger<DeliveryController> logger, IOrderService orderService, IValidationService validationService)
        {
            _orderService = orderService;
            _validationService = validationService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<DeliveryModel> Get()
        {
            return _orderService.GetAllDeliveries();
        }

        public ActionResult<DeliveryModel> GetOne(Guid id)
        {
            var result = _orderService.GetOneDelivery(id);

            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> InsertOne(DeliveryModel delivery)
        {
            // Validate reference ids.
            if (!_validationService.ValidateStorage(delivery.StorageId))
                return BadRequest(new {message = "StorageId does not point to existing storage"});

            if (delivery.ItemId != default && !_validationService.ValidateItem(delivery.ItemId))
                return BadRequest(new {message = "There is no Item with such ItemId"});

            if (!_validationService.ValidateDeliveryVolume(delivery))
                    return BadRequest(new {message = "Delivery is over storage capacity"});

            var dto = _orderService.InsertOneDelivery(delivery);

            if(dto != null)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> UpdateOne(DeliveryModel delivery)
        {
            if (_validationService.ValidateDeliveryUpdate(delivery))
            {
                if (!_validationService.ValidateDeliveryVolume(delivery))
                    return BadRequest(new {message = "Delivery is over storage capacity"});

                var dto = _orderService.UpdateOneDelivery(delivery);
                return Ok(delivery);
            }
            else
                return BadRequest();

        }
        /*
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
        */
    }
}