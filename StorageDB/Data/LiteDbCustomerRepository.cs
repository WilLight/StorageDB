using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbCustomerRepository : LiteDbBaseRepository<CustomerModel>
    {
        private LiteDatabase _liteDb;

        public LiteDbCustomerRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }
    }
}