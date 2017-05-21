using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Measurements
{
    public interface IMeasurementRepository
    {
        IEnumerable<MeasureSummary> GetMeasures(Guid userId);
        bool CreateMeasure(Measure measure);
        bool CreateMeasurement(Measurement measurement);
        bool UpdateMeasurement(Measurement measurement);
        IEnumerable<Measurement> GetMeasurementHistory(Guid measurementId, Guid userId, DateTimeOffset start, DateTimeOffset end);
    }
}
