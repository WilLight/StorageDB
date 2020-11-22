using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Data;
using StorageDB.Models;

namespace StorageDB.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StorageController : ControllerBase
    {
        private static readonly string[] Names = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<StorageController> _logger;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public StorageController(ILogger<StorageController> logger, ILiteDbStorageRepository dbStorageService)
        {
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<StorageModel> Get()
        {
            return _dbStorageService.FindAll().OrderBy(item => item.Name);
        }

        [HttpGet]
        public ActionResult<StorageModel> GetOne(Guid id)
        {
            var result = _dbStorageService.FindOne(id);
            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        public ActionResult<StorageModel> GenerateOne()
        {
            var rng = new Random();
            var item = new StorageModel();
            item.Id = Guid.NewGuid();
            item.Name = Names[rng.Next(Names.Length)];
            item.Capacity = rng.Next(170) * 5;
            var id = _dbStorageService.Insert(item);
            if (id != default)
                return CreatedAtAction("GetOne", _dbStorageService.FindOne(id));
            else
                return BadRequest();
        }
    }
}