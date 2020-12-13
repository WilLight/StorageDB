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

        public ReservationController(ILogger<ReservationController> logger, ILiteDbItemRepository dbItemService, ILiteDbReservationRepository dbReservationService, ILiteDbStorageRepository dbStorageService)
        {
            _dbItemService = dbItemService;
            _dbReservationService = dbReservationService;
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        public Dictionary<DateTime, int> ReservationCapacitySumDictionary(Guid storageId)
        {
            var reservationCapacitySumDictionary = new Dictionary<DateTime, int>();
            var reservations = _dbReservationService.FindAllInStorage(storageId).OrderBy(x => x.StartDate);
            var dateAnchor = reservations.First().StartDate;
            DateTime endDate = dateAnchor;

            foreach (var reservation in reservations)
            {
                if (endDate < reservation.EndDate)
                    endDate = reservation.EndDate;
            }

            while (dateAnchor <= endDate)
            {
                int reservationCapacitySum = 0;

                foreach (var reservation in reservations
                    .Where(x => x.EndDate >= dateAnchor && x.StartDate <= dateAnchor))
                {
                    reservationCapacitySum += ItemsCapacity(reservation.Volume, reservation.ItemId);
                }

                reservationCapacitySumDictionary.Add(dateAnchor, reservationCapacitySum);

                if (dateAnchor.AddDays(1) > endDate)
                    dateAnchor = endDate;
                else
                    dateAnchor = dateAnchor.AddDays(1);
            }

            return reservationCapacitySumDictionary;
        }

        public Dictionary<DateTime, int> ReservationCapacitySumOverlappingDateRangeDictionary(DateTime startDate, DateTime endDate, Guid storageId, Guid reservationId = default)
        {
            var reservationCapacitySumWithinDateRangeDictionary = new Dictionary<DateTime, int>();
            var overlappingReservations = _dbReservationService.FindOverlappingDateRange(startDate, endDate, storageId).Where(x => x.Id != reservationId);
            var dateAnchor = startDate;
            if (overlappingReservations != null)
            {
                while (dateAnchor <= endDate)
                {
                    int reservationCapacitySum = 0;

                    foreach (var reservation in overlappingReservations
                        .Where(x => x.EndDate >= dateAnchor && x.StartDate <= dateAnchor))
                    {
                        reservationCapacitySum += ItemsCapacity(reservation.Volume, reservation.ItemId);
                    }

                    reservationCapacitySumWithinDateRangeDictionary.Add(dateAnchor, reservationCapacitySum);

                    if (dateAnchor == endDate)
                        dateAnchor = dateAnchor.AddDays(1);
                    else if (dateAnchor.AddDays(1) > endDate)
                        dateAnchor = endDate;
                    else
                        dateAnchor = dateAnchor.AddDays(1);
                }
            }
            

            return reservationCapacitySumWithinDateRangeDictionary;
        }

        public bool ReservationIsOverStorageCapacity(ReservationModel reservation, bool update = false)
        {
            // Validate reservation size.
            var storageCapacity = _dbStorageService.FindOne(reservation.StorageId).Capacity;
            var reservationCapacitySum = new Dictionary<DateTime, int>();
            if(update)
                reservationCapacitySum = ReservationCapacitySumOverlappingDateRangeDictionary(reservation.StartDate, reservation.EndDate, reservation.StorageId, reservation.Id);
            else
                reservationCapacitySum = ReservationCapacitySumOverlappingDateRangeDictionary(reservation.StartDate, reservation.EndDate, reservation.StorageId);

            foreach(var sum in reservationCapacitySum)
            {
                if (storageCapacity < sum.Value + ItemsCapacity(reservation.Volume, reservation.ItemId))
                    return true;
            }
            return false;
        }

        [HttpGet]
        public IEnumerable<ReservationModel> Get()
        {
            return _dbReservationService.FindAll().OrderBy(item => item.Id);
        }

        [HttpGet]
        public ActionResult<ReservationModel> GetOne([FromBody] Guid id)
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

            if (ReservationIsOverStorageCapacity(reservation, true))
                return BadRequest("Combined reservation size exceeds storage capacity");
            
            if (_dbReservationService.UpdateOne(reservation))
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
            reservation.StartDate = DateTime.Today.AddDays(rng.Next(5));
            reservation.EndDate = reservation.StartDate.AddDays(rng.Next(5));
            reservation.Volume = 10;

            return InsertOne(reservation);
        }
    }
}