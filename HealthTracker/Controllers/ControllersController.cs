using AutoMapper;
using HealthTracker.DAL.Entities;
using HealthTracker.DAL.Interfaces;
using HealthTracker.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ControllersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public ControllersController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        //PUT controllers/{deviceId}
        //Body { }
        [HttpPut("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ControllerDto>> CreateOrUpdateController(string deviceId)
        {
            var controller = _uow.Controllers.GetAll().FirstOrDefault(n => n.DeviceId == deviceId);

            if (controller != null)
            {
                controller.IsOnline = !controller.IsOnline;

                _uow.Controllers.Update(controller);
                await _uow.SaveAsync();

                return StatusCode(200, _mapper.Map<ControllerDto>(controller));
            }
            else
            {
                controller = new DAL.Entities.Controller() { DeviceId = deviceId, IsOnline = true };

                _uow.Controllers.Add(controller);

                foreach (var sens in RegisterSensorsForController(deviceId))
                {
                    _uow.Sensors.Add(sens);
                }

                await _uow.SaveAsync();

                return StatusCode(201, _mapper.Map<ControllerDto>(controller));
            }
        }

        //GET controllers/deviceId
        //Body {}
        [HttpGet("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ControllerDto> CheckControllerStatus(string deviceId)
        {
            var controller = _uow.Controllers.GetAll().FirstOrDefault(n => n.DeviceId == deviceId);

            if (controller != null)
            {
                return StatusCode(200, _mapper.Map<ControllerDto>(controller));
            }
            else
            {
                return StatusCode(404, new { Message = $"Controller with {deviceId} is not found" });
            }
        }

        [NonAction]
        private IEnumerable<Sensor> RegisterSensorsForController(string deviceid)
        {
            List<Sensor> sensors = new List<Sensor>();

            sensors.Add(new Sensor() { DeviceId = deviceid, SensorType = "temperature" });
            sensors.Add(new Sensor() { DeviceId = deviceid, SensorType = "location" });
            sensors.Add(new Sensor() { DeviceId = deviceid, SensorType = "fall" });
            sensors.Add(new Sensor() { DeviceId = deviceid, SensorType = "sos" });

            return sensors;
        }
    }
}
