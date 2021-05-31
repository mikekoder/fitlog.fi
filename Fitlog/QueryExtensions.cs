using Fitlog.Measurements;
using System;
using System.Linq;

namespace Fitlog.Web
{
    public static class QueryExtensions
    {
        public static decimal? GetUserWeight(this IMeasurementRepository measurementRepository, Guid userId)
        {
            return measurementRepository.GetMeasures(userId).FirstOrDefault(m => m.Id == Constants.Measurements.WeightId)?.LatestValue;
        }
    }
}
