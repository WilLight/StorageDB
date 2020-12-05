using System;
using System.Collections.Generic;
using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbStoredRepository<TModel> where TModel: IModelIndexed, IModelStored
    {
        IEnumerable<TModel> FindAllInStorage(Guid storageId);
    }
}