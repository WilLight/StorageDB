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
    }
}