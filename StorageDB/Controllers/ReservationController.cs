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

        private int ItemsPerCell(float itemSize)
        {
            var result = (int)MathF.Floor(1 / itemSize);
            if(result < 1)
                result = 1;
            return result;
        }

        private int ReservationCapacity(int reservationVolume, ItemModel item)
        {
            if (item == null)
                return reservationVolume;
            else
                return reservationVolume * ItemsPerCell(item.Size);
        }

        private bool ReservationIsOverStorageCapacity(ReservationModel reservation)
        {
            // Validate reservation size.
            var overlappingReservations = _dbReservationService.FindOverlappingDateRange(reservation.StartDate, reservation.EndDate).Where(x => x.Id != reservation.Id);
            var date = reservation.StartDate.Date;
            var storage = _dbStorageService.FindOne(reservation.StorageId);

            while (date <= reservation.EndDate.Date)
            {
                int reservationTotalCapacity = 0;

                reservationTotalCapacity = ReservationCapacity(reservation.Volume, _dbItemService.FindOne(reservation.ItemId));

                foreach (var overlappingReservation in overlappingReservations
                    .Where(x => x.EndDate >= date && x.StartDate <= date))
                {
                    reservationTotalCapacity = ReservationCapacity(overlappingReservation.Volume, _dbItemService.FindOne(overlappingReservation.ItemId));
                }

                if (reservationTotalCapacity > storage.Capacity)
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

            if (reservation.StartDate >= reservation.EndDate)
                return BadRequest("Reservation cannot start after it ends");

            if (ReservationIsOverStorageCapacity(reservation))
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

            if (ReservationIsOverStorageCapacity(reservation))
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
            reservation.Volume = 10;

            var id = _dbReservationService.Insert(reservation);

            if (id != default)
                return CreatedAtAction("GetOne", _dbReservationService.FindOne(id));
            else
                return BadRequest();
        }
    }
}