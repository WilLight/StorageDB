using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbStorageRepository : LiteDbBaseRepository<StorageModel>
    {
        private LiteDatabase _liteDb;

        public LiteDbStorageRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }
    }
}