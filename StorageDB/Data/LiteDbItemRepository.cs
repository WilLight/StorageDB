using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbItemRepository 
    {
        private LiteDatabase _liteDb;

        public LiteDbItemRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<ItemModel> FindAll()
        {
            return _liteDb.GetCollection<ItemModel>("Item").FindAll().OrderBy(item => item.Name);
        }
    }
}