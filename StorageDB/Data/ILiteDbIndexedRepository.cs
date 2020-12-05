using System;
using System.Collections.Generic;
using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbIndexedRepository<TModel> where TModel: IModelIndexed
    {
        IEnumerable<TModel> FindAll();
        TModel FindOne(Guid id);
        Guid Insert(TModel model);
        bool Update(TModel model);
    }
}