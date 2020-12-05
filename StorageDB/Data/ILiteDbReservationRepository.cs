using System;
using System.Collections.Generic;
using StorageDB.Models;
using LiteDB;

namespace StorageDB.Data
{
    public interface ILiteDbReservationRepository : ILiteDbIndexedRepository<ReservationModel>, ILiteDbStoredRepository<ReservationModel>
    {
        IEnumerable<ReservationModel> FindWithinDateRange(DateTime startDate, DateTime endDate, Guid storageId = default);
        IEnumerable<ReservationModel> FindOverlappingDateRange(DateTime startDate, DateTime endDate, Guid storageId = default);
    }
}