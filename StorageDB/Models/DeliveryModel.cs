using System;

namespace StorageDB.Models
{
    public class DeliveryModel : BaseStoredModel
    {
        public DateTime DeliveryDate { get; set; }
        public bool ToStorage { get; set; }
    }
}