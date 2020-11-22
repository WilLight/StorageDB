using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbItemRepository : ILiteDbItemRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbItemRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<ItemModel> FindAll()
        {
            return _liteDb.GetCollection<ItemModel>("Item").FindAll();
        }

        public ItemModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<ItemModel>("Item").FindOne(item => item.Id.Equals(id));
        }
        public Guid Insert(ItemModel item)
        {
            return _liteDb.GetCollection<ItemModel>("Item").Insert(item);
        }
        public bool Update(ItemModel item)
        {
            return _liteDb.GetCollection<ItemModel>("Item").Update(item);
        }
    }
}