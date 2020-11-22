using System;

namespace StorageDB.Models
{
    public interface IModelIndexed
    {
        Guid Id { get; set; }
    }
}