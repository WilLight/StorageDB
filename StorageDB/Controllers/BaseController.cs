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
    public class BaseController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;
        private readonly ILiteDbDeliveryRepository _dbDeliveryService;
        private readonly ILiteDbReservationRepository _dbReservationService;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public int ItemsPerCell(float itemSize)
        {
            var result = (int)MathF.Floor(1 / itemSize);
            if(result < 1)
                result = 1;
            return result;
        }

        public int ReservationCapacity(int reservationVolume, Guid itemId)
        {
            ItemModel item = null;
            if(itemId != default)
                item = _dbItemService.FindOne(itemId);
            if (item == null)
                return reservationVolume;
            else
                return reservationVolume * ItemsPerCell(item.Size);
        }

        public bool ReservationIsOverStorageCapacity(ReservationModel reservation, bool update = false)
        {
            // Validate reservation size.
            var storageCapacity = _dbStorageService.FindOne(reservation.StorageId).Capacity;
            Dictionary<DateTime, int> reservationCapacitySum = new Dictionary<DateTime, int>();
            if(update)
                reservationCapacitySum = ReservationCapacitySumOverlappingDateRangeDictionary(reservation.StartDate, reservation.EndDate, reservation.StorageId, reservation.Id);
            else
                reservationCapacitySum = ReservationCapacitySumOverlappingDateRangeDictionary(reservation.StartDate, reservation.EndDate, reservation.StorageId);

            foreach(var sum in reservationCapacitySum)
            {
                if (storageCapacity < sum.Value + ReservationCapacity(reservation.Volume, reservation.ItemId))
                    return true;
            }
            return false;
        }

        public Dictionary<DateTime, int> ReservationCapacitySumOverlappingDateRangeDictionary(DateTime startDate, DateTime endDate, Guid storageId = default, Guid reservationId = default)
        {
            Dictionary<DateTime, int> reservationCapacitySumWithinDateRangeDictionary = new Dictionary<DateTime, int>();
            var overlappingReservations = _dbReservationService.FindOverlappingDateRange(startDate, endDate, storageId).Where(x => x.Id != reservationId);
            var dateAnchor = startDate;

            while(dateAnchor <= endDate)
            {
                int reservationCapacitySum = 0;

                foreach (var reservation in overlappingReservations
                    .Where(x => x.EndDate >= dateAnchor && x.StartDate <= dateAnchor))
                {
                    reservationCapacitySum += ReservationCapacity(reservation.Volume, reservation.ItemId);
                }

                reservationCapacitySumWithinDateRangeDictionary.Add(dateAnchor, reservationCapacitySum);
                
                if(dateAnchor.AddDays(1) > endDate)
                    dateAnchor = endDate;
                else
                    dateAnchor = dateAnchor.AddDays(1);
            }

            return reservationCapacitySumWithinDateRangeDictionary;
        }
    }
}