using System;
using System.Collections.Generic;
using StorageDB.Models;
using LiteDB;

namespace StorageDB.Data
{
    public abstract class LiteDbBaseRepository<TModel> where TModel : BaseModel
    {
        private LiteDatabase _liteDb;

        public LiteDbBaseRepository(ILiteDbContext liteDbContext)
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

        public IEnumerable<TModel> FindAll()
        {
            return _liteDb.GetCollection<TModel>(ModelRepository).FindAll();
        }
        public TModel FindOne(Guid id)
        {
            return _liteDb.GetCollection<TModel>(ModelRepository)
                .FindOne(Client => Client.Id.Equals(id));
        }
        public Guid Insert(TModel model)
        {
            return _liteDb.GetCollection<TModel>(ModelRepository).Insert(model);
        }
        public bool UpdateOne(TModel model)
        {
            return _liteDb.GetCollection<TModel>(ModelRepository).Update(model);
        }
    }
}