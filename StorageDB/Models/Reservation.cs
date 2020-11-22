using System;

namespace StorageDB.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}