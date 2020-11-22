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
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly ILiteDbItemRepository _dbItemService;
        private readonly ILiteDbReservationRepository _dbReservationService;
        private readonly ILiteDbStorageRepository _dbStorageService;

        public ReservationController(ILogger<ReservationController> logger, ILiteDbItemRepository dbItemService, ILiteDbReservationRepository dbReservationService, ILiteDbStorageRepository dbStorageService)
        {
            _dbItemService = dbItemService;
            _dbReservationService = dbReservationService;
            _dbStorageService = dbStorageService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ReservationModel> Get()
        {
            return _dbReservationService.FindAll().OrderBy(item => item.Id);
        }

        [HttpGet]
        public ActionResult<ReservationModel> GetOne(Guid id)
        {
            var result = _dbReservationService.FindOne(id);
            if (result != default)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet]
        public ActionResult<ReservationModel> GenerateOne()
        {
            var rng = new Random();
            var reservation = new ReservationModel();
            reservation.Id = Guid.NewGuid();
            reservation.StorageId = _dbStorageService.FindAll().First<StorageModel>().Id;
            if (reservation.StorageId == default)
                return BadRequest();
            reservation.StartDate = DateTime.Today.AddDays(rng.Next(5));
            reservation.EndDate = reservation.StartDate.AddDays(rng.Next(5));
            var id = _dbReservationService.Insert(reservation);
            if (id != default)
                return CreatedAtAction("GetOne", _dbReservationService.FindOne(id));
            else
                return BadRequest();
        }
    }
}