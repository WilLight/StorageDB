using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbReservationRepository : LiteDbStoredRepository<ReservationModel>
    {
        private LiteDatabase _liteDb;

        public LiteDbReservationRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
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
    }
}