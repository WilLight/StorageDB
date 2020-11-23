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

        public IEnumerable<ReservationModel> FindWithinDateRange(ReservationModel reservation)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation")
                .Find(x => x.StartDate.Date <= reservation.EndDate.Date || x.EndDate.Date >= reservation.StartDate.Date);
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
        public bool Update(ReservationModel item)
        {
            return _liteDb.GetCollection<ReservationModel>("Reservation").Update(item);
        }
    }
}