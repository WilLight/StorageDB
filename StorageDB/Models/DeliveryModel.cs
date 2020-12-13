using System;

namespace StorageDB.Models
{
    public class DeliveryModel : IModelIndexed, IModelStored, IModelDeletable
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool ToStorage { get; set; }
    }
}