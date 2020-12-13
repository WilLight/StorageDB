using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbReservationRepository : ILiteDbReservationRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbReservationRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<ReservationModel> FindAll()
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation").FindAll();
        }

        public IEnumerable<ReservationModel> FindAllInStorage(Guid storageId)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation").Find(x => x.StorageId == storageId);
        }

        public IEnumerable<ReservationModel> FindWithinDateRange(DateTime startDate, DateTime endDate, Guid storageId = default)
        {
            if(storageId != default)
                return _liteDb.GetCollection<ReservationModel>("Reservation")
                    .Find(x => (x.StartDate.Date >= endDate.Date || x.EndDate.Date <= startDate.Date) && x.StorageId == storageId);
            else
                return _liteDb.GetCollection<ReservationModel>("Reservation")
                    .Find(x => x.StartDate.Date >= endDate.Date || x.EndDate.Date <= startDate.Date);
        }

        public IEnumerable<ReservationModel> FindOverlappingDateRange(DateTime startDate, DateTime endDate, Guid storageId = default)
        {
            if(storageId != default)
                return _liteDb.GetCollection<ReservationModel>("Reservation")
                    .Find(x => (x.StartDate.Date <= endDate.Date || x.EndDate.Date >= startDate.Date) && x.StorageId == storageId);
            else
                return _liteDb.GetCollection<ReservationModel>("Reservation")
                    .Find(x => x.StartDate.Date <= endDate.Date || x.EndDate.Date >= startDate.Date);
        }

        public ReservationModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation")
                .FindOne(reservation => reservation.Id.Equals(id));
        }
        public Guid Insert(ReservationModel item)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation").Insert(item);
        }
        public bool UpdateOne(ReservationModel item)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation").Update(item);
        }
    }
}