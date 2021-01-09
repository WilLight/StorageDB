using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbCustomerRepository : ILiteDbCustomerRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbCustomerRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<CustomerModel> FindAll()
        {
            return _liteDb.GetCollection<CustomerModel>("Client").FindAll();
        }

        public CustomerModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<CustomerModel>("Client")
                .FindOne(Client => Client.Id.Equals(id));
        }
        public Guid Insert(CustomerModel item)
        {
            return _liteDb.GetCollection<CustomerModel>("Client").Insert(item);
        }
        public bool UpdateOne(CustomerModel item)
        {
            return _liteDb.GetCollection<CustomerModel>("Client").Update(item);
        }
    }
}