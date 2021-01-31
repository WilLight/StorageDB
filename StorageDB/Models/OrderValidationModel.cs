using System;

namespace StorageDB.Models
{
    public class OrderValidationModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Volume { get; set; }

        public OrderValidationModel()
        {

        }

        public OrderValidationModel(DateTime startDate, DateTime endDate, int volume)
        {
            StartDate = startDate;
            EndDate = endDate;
            Volume = volume;
        }
    }
}