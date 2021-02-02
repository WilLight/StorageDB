using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using StorageDB.Models;
using StorageDB.Services;

namespace StorageDBTests
{
    public class ValidationService_CheckStorageOverflow
    {
        public ValidationService InitializeValidationService()
        {
            var customerService = new Mock<ICustomerService>();
            var itemService = new Mock<IItemService>();
            var orderService = new Mock<IOrderService>();
            var storageService = new Mock<IStorageService>();
            ValidationService validationService = new ValidationService(customerService.Object, itemService.Object, orderService.Object, storageService.Object);
            return validationService;
        }

        [Fact]
        public void CheckStorageOverflow_OneOrder_OverflowTrue()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(5), 12));

            int storageVolume = 10;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.True(result);
        }

        [Fact]
        public void CheckStorageOverflow_TwoOrders_OverflowTrue()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(5), 8));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), 8));

            int storageVolume = 10;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.True(result);
        }

        [Fact]
        public void CheckStorageOverflow_TwoOrders_OverflowFalse()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(5), 5));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(1), DateTime.Now.AddDays(3), 5));

            int storageVolume = 10;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.False(result);
        }

        [Fact]
        public void CheckStorageOverflow_ThreeOrders_OneRecursion_OverflowFalse()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(7), 5));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(3), 4));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), 4));

            int storageVolume = 10;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.False(result);
        }

        [Fact]
        public void CheckStorageOverflow_ThreeOrders_OneRecursion_OverflowTrue()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(7), 5));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(3), 4));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), 8));

            int storageVolume = 10;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.True(result);
        }
        [Fact]
        public void CheckStorageOverflow_FourOrders_TwoRecursions_OverflowFalse()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(7), 5));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(3), 4));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(2), 4));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), 8));

            int storageVolume = 20;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.False(result);
        }

        [Fact]
        public void CheckStorageOverflow_FourOrders_TwoRecursions_OverflowTrue()
        {
            ValidationService validationService = InitializeValidationService();
            
            List<OrderValidationModel> orders = new List<OrderValidationModel>();
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(7), 5));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(3), 8));
            orders.Add(new OrderValidationModel(DateTime.Now, DateTime.Now.AddDays(2), 8));
            orders.Add(new OrderValidationModel(DateTime.Now.AddDays(4), DateTime.Now.AddDays(7), 8));

            int storageVolume = 20;

            var result = validationService.CheckStorageOverflow(orders, storageVolume);

            Assert.True(result);
        }
    }
}
