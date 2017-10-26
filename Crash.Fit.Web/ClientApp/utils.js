import moment from 'moment'
export default {
    parseFloat(value) {
        if (typeof (value) === 'number') {
            return value;
        }
        return parseFloat((value || '0').replace(',', '.'));
    },
    roundToNearest(value, step) {
        var inv = 1.0 / step;
        return Math.round(value * inv) / inv;
    },
    previousHalfHour(value) {
        if (!value) {
            value = new Date();
        }
        var minutes = value.getMinutes();

        value = new Date(value.setMinutes(minutes - (minutes % 30)));
        value = new Date(value.setSeconds(0));
        value = new Date(value.setMilliseconds(0));
        return value;
    },
    nutrientGoal(nutritionGoal, allWorkouts, nutrientId, day, meal) {
        if (!nutritionGoal || !nutritionGoal.periods || nutritionGoal.periods.length == 0) {
            return { min: undefined, max: undefined };
        }
        var goals;
        if (meal) {
            goals = nutritionGoal.periods.filter(g => !g.wholeDay && (g.mealDefinitions == null || g.mealDefinitions.length == 0 || g.mealDefinitions.includes(meal.definitionId)) && g.nutrients.find(v => v.nutrientId === nutrientId));
        }
        else {
            goals = nutritionGoal.periods.filter(g => g.wholeDay && g.nutrients.find(v => v.nutrientId === nutrientId));
        }

        // exercise/rest day
        var start = moment(day).startOf('day');
        var end = moment(day).endOf('day');
        var workouts = allWorkouts.filter(w => moment(w.time).isBetween(start, end));
        var hasWorkout = workouts.length > 0;

        var goal = goals.find(g => (hasWorkout && g.exerciseDay) || (!hasWorkout && g.restday));

        // weekday
        if (!goal) {
            var dayNumber = day.getDay();
            switch (day.getDay()) {
                case 0:
                    goal = goals.find(g => g.sunday);
                    break;
                case 1:
                    goal = goals.find(g => g.monday);
                    break;
                case 2:
                    goal = goals.find(g => g.tuesday);
                    break;
                case 3:
                    goal = goals.find(g => g.wednesday);
                    break;
                case 4:
                    goal = goals.find(g => g.thursday);
                    break;
                case 5:
                    goal = goals.find(g => g.friday);
                    break;
                case 6:
                    goal = goals.find(g => g.saturday);
                    break;
            }
        }

        // default
        if (!goal) {
            goal = goals.find(g => !g.monday && !g.tuesday && !g.wednesday && !g.thursday && !g.friday && !g.saturday && !g.sunday && !g.exerciseDay && !g.restday);
        }

        if (goal) {
            var values = goal.nutrients.find(n => n.nutrientId === nutrientId);
            if (values) {
                return values;
            }

        }

        return { min: undefined, max: undefined };
    }
}
