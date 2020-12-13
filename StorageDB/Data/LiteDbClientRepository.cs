using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using StorageDB.Models;

namespace StorageDB.Data
{
    public class LiteDbClientRepository : ILiteDbClientRepository
    {
        private LiteDatabase _liteDb;

        public LiteDbClientRepository(ILiteDbContext liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        public IEnumerable<ClientModel> FindAll()
        {
            return _liteDb.GetCollection<ClientModel>("Client").FindAll();
        }

        public ClientModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<ClientModel>("Client")
                .FindOne(Client => Client.Id.Equals(id));
        }
        public Guid Insert(ClientModel item)
        {
            return _liteDb.GetCollection<ClientModel>("Client").Insert(item);
        }
        public bool UpdateOne(ClientModel item)
        {
            return _liteDb.GetCollection<ClientModel>("Client").Update(item);
        }
    }
}