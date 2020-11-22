using System;

namespace StorageDB.Models
{
    public class ReservationModel : IModelIndexed
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}