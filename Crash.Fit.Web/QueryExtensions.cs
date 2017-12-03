using Crash.Fit.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web
{
    public static class QueryExtensions
    {
        public static decimal? GetUserWeight(this IMeasurementRepository measurementRepository, Guid userId)
        {
            return measurementRepository.GetMeasures(userId).FirstOrDefault(m => m.Id == Constants.Measurements.WeightId)?.LatestValue;
        }
    }
}
