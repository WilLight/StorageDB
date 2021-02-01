using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Data;
using StorageDB.Services;
using StorageDB.Models;

namespace StorageDB.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ICustomerService _customerService;
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private readonly IStorageService _storageService;

        public ValidationService(ICustomerService customerService, IItemService itemService, IOrderService orderService, IStorageService storageService)
        {
            _customerService = customerService;
            _itemService = itemService;
            _orderService = orderService;
            _storageService = storageService;
        }

        public bool ValidateCustomer(Guid customerId)
        {
            return _customerService.GetOne(customerId) != null ? true : false;
        }

        public bool ValidateItem(Guid itemId)
        {
            return _itemService.GetOne(itemId) != null ? true : false;
        }

        public bool ValidateStorage(Guid storageId)
        {
            if (_storageService.GetOne(storageId) != null)
                return true;
            else
                return false;
        }

        public bool ValidateDeliveryUpdate(DeliveryModel delivery)
        {
            if(_orderService.GetOneDelivery(delivery.Id) != null)
                return true;
            else
                return false;
        }

        public bool ValidateReservationUpdate(ReservationModel reservation)
        {
            if(_orderService.GetOneReservation(reservation.Id) != null)
                return true;
            else
                return false;
        }

        public int CheckStorageOverflowRecursion(List<OrderValidationModel> orders, int storageSize)
        {
            var ordersCopy = orders;
            int ordersVolume = 0;

            foreach (var order in ordersCopy)
            { 
                var overlappingOrders = orders.Where(x => x.StartDate <= order.EndDate && x.EndDate >= order.StartDate).ToList();

                ordersCopy.Except(overlappingOrders);

                overlappingOrders.Remove(order);

                if (overlappingOrders.Count() > 0)
                {
                    var tempVolume = order.Volume + CheckStorageOverflowRecursion(overlappingOrders, storageSize);

                    if (ordersVolume < tempVolume)
                    {
                        ordersVolume = tempVolume;
                    }
                }
                else
                {
                    if (ordersVolume < order.Volume)
                    {
                        ordersVolume = order.Volume;
                    }
                }
            }
            return ordersVolume;
        }

        public bool CheckStorageOverflow(List<OrderValidationModel> orders, int storageSize)
        {
            var ordersCopy = orders;

            foreach (var order in ordersCopy)
            { 
                var overlappingOrders = orders.Where(x => x.StartDate <= order.EndDate && x.EndDate >= order.StartDate).ToList();

                ordersCopy.Except(overlappingOrders);

                overlappingOrders.Remove(order);

                if (order.Volume + CheckStorageOverflowRecursion(overlappingOrders, storageSize) > storageSize)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ValidateDeliveryVolume(DeliveryModel delivery)
        {
            var deliveries = _orderService.GetAllDeliveries().ToList();
            var overlappingReservations = _orderService.GetReservationsOverlappingDateRange(delivery.DeliveryDate, delivery.DeliveryDate.AddYears(5), delivery.StorageId);
            List<OrderValidationModel> orderValidationModels = new List<OrderValidationModel>();
            
            if (deliveries.FirstOrDefault(x => x.Id == delivery.Id) == default)
            {
                deliveries.Add(delivery);
            }
            else
            {
                deliveries.Remove(deliveries.First(x => x.Id == delivery.Id));
                deliveries.Add(delivery);
            }
            
            foreach (var d in deliveries)
            {
                OrderValidationModel order = new OrderValidationModel();
                order.StartDate = d.DeliveryDate;
                order.EndDate = d.DeliveryDate.AddYears(5);
                order.Volume = (int)(d.Volume/_itemService.CountItemsPerCell(d.ItemId));
                orderValidationModels.Add(order);
            }

            foreach (var reservation in overlappingReservations)
            {
                OrderValidationModel order = new OrderValidationModel();
                order.StartDate = reservation.StartDate;
                order.EndDate = reservation.EndDate;

                if(reservation.ItemId != default)
                    order.Volume = (int)(reservation.Volume/_itemService.CountItemsPerCell(reservation.ItemId));
                else
                    order.Volume = reservation.Volume;

                orderValidationModels.Add(order);
            }

            orderValidationModels.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            CheckStorageOverflow(orderValidationModels, _storageService.GetOne(delivery.StorageId).Capacity);

            return false;
        }

        public bool ValidateReservationVolume(ReservationModel reservation)
        {
            var deliveries = _orderService.GetAllDeliveries();
            var overlappingReservations = _orderService.GetReservationsOverlappingDateRange(reservation.StartDate, reservation.EndDate, reservation.StorageId).ToList();

            if (overlappingReservations.FirstOrDefault(x => x.Id == reservation.Id) == default)
            {
                overlappingReservations.Add(reservation);
            }
            else
            {
                overlappingReservations.Remove(overlappingReservations.First(x => x.Id == reservation.Id));
                overlappingReservations.Add(reservation);
            }

            List<OrderValidationModel> orderValidationModels = new List<OrderValidationModel>();
            
            foreach (var d in deliveries)
            {
                OrderValidationModel order = new OrderValidationModel();
                order.StartDate = d.DeliveryDate;
                order.EndDate = d.DeliveryDate.AddYears(5);
                order.Volume = (int)(d.Volume/_itemService.CountItemsPerCell(d.ItemId));
                orderValidationModels.Add(order);
            }

            foreach (var r in overlappingReservations)
            {
                OrderValidationModel order = new OrderValidationModel();
                order.StartDate = r.StartDate;
                order.EndDate = r.EndDate;

                if(r.ItemId != default)
                    order.Volume = (int)(r.Volume/_itemService.CountItemsPerCell(r.ItemId));
                else
                    order.Volume = r.Volume;

                orderValidationModels.Add(order);
            }

            orderValidationModels.Sort((x, y) => x.StartDate.CompareTo(y.StartDate));

            return !CheckStorageOverflow(orderValidationModels, _storageService.GetOne(reservation.StorageId).Capacity);
        }
    }
}