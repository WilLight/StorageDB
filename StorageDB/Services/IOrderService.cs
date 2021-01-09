using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace StorageDB.Services
{
    public interface IOrderService
    {
        IEnumerable<DeliveryModel> GetAllDeliveries();
        DeliveryModel GetOneDelivery(Guid id);
        DeliveryModel InsertOneDelivery(DeliveryModel delivery);
        DeliveryModel UpdateOneDelivery(DeliveryModel delivery);
        IEnumerable<ReservationModel> GetAllReservations();
        ReservationModel GetOneReservation(Guid id);
        ReservationModel InsertOneReservation(ReservationModel reservation);
        ReservationModel UpdateOneReservation(ReservationModel reservation);
    }
}