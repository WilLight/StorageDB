using System;
using System.Collections.Generic;
using StorageDB.Models;
using LiteDB;

namespace StorageDB.Data
{
    public interface ILiteDbReservationRepository : ILiteDbIndexedRepository<ReservationModel>
    {
        IEnumerable<ReservationModel> FindWithinDateRange(ReservationModel reservation);
    }
}