using System;
using System.Collections.Generic;
using StorageDB.Models;
using LiteDB;

namespace StorageDB.Data
{
    public abstract class LiteDbStoredRepository<TModel> : LiteDbBaseRepository<TModel> where TModel : BaseStoredModel
    {
        private LiteDatabase _liteDb;

        public LiteDbStoredRepository(ILiteDbContext liteDbContext) : base(liteDbContext)
        {
            _liteDb = liteDbContext.Database;
        }

        private string ModelRepository
        {
            get
            {
                return typeof(TModel).Name.Substring(0, typeof(TModel).Name.Length - 5);
            }
        }

        public IEnumerable<TModel> FindAllInStorage(Guid storageId)
        {
            return _liteDb.GetCollection<TModel>(ModelRepository).Find(x => x.StorageId == storageId);
        }

        public IEnumerable<TModel> FindAllWithCustomer(Guid clientId)
        {
            return _liteDb.GetCollection<TModel>(ModelRepository).Find(x => x.CustomerId == clientId);
        }
    }
}