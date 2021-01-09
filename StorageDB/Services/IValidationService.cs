using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace StorageDB.Services
{
    public interface IValidationService
    {
        bool ValidateCustomer(Guid customerId);
        bool ValidateItem(Guid itemId);
        bool ValidateStorage(Guid storageId);
        bool ValidateDeliveryUpdate(DeliveryModel delivery);
        bool ValidateReservationUpdate(ReservationModel reservation);
    }
}