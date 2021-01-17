using System;
using Xunit;
using StorageDB.Models;
using StorageDB.Services;

namespace StorageDBTests
{
    public class ValidationService_ValidateSizeShould
    {
        [Fact]
        public void ValidateSize_Reservation_ReturnTrue()
        {
            ReservationModel reservation = new ReservationModel();
            reservation.Volume = 10;
            
        }
    }
}
