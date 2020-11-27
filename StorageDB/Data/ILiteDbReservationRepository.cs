using System;
using System.Collections.Generic;
using StorageDB.Models;
using LiteDB;

namespace StorageDB.Data
{
    public interface ILiteDbReservationRepository : ILiteDbIndexedRepository<ReservationModel>
    {
        IEnumerable<ReservationModel> FindWithinDateRange(DateTime startDate, DateTime endDate);
        IEnumerable<ReservationModel> FindOverlappingDateRange(DateTime startDate, DateTime endDate);
    }
}