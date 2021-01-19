using System;

namespace StorageDB.Models
{
    public class ReservationModel : BaseStoredModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}