using System;

namespace StorageDB.Models
{
    public class ReservationModel : IModelIndexed, IModelStored
    {
        public Guid Id { get; set; }
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Volume { get; set; }
    }
}