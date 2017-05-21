using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Web.Models.Nutrition;
using Microsoft.Extensions.Logging;
using Crash.Fit.Logging;
using Crash.Fit.Measurements;
using Crash.Fit.Web.Models.Measurements;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MeasurementsController : ApiControllerBase
    {
        private readonly IMeasurementRepository measurementRepository;
        public MeasurementsController(IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("measures")]
        public IActionResult ListMeasures()
        {
            var measures = measurementRepository.GetMeasures(CurrentUserId);

            var response = AutoMapper.Mapper.Map<MeasureSummaryResponse[]>(measures);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]MeasurementRequest request)
        {
            CreateMeasures(request.Measurements);
            foreach(var reqMeasurement in request.Measurements)
            {
                var measurement = new Measurement
                {
                    MeasureId = reqMeasurement.MeasureId.Value,
                    Time = request.Time,
                    Value = reqMeasurement.Value
                };
                if (reqMeasurement.MeasurementId.HasValue)
                {
                    measurement.Id = reqMeasurement.MeasurementId.Value;
                    measurementRepository.UpdateMeasurement(measurement);
                }
                else
                {
                    measurementRepository.CreateMeasurement(measurement);
                }
            }
            return Ok();
        }
        /*
        [HttpGet]
        [Route("daily-intakes")]
        public IEnumerable<DailyIntake> DailyIntakes(Gender gender, DateTime dateOfBirth)
        {
            var age = DateTime.Now - dateOfBirth;
            return nutritionRepository.GetDailyIntakes(gender, age);
        }
        */
        private void CreateMeasures(IEnumerable<MeasurementRequest.Measurement> measurements)
        {
            if (!measurements.Any(m => m.MeasureId == null))
            {
                return;
            }
            var measures = new List<Measure>();
            measures.AddRange(measurementRepository.GetMeasures(CurrentUserId));
            foreach (var measurement in measurements.Where(m => m.MeasureId == null && !string.IsNullOrWhiteSpace(m.MeasureName)))
            {
                var measure = measures.FirstOrDefault(e => e.Name.Equals(measurement.MeasureName, StringComparison.CurrentCultureIgnoreCase));
                if (measure != null)
                {
                    measurement.MeasureId = measure.Id;
                }
                else
                {
                    var newMeasure = new Measure
                    {
                        UserId = CurrentUserId,
                        Name = char.ToUpper(measurement.MeasureName[0]) + measurement.MeasureName.Substring(1).ToLower()
                    };
                    measurementRepository.CreateMeasure(newMeasure);
                    measures.Add(newMeasure);
                    measurement.MeasureId = newMeasure.Id;
                }
            }
        }
    }
}