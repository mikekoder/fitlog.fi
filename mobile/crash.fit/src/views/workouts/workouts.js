import { openURL } from 'quasar'
import moment from 'moment'
import constants from '../../store/constants'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime, QSearch,QAutocomplete, QInput, QTooltip, QBtnDropdown} from 'quasar'
import api from '../../api'
import exercisesMixin from '../../mixins/exercises'
import utils from '../../utils'

export default {
  components: {
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem, QDatetime,QSearch,QAutocomplete, QInput,  QTooltip, QBtnDropdown
  },
  mixins: [exercisesMixin],
  data() {
      return {
          startDatepickerVisible: false,
          endDatepickerVisible: false,
          tab: 'workouts',
          progress: [],
          tooltipsOpen: true
      }
  },
  computed: {
      muscleGroups() {
          return this.$store.state.training.muscleGroups;
      },
      workouts() {
          var self = this;
          return this.$store.state.training.workouts.filter(w => moment(w.time).isBetween(self.start, self.end)).sort(function (a, b) {
            if (a.time.getTime() < b.time.getTime())
                return 1;
            if (a.time.getTime() > b.time.getTime())
                return -1;
            return 0;
        }).map(w => { return { ...w, exercises: [...new Set(w.sets.map(e => e.exerciseName))]}});
      },
      activeRoutine() {
          return this.$store.state.training.activeRoutine;
      },
      start() {
          var self = this;
          return this.$store.state.training.workoutsDisplayStart;
      },
      end() {
          var self = this;
          return this.$store.state.training.workoutsDisplayEnd;
      }
  },
  methods: {
    changeStart(date) {
        this.showDateRange(date, this.end);
    },
    changeEnd(date) {
        this.showDateRange(this.start, date);
    },
      showWeek() {
          var end = moment().endOf('day').toDate();
          var start = moment().startOf('isoWeek').toDate();
          this.showDateRange(start, end);
      },
      showMonth() {
          var end = moment().endOf('day').toDate();
          var start = moment().startOf('month').toDate();
          this.showDateRange(start, end);
      },
      showDays(days) {
          var end = moment().endOf('day').toDate();
          var start = moment().subtract(days - 1, 'days').startOf('day').toDate();
          this.showDateRange(start, end);
      },
      showDateRange(start, end) {
          var self = this;
          self.progress = [];
          self.$store.dispatch(constants.SELECT_WORKOUT_DATE_RANGE, {
              start: start,
              end: end,
              success() {
                  self.fetchWorkouts();
              }
          });
      },
      fetchWorkouts() {
          var self = this;
          this.$store.dispatch(constants.FETCH_WORKOUTS, {
              start: self.start,
              end: self.end,
              success() {
                  self.$store.commit(constants.LOADING_DONE);
              }
          });
      },
      showWorkout(workout){
        this.$router.push({ name: 'workout-details', params: { id: workout.id } });
      },
      createWorkout(routineId, workoutId) {
          if (routineId && workoutId) {
              this.$router.push({ name: 'workout-details', params: { id: constants.NEW_ID }, query: { [constants.ROUTINE_PARAM]: routineId, [constants.WORKOUT_PARAM]: workoutId } });
          }
          else {
              this.$router.push({ name: 'workout-details', params: { id: constants.NEW_ID } });
          }
      },
      deleteWorkout(workout) {
          var self = this;
          self.$store.dispatch(constants.DELETE_WORKOUT, {
              workout,
              success() { },
              failure() {
                  toaster(self.$t('deleteFailed'));
              }
          });
      },
      changeStart(date) {
          this.showDateRange(date, this.end);
      },
      changeEnd(date) {
          this.showDateRange(this.start, date);
      },
      calculateProgress() {
          var self = this;
          self.progress = [];
          var goals = [];
          var sets = [];
          if (self.activeRoutine) {
              var weeks = moment(self.end).diff(moment(self.start), 'weeks', true);
              self.activeRoutine.workouts.forEach(w =>
              {
                  w.exercises.forEach(e =>
                  {
                      var exercise = self.$exercises.find(e2 => e2.id == e.exerciseId);
                      var loadFrom = e.loadFrom || e.loadFrom == 0 ? e.loadFrom : e.loadTo;
                      var loadTo = e.loadTo || e.loadTo == 0 ? e.loadTo : e.loadFrom;
                      var step = (loadFrom || loadFrom == 0) && (loadTo || loadTo == 0) ? (loadTo - loadFrom) / (e.sets - 1) : undefined;

                      if (loadFrom == loadTo) {
                          goals.push({
                              index: goals.length,
                              exerciseId: e.exerciseId,
                              exercise,
                              reps: e.reps,
                              load: loadFrom,
                              times: utils.roundToNearest(e.sets * w.frequency * weeks, 0.1),
                              sets: [],
                              expanded: false
                          });
                      }
                      else {
                          for (var i = 0; i < e.sets; i++) {
                              goals.push({
                                  index: goals.length,
                                  exerciseId: e.exerciseId,
                                  exercise,
                                  reps: e.reps,
                                  load: (step || step == 0) ? loadFrom + i * step : undefined,
                                  times: utils.roundToNearest(w.frequency * weeks, 0.1),
                                  sets: [],
                                  expanded: false
                              });
                          }
                      }
                  });
              });

              goals.sort((a, b) => b.load - a.load);

              var leftOverSets = [];
              var loadMargin = 2.5;
              self.workouts.forEach(w =>
              {
                  w.sets.forEach(s =>
                  {
                      var set = {
                          exerciseId: s.exerciseId,
                          reps: s.reps,
                          weights: s.weights,
                          load: s.load,
                          loadBW: s.loadBW
                      };
                      var candidates = goals.filter(g => g.exerciseId == s.exerciseId && g.reps <= s.reps && g.load <= (s.load + loadMargin) && g.sets.length < g.times).sort((a,b) => b.load - a.load);
                      if (candidates.length > 0) {
                          candidates[0].sets.push(set);
                      }
                      else {
                          leftOverSets.push(set);
                      }
                  });
              });

              
          }
          self.progress = goals.sort((a,b) => a.index - b.index);
      },
      showTooltips(){
          var self = this;
          setTimeout(() => {
            self.$refs.tooltip1.open();
          },200);
      },
      hideTooltips(){
        var self = this;
        self.$refs.tooltip1.close();
    }
  },
  watch: {
      activeRoutine() {
          this.calculateProgress();
          
      },
      workouts() {
          this.calculateProgress();
      }
  },
  created() {

      var self = this;
      this.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {
          success() { },
          failure() { }
      });
      this.$store.dispatch(constants.FETCH_ROUTINES, {
          success() { },
          failure() { }
      });
      if (self.start && self.end) {
          self.fetchWorkouts();
      }
      else {
          self.showDays(7);
      }
      
  }
}
