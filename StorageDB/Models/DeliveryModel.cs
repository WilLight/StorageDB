using System;

namespace StorageDB.Models
{
    public class DeliveryModel : IModelIndexed, IModelStored
    {
        public Guid Id { get; set; }
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public Guid ClientId { get; set; }
        public int Volume { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool ToStorage { get; set; }
    }
}