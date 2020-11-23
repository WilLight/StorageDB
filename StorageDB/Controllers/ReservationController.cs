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
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;
        private readonly ILiteDbReservationRepository _dbReservationService;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public ReservationController(ILogger<ReservationController> logger, ILiteDbItemRepository dbItemService, ILiteDbReservationRepository dbReservationService, ILiteDbStorageRepository dbStorageService)
        {
            _dbItemService = dbItemService;
            _dbReservationService = dbReservationService;
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        private bool ReservationExceedsCapacity(ReservationModel reservation)
        {
            // Validate reservation size.
            var overlappingReservations = _dbReservationService.FindWithinDateRange(reservation).Where(x => x.Id != reservation.Id);
            var date = reservation.StartDate.Date;
            var storage = _dbStorageService.FindOne(reservation.StorageId);

            while (DateTime.Compare(date, reservation.EndDate.Date) <= 0)
            {
                int reservationSizeTotal = 0;
                
                var item = _dbItemService.FindOne(reservation.ItemId);
                if (item != null)
                {
                    reservationSizeTotal += (int)MathF.Ceiling(item.Size * reservation.Size);
                }
                else
                {
                    reservationSizeTotal += reservation.Size;
                }

                foreach (var overlappingReservation in overlappingReservations
                    .Where(x => DateTime.Compare(x.EndDate, date) >= 0))
                {
                    item = _dbItemService.FindOne(overlappingReservation.ItemId);
                    if (item != null)
                    {
                        reservationSizeTotal += (int)MathF.Ceiling(item.Size * overlappingReservation.Size);
                    }
                    else
                    {
                        reservationSizeTotal += overlappingReservation.Size;
                    }
                }

                if (reservationSizeTotal > storage.Capacity)
                    return true;

                date = date.AddDays(1);
            }
            return false;
        }

        [HttpGet]
        public IEnumerable<ReservationModel> Get()
        {
            return _dbReservationService.FindAll().OrderBy(item => item.Id);
        }

        [HttpGet]
        public ActionResult<ReservationModel> GetOne(Guid id)
        {
            var result = _dbReservationService.FindOne(id);

            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<ReservationModel> InsertOne(ReservationModel reservation)
        {
            var dto = _dbReservationService.FindOne(reservation.Id);

            if (dto == null)
                reservation.Id = Guid.NewGuid();
            else
                return BadRequest(dto);

            // Validate reference ids.
            if (_dbStorageService.FindOne(reservation.StorageId) == null)
                return BadRequest("StorageId does not point to existing storage");

            if (reservation.ItemId != default)
                if (_dbItemService.FindOne(reservation.ItemId) == null)
                    return BadRequest("There is no Item with such ItemId");

            if (DateTime.Compare(reservation.StartDate, reservation.EndDate) > 0)
                return BadRequest("Reservation cannot start before it ends");

            if (ReservationExceedsCapacity(reservation))
                return BadRequest("Combined reservation size exceeds storage capacity");
            
            var id = _dbReservationService.Insert(reservation);

            if (id != default)
                return CreatedAtAction("GetOne", _dbReservationService.FindOne(id));
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ReservationModel> UpdateOne(ReservationModel reservation)
        {
            // Validate reference ids.
            if (_dbStorageService.FindOne(reservation.StorageId) == null)
                return BadRequest("StorageId does not point to existing storage");

            if (reservation.ItemId != default)
                if (_dbItemService.FindOne(reservation.ItemId) == null)
                    return BadRequest("There is no Item with such ItemId");

            if (DateTime.Compare(reservation.StartDate, reservation.EndDate) > 0)
                return BadRequest("Reservation cannot start after it ends");

            if (ReservationExceedsCapacity(reservation))
                return BadRequest("Combined reservation size exceeds storage capacity");
            
            if (_dbReservationService.Update(reservation))
                return Ok();
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<ReservationModel> GenerateOne()
        {
            var rng = new Random();
            var reservation = new ReservationModel();

            reservation.Id = Guid.NewGuid();
            reservation.StorageId = _dbStorageService.FindAll().First<StorageModel>().Id;

            if (reservation.StorageId == default)
                return BadRequest("StorageId does not point to existing storage");

            reservation.StartDate = DateTime.Today.AddDays(rng.Next(5));
            reservation.EndDate = reservation.StartDate.AddDays(rng.Next(5));
            reservation.Size = 10;

            var id = _dbReservationService.Insert(reservation);

            if (id != default)
                return CreatedAtAction("GetOne", _dbReservationService.FindOne(id));
            else
                return BadRequest();
        }
    }
}