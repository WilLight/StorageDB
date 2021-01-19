using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbDeliveryRepository : LiteDbStoredRepository<DeliveryModel>
    {
        private LiteDatabase _liteDb;

        public LiteDbDeliveryRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public bool DeleteOne(Guid id)
        {
            _liteDb.GetCollection<DeliveryModel>("Delivery").Delete(id);
            return true;
        }
    }
}