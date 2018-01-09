using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Activities
{
    public interface IActivityRepository
    {
        IEnumerable<Activity> GetActivities();
        Activity GetActivity(Guid id);
        void CreateActivity(Activity activity);
        void UpdateActivity(Activity activity);
        void DeleteActivity(Activity activity);
        IEnumerable<EnergyExpenditure> GetEnergyExpenditures(Guid userId, DateTimeOffset start, DateTimeOffset dateTimeOffset);
        void CreateEnergyExpenditure(EnergyExpenditure expenditure);
        EnergyExpenditure GetEnergyExpenditure(Guid id);
        void UpdateEnergyExpenditure(EnergyExpenditure expenditure);
        void DeleteEnergyExpenditure(EnergyExpenditure expenditure);
        EnergyExpenditure GetEnergyExpenditureForWorkout(Guid workoutId);

        IEnumerable<ActivityPreset> GetActivityPresets(Guid userId);
        void SaveActivityPresets(IEnumerable<ActivityPreset> presets);
    }
}
