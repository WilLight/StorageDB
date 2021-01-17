using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbDeliveryRepository : ILiteDbDeliveryRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbDeliveryRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public bool DeleteOne(Guid id)
        {
            _liteDb.GetCollection<DeliveryModel>("Delivery").Delete(id);
            return true;
        }

        public IEnumerable<DeliveryModel> FindAll()
        {
            return _liteDb.GetCollection<DeliveryModel>("Delivery").FindAll();
        }

        public IEnumerable<DeliveryModel> FindAllInStorage(Guid storageId)
        {
            return _liteDb.GetCollection<DeliveryModel>("Delivery").Find(x => x.StorageId == storageId);
        }

        public DeliveryModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<DeliveryModel>("Delivery")
                .FindOne(Delivery => Delivery.Id.Equals(id));
        }
        public Guid Insert(DeliveryModel item)
        {
            return _liteDb.GetCollection<DeliveryModel>("Delivery").Insert(item);
        }
        public bool UpdateOne(DeliveryModel item)
        {
            return _liteDb.GetCollection<DeliveryModel>("Delivery").Update(item);
        }
    }
}