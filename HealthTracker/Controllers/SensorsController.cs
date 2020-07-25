using AutoMapper;
using HealthTracker.DAL.Interfaces;
using HealthTracker.Dto;
using HealthTracker.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SensorsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public SensorsController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        //POST sensors/{deviceId = 1}
        //Body [{ "SensorType" : string, "CurrentValue" : object, TimeStamp : DateTime }, { }]
        [HttpPost("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GetSensorDto>>> AddSensors(string deviceId, IEnumerable<PostSensorDto> addedSensors)
        {
            var controller = _uow.Controllers.GetAll()
                .FirstOrDefault(n => n.DeviceId == deviceId);

            if (controller != null)
            {
                if (!controller.IsOnline)
                {
                    return StatusCode(400, new { Message = "Controller is offline" });
                }

                List<GetSensorDto> responseSensors = new List<GetSensorDto>();

                foreach (var sensor in addedSensors)
                {

                    var concreteSensor = SensorBuilder.Build(deviceId, sensor.SensorType);

                    if (concreteSensor == null)
                    {
                        return StatusCode(400, new { Message = $"SensorType: {sensor.SensorType} is unsupported" });
                    }

                    try
                    {
                        concreteSensor.SetValue(sensor.CurrentValue);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(400, new { ex.Message });
                    }

                    var sensorPostgreId = _uow.Sensors
                       .GetAll()
                       .FirstOrDefault(n => n.DeviceId == deviceId && n.SensorType == sensor.SensorType).Id;

                    concreteSensor.PostgreSQLId = sensorPostgreId;
                    concreteSensor.TimeStamp = sensor.TimeStamp.ToUniversalTime();

                    if (concreteSensor.TimeStamp.ToUnixTime() <= 0)
                    {
                        return StatusCode(400, new { Message = "Wrong date time: " + sensor.TimeStamp });
                    }

                    await concreteSensor.Save();

                    var sensorDto = _mapper.Map<GetSensorDto>(concreteSensor);
                    sensorDto.CurrentValue = sensor.CurrentValue;

                    responseSensors.Add(sensorDto);
                }

                return StatusCode(201, responseSensors);
            }
            else
                return StatusCode(404, new { Message = $"Controller with {deviceId} is not found" });
        }

        //GET sensors/current/{sensortype}
        //Body {}
        [HttpGet("current/{sensortype}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GetSensorDto>>> GetAllCurrentSensorsDataBySensorType(string sensortype)
        {
            sensortype = sensortype.ToLower();

            if (_uow.Sensors.GetAll().FirstOrDefault(n => n.SensorType == sensortype) == null)
            {
                return StatusCode(404, new { Message = $"Sensor type \'{sensortype}\' not founded" });
            }

            IEnumerable<string> onlineControllersDeviceIds = _uow.Controllers.GetAll()
                .Where(n => n.IsOnline)
                .Select(d => d.DeviceId);

            List<GetSensorDto> responseSensors = new List<GetSensorDto>();

            foreach (string id in onlineControllersDeviceIds)
            {
                var sensorEntities = _uow.Sensors.GetAll().Where(n => n.DeviceId == id && n.SensorType == sensortype);

                foreach (var sensor in sensorEntities)
                {
                    var concreteSensor = SensorBuilder.Build(sensor.DeviceId, sensor.SensorType);

                    concreteSensor.PostgreSQLId = sensor.Id;

                    var currentValue = await concreteSensor.GetValue(1);

                    if (currentValue != null)
                    {
                        var sensorDto = _mapper.Map<GetSensorDto>(concreteSensor);
                        sensorDto.CurrentValue = currentValue;

                        responseSensors.Add(sensorDto);
                    }
                }
            }

            return StatusCode(200, responseSensors);
        }

        //GET sensors/current/all
        //Body {}
        [HttpGet("current/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetSensorDto>>> GetAllCurrentSensorsData()
        {
            IEnumerable<string> onlineControllersDeviceIds = _uow.Controllers.GetAll()
                .Where(n => n.IsOnline)
                .Select(d => d.DeviceId);

            List<GetSensorDto> responseSensors = new List<GetSensorDto>();

            foreach (string id in onlineControllersDeviceIds)
            {
                var sensorEntities = _uow.Sensors.GetAll().Where(n => n.DeviceId == id);

                foreach (var sensor in sensorEntities)
                {
                    var concreteSensor = SensorBuilder.Build(sensor.DeviceId, sensor.SensorType);
                    concreteSensor.PostgreSQLId = sensor.Id;

                    var currentValue = await concreteSensor.GetValue(1);

                    if (currentValue != null)
                    {
                        var sensorDto = _mapper.Map<GetSensorDto>(concreteSensor);
                        sensorDto.CurrentValue = currentValue;

                        responseSensors.Add(sensorDto);
                    }
                }
            }

            return StatusCode(200, responseSensors);
        }

        //GET sensors/current/all/{deviceId = 1}
        //Body {}
        [HttpGet("current/all/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GetSensorDto>>> GetAllCurrentSensorsDataByDeviceId(string deviceId)
        {
            var controller = _uow.Controllers.GetAll()
                .FirstOrDefault(n => n.DeviceId == deviceId);

            if (controller != null)
            {

                if (!controller.IsOnline)
                {
                    return StatusCode(400, new { Message = "Controller is offline" });
                }

                List<GetSensorDto> responseSensors = new List<GetSensorDto>();

                var sensorEntities = _uow.Sensors.GetAll().Where(n => n.DeviceId == controller.DeviceId);

                foreach (var sensor in sensorEntities)
                {
                    var concreteSensor = SensorBuilder.Build(sensor.DeviceId, sensor.SensorType);
                    concreteSensor.PostgreSQLId = sensor.Id;

                    var currentValue = await concreteSensor.GetValue(1);

                    if (currentValue != null)
                    {
                        var sensorDto = _mapper.Map<GetSensorDto>(concreteSensor);
                        sensorDto.CurrentValue = currentValue;

                        responseSensors.Add(sensorDto);
                    }
                }

                return StatusCode(200, responseSensors);
            }
            return StatusCode(404, new { Message = $"Controller with {deviceId} is not found" });
        }

        //GET sensors/{sensortype}/{count}/{deviceId}
        //Body {}
        [HttpGet("{sensortype}/{count}/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetSensorDto>>> GetSensorDataByDeviceIdAndSensorType(string sensortype, int count, string deviceId)
        {
            sensortype = sensortype.ToLower();

            if (_uow.Sensors.GetAll().FirstOrDefault(n => n.SensorType == sensortype) == null)
            {
                return StatusCode(404, new { Message = $"Sensor type \'{sensortype}\' not founded" });
            }

            var controller = _uow.Controllers.GetAll()
                .FirstOrDefault(n => n.DeviceId == deviceId);

            if (controller != null)
            {

                List<GetSensorDto> responseSensors = new List<GetSensorDto>();

                var postgreId = _uow.Sensors
                   .GetAll()
                   .FirstOrDefault(n => n.DeviceId == controller.DeviceId && n.SensorType == sensortype).Id;

                var concreteSensor = SensorBuilder.Build(controller.DeviceId, sensortype);
                concreteSensor.PostgreSQLId = postgreId;

                while (count != 0)
                {
                    var currentValue = await concreteSensor.GetValue(count);

                    if (currentValue != null)
                    {
                        var sensorDto = _mapper.Map<GetSensorDto>(concreteSensor);
                        sensorDto.CurrentValue = currentValue;

                        responseSensors.Add(sensorDto);
                    }

                    count--;
                }

                return StatusCode(200, responseSensors);
            }
            return StatusCode(404, new { Message = $"Controller with {deviceId} is not found" });
        }

        //GET sensors/alerts/all
        //Body {}
        [HttpGet("alerts/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetDeviceIdsWithAlert()
        {
            List<string> Ids = new List<string>();

            var controllers = _uow.Controllers.GetAllIncluded()
                .Where(d => d.IsOnline);

            foreach (var controller in controllers)
            {
                foreach (var sensor in controller.Sensors)
                {
                    var concreteSensor = SensorBuilder.Build(sensor.DeviceId, sensor.SensorType);

                    concreteSensor.PostgreSQLId = sensor.Id;

                    await concreteSensor.GetValue(1);

                    if (concreteSensor.AlertState)
                    {
                        Ids.Add(concreteSensor.DeviceId);
                        break;
                    }
                }
            }

            return StatusCode(200, Ids);
        }
    }
}

