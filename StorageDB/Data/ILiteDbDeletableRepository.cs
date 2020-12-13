using System;
using System.Collections.Generic;
using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbDeletableRepository<TModel> where TModel : IModelDeletable
    {
         bool DeleteOne(Guid id);
    }
}