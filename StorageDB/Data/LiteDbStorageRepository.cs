using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbStorageRepository : ILiteDbStorageRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbStorageRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<StorageModel> FindAll()
        {
            return _liteDb.GetCollection<StorageModel>("Storage").FindAll();
        }

        public StorageModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<StorageModel>("Storage").FindOne(storage => storage.Id.Equals(id));
        }
        public Guid Insert(StorageModel item)
        {
            return _liteDb.GetCollection<StorageModel>("Storage").Insert(item);
        }
        public bool UpdateOne(StorageModel item)
        {
            return _liteDb.GetCollection<StorageModel>("Storage").Update(item);
        }
    }
}