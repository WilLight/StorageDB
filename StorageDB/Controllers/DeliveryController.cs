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
        public ActionResult<IEnumerable<DeliveryModel>> Get()
        {
            var deliveries = _orderService.GetAllDeliveries();
            if (deliveries != null)
                return Ok(deliveries);
            else
                return NotFound();
        }

        [HttpGet]
        public ActionResult<IEnumerable<DeliveryModel>> GetFromCustomerId(Guid customerId)
        {
            var deliveries = _orderService.GetAllDeliveriesWithCustomer(customerId);
            if (deliveries != null)
                return Ok(deliveries);
            else
                return NotFound();
        }

        [HttpGet]
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
                return BadRequest(new { message = "StorageId does not point to existing storage" });

            if (delivery.ItemId != default && !_validationService.ValidateItem(delivery.ItemId))
                return BadRequest(new { message = "There is no Item with such ItemId" });

            if (!_validationService.ValidateCustomer(delivery.ClientId))
                return BadRequest(new { message = "There is no Item with such ItemId" });

            if (!_validationService.ValidateDeliveryVolume(delivery))
                return BadRequest(new { message = "Delivery is over storage capacity" });

            var dto = _orderService.InsertOneDelivery(delivery);

            if (dto != null)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<DeliveryModel> UpdateOne(DeliveryModel delivery)
        {
            if (_validationService.ValidateDeliveryUpdate(delivery))
            {
                if (delivery.ItemId != default && !_validationService.ValidateItem(delivery.ItemId))
                    return BadRequest(new { message = "There is no Item with such ItemId" });

                if (!_validationService.ValidateCustomer(delivery.ClientId))
                    return BadRequest(new { message = "There is no Item with such ItemId" });

                if (!_validationService.ValidateDeliveryVolume(delivery))
                    return BadRequest(new { message = "Delivery is over storage capacity" });

                var dto = _orderService.UpdateOneDelivery(delivery);

                return Ok(delivery);
            }
            else
                return BadRequest();

        }
    }
}