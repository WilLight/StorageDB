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

            while (dateAnchor <= endDate)
            {
                int reservationCapacitySum = 0;

                foreach (var reservation in overlappingReservations
                    .Where(x => x.EndDate >= dateAnchor && x.StartDate <= dateAnchor))
                {
                    reservationCapacitySum += ItemsCapacity(reservation.Volume, reservation.ItemId);
                }

                reservationCapacitySumWithinDateRangeDictionary.Add(dateAnchor, reservationCapacitySum);

                if (dateAnchor.AddDays(1) > endDate)
                    dateAnchor = endDate;
                else
                    dateAnchor = dateAnchor.AddDays(1);
            }

            return reservationCapacitySumWithinDateRangeDictionary;
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

            var reservationCapacitySumDictionary = ReservationCapacitySumDictionary(delivery.StorageId);

            // TODO: join two dictionaries and check for excess over storage capacity

            return false;
        }
    }
}