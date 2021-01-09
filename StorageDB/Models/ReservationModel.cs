using System;

namespace StorageDB.Models
{
    public class ReservationModel : IModelIndexed, IModelStored, IModelDeletable
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public Guid StorageId { get; set; }
        public Guid ItemId { get; set; }
        public Guid ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Volume { get; set; }
    }
}