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
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IOrderService _orderService;
        private readonly IValidationService _validationService;

        public ReservationController(ILogger<ReservationController> logger, IOrderService orderService, IValidationService validationService)
        {
            _orderService = orderService;
            _validationService = validationService;
            _logger = logger;
        }

        [Route("api/[controller]/get")]
        [HttpGet]
        public IEnumerable<ReservationModel> Get()
        {
            return _orderService.GetAllReservations();
        }

        [Route("api/[controller]/get/{id}")]
        [HttpGet]
        public ActionResult<ReservationModel> GetOne(Guid id)
        {
            var result = _orderService.GetOneReservation(id);

            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<ReservationModel> InsertOne(ReservationModel reservation)
        {
            

            // Validate reference ids.
            if (!_validationService.ValidateStorage(reservation.StorageId))
                return BadRequest("StorageId does not point to existing storage");

            if (!_validationService.ValidateItem(reservation.ItemId))
                return BadRequest("There is no Item with such ItemId");

            if (reservation.StartDate >= reservation.EndDate)
                return BadRequest("Reservation cannot start after it ends");
            
            var dto = _orderService.InsertOneReservation(reservation);

            if (dto != null)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ReservationModel> UpdateOne(ReservationModel reservation)
        {
            // Validate reference ids.
            if (_validationService.ValidateReservationUpdate(reservation))
            {
                var dto = _orderService.UpdateOneReservation(reservation);
                return Ok(reservation);
            }
            else
                return BadRequest();
        }
    }
}