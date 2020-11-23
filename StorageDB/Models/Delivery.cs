using System;

namespace StorageDB.Models
{
    public class Delivery
    {
        Guid Id { get; set; }
        Guid StorageId { get; set; }
        Guid ItemId { get; set; }
        int ItemCount { get; set; }
        DateTime DeliveryDate { get; set; }
        bool ToStorage { get; set; }
    }
}