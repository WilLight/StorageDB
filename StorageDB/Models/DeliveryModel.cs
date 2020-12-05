using System;

namespace StorageDB.Models
{
    public class DeliveryModel : IModelIndexed, IModelStored
    {
        public Guid Id { get; set; }
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public int ItemCount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool ToStorage { get; set; }
    }
}