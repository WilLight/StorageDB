using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbItemRepository : LiteDbBaseRepository<ItemModel>
    {
        private LiteDatabase _liteDb;

        public LiteDbItemRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }
    }
}