using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Models;
using StorageDB.Services;

namespace StorageDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StorageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly IStorageService _storageService;
        private readonly IValidationService _validationService;

        public StorageController(ILogger<StorageController> logger, IStorageService storageService, IValidationService validationService)
        {
            _storageService = storageService;
            _validationService = validationService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<StorageModel> Get()
        {
            return _storageService.GetAll().OrderBy(item => item.Name);
        }

        [HttpGet]
        public ActionResult<StorageModel> GetOne(Guid id)
        {
            var result = _storageService.GetOne(id);
            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<StorageModel> InsertOne(StorageModel storage)
        {
            if (_validationService.ValidateStorage(storage.Id))
                return BadRequest();
            
            var dto = _storageService.InsertOne(storage);
            if (dto != null)
                return CreatedAtAction("GetOne", dto);
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult<StorageModel> UpdateOne(StorageModel storage)
        {
            if(_storageService.UpdateOne(storage) != null)
                return Ok();
            else
                return BadRequest();
        }
    }
}