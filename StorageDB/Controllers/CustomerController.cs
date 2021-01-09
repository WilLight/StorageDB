using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Services;
using StorageDB.Models;

namespace StorageDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IValidationService _validationService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService, IValidationService validationService)
        {
            _customerService = customerService;
            _validationService = validationService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CustomerModel> Get()
        {
            return _customerService.GetAll();
        }

        [HttpGet]
        public ActionResult<CustomerModel> GetOne(Guid id)
        {
            if (!_validationService.ValidateCustomer(id))
                return Ok(_customerService.GetOne(id));
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<CustomerModel> InsertOne(CustomerModel item)
        {
            if (!_validationService.ValidateCustomer(item.Id))
                item.Id = Guid.NewGuid();
            else
                return BadRequest();
            var dto = _customerService.InsertOne(item);
            if (dto != default)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<CustomerModel> UpdateOne(CustomerModel item)
        {
            if (_customerService.UpdateOne(item) != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}