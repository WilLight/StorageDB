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
        IEnumerable<DeliveryModel> GetAllDeliveriesInStorage(Guid storageId);
        IEnumerable<DeliveryModel> GetAllDeliveriesWithCustomer(Guid clientId);
        DeliveryModel GetOneDelivery(Guid id);
        DeliveryModel InsertOneDelivery(DeliveryModel delivery);
        DeliveryModel UpdateOneDelivery(DeliveryModel delivery);
        IEnumerable<ReservationModel> GetAllReservations();
        IEnumerable<ReservationModel> GetAllReservationsInStorage(Guid storageId);
        IEnumerable<ReservationModel> GetReservationsOverlappingDateRange(DateTime startDate, DateTime endDate, Guid storageId);
        ReservationModel GetOneReservation(Guid id);
        ReservationModel InsertOneReservation(ReservationModel reservation);
        ReservationModel UpdateOneReservation(ReservationModel reservation);
    }
}