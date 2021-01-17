using System;

namespace StorageDB.Models
{
    public class OrderValidationModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Volume { get; set; }
    }
}