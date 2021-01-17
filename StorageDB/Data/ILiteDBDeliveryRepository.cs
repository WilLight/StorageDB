using System;
using System.Collections.Generic;
using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbDeliveryRepository : ILiteDbIndexedRepository<DeliveryModel>, ILiteDbStoredRepository<DeliveryModel>
    {
        bool DeleteOne(Guid id);
    }
}