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
    public class OrderService : IOrderService
    {
        private readonly LiteDbDeliveryRepository _dbDeliveryRepository;
        private readonly LiteDbReservationRepository _dbReservationRepository;

        public OrderService(IItemService itemService, LiteDbDeliveryRepository dbDeliveryRepository, LiteDbReservationRepository dbReservationRepository)
        {
            _dbDeliveryRepository = dbDeliveryRepository;
            _dbReservationRepository = dbReservationRepository;
        }

        public IEnumerable<DeliveryModel> GetAllDeliveries()
        {
            return _dbDeliveryRepository.FindAll().OrderBy(item => item.Id);
        }

        public IEnumerable<DeliveryModel> GetAllDeliveriesInStorage(Guid storageId)
        {
            return _dbDeliveryRepository.FindAllInStorage(storageId).OrderBy(item => item.Id);
        }

        public IEnumerable<DeliveryModel> GetAllDeliveriesWithCustomer(Guid customerId)
        {
            return _dbDeliveryRepository.FindAllWithCustomer(customerId).OrderBy(item => item.Id);
        }

        public DeliveryModel GetOneDelivery(Guid id)
        {
            return _dbDeliveryRepository.FindOne(id);
        }

        public DeliveryModel InsertOneDelivery(DeliveryModel delivery)
        {
            delivery.Id = Guid.NewGuid();
            if (_dbDeliveryRepository.Insert(delivery) != null)
                return delivery;
            else
                return null;
        }

        public DeliveryModel UpdateOneDelivery(DeliveryModel delivery)
        {
            if (_dbDeliveryRepository.UpdateOne(delivery))
                return delivery;
            else
                return null;
        }

        public IEnumerable<ReservationModel> GetAllReservations()
        {
            return _dbReservationRepository.FindAll().OrderBy(item => item.Id);
        }

        public IEnumerable<ReservationModel> GetAllReservationsInStorage(Guid storageId)
        {
            return _dbReservationRepository.FindAllInStorage(storageId).OrderBy(item => item.Id);
        }

        public IEnumerable<ReservationModel> GetReservationsOverlappingDateRange(DateTime startDate, DateTime endDate, Guid storageId)
        {
            return _dbReservationRepository.FindOverlappingDateRange(startDate, endDate, storageId);
        }

        public ReservationModel GetOneReservation(Guid id)
        {
            return _dbReservationRepository.FindOne(id);
        }

        public ReservationModel InsertOneReservation(ReservationModel reservation)
        {
            reservation.Id = Guid.NewGuid();
            if (_dbReservationRepository.Insert(reservation) != null)
                return reservation;
            else
                return null;
        }

        public ReservationModel UpdateOneReservation(ReservationModel reservation)
        {
            if (_dbReservationRepository.UpdateOne(reservation))
                return reservation;
            else
                return null;
        }
    }
}