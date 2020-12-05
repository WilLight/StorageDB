using System;

namespace StorageDB.Models
{
    public interface IModelStored
    {
         Guid StorageId { get; set; }
    }
}