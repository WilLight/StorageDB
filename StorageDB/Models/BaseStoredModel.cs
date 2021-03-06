using System;

namespace StorageDB.Models
{
    public abstract class BaseStoredModel : BaseModel
    {
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public Guid CustomerId { get; set; }
        public int Volume { get; set; }
    }
}