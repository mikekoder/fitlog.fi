<template>
  <div>
    <q-card>
      <q-card-title>
        <div class="row">
          <div class="col col-lg-2"><q-btn round  @click="changeDate(-1)"><q-icon name="fa-chevron-left" /></q-btn></div>
          <div class="col col-lg-2">
            <q-datetime v-model="selectedDate" type="date" :monday-first="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" :day-names="localDayNamesAbbr" :month-names="localMonthNames" @change="changeDate" @blur="datepickerVisible=false;" ref="datepicker" v-show="datepickerVisible" />
            <q-btn round :flat="true" @click="()=> {datepickerVisible=true; $refs.datepicker.open();}">{{ dateText }}</q-btn>
            </div>
          <div class="col col-lg-2"><q-btn round  @click="changeDate(1)"><q-icon name="fa-chevron-right" /></q-btn></div>
          <q-btn round class="pull-right" icon="fa-plus" @click="createWorkout"></q-btn>
        </div>
      </q-card-title>
    </q-card>
    <template v-for="(workout,index) in workouts">
      <q-card :key="index">
        <q-card-title>
          <q-datetime v-model="workout.time" type="time" :format24h="true" :no-clear="true" :ok-label="$t('ok')" :cancel-label="$t('cancel')" />
        </q-card-title>
        <q-card-separator />
        <q-card-main>
          <template v-for="(set,index_s) in workout.sets">
          <div class="row" :key="index_s">
            <div class="col-8">
              <q-search v-model="set.exerciseName" type="text" :float-label="$t('exercise')" placeholder="" icon="">
                <q-autocomplete @search="searchExercises" @selected="value => exerciseSelected(value,set)" :min-characters="1" :max-results="20" />
              </q-search>
            </div>
            <div class="col-2"><q-input v-model="set.reps" type="number" :float-label="$t('reps')" /></div>
            <div class="col-2"><q-input v-model="set.weights" type="number" :float-label="$t('weights')" /></div>
          </div>
          </template>
        </q-card-main>
        <q-card-actions>
          <q-btn round small @click="addSet(workout)"></q-btn>
        </q-card-actions>
      </q-card>
      </template>
  </div>
</template>

<script>
import { openURL } from 'quasar'
import moment from 'moment'
import constants from '../store/constants'
import formatters from '../formatters'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime, QSearch,QAutocomplete, QInput } from 'quasar'
import api from '../api'
export default {
  components: {
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem, QDatetime,QSearch,QAutocomplete, QInput
  },
  data () {
    return {
      datepickerVisible: false
    }
  },
  computed: {
    selectedDate() {
      return this.$store.state.training.diaryDate;
    },
    workouts(){
      var date = this.selectedDate;
      return this.$store.state.training.workouts.filter(w => moment(w.time).isSame(date, 'd'));
    },
    dateText() {
        if (moment().isSame(this.selectedDate, 'd')) {
            return this.$t('today');
        }
        else if (moment().subtract(1,'day').isSame(this.selectedDate, 'd')) {
            return this.$t('yesterday');
        }
        return this.date(this.selectedDate);
    },
    exercises(){
      return this.$store.state.training.exercises;
    }
  },
  methods: {
    createWorkout(){
      this.$store.dispatch(constants.START_WORKOUT, { time: new Date() });
    },
    addSet(workout){
      workout.sets.push({ exerciseId: undefined, exerciseName: undefined, reps: undefined, weights: undefined });
    },
    searchExercises(text, done){
      var textLower = text.toLowerCase();
      var self = this;
      var results = this.exercises.filter(e => e.name.toLowerCase().indexOf(textLower) >= 0).map(e => { return { ...e, value: e.name, label: e.name}});
      
      self.workouts.forEach(workout => {
        workout.sets.forEach(set => {
          if(!set.exerciseId && set.exerciseName.toLowerCase().indexOf(textLower) >= 0 && !results.find(r => r.name.toLowerCase() == set.exerciseName.toLowerCase())){
            results.push({name: set.exerciseName, value: set.exerciseName, label: set.exerciseName});
          }
        });
      });
      
      /*
      if(!results.find(r => r.name.toLowerCase() == textLower)){
        results.push({name: text, value: text, label: text});
      }*/
      // returning too fast -> every other char works
      setTimeout(() => { done(results);},50);
     
    },
    exerciseSelected(exercise, set){
      set.exerciseId = exercise.id;
      set.exerciseName = exercise.name;
    },
    changeDate(date) {
        this.datepickerVisible = false;
        var newDate;
        if (date == 'today') {
            newDate = new Date();
        }
        else if (date == 'yesterday') {
            newDate = moment().subtract(1, 'days').toDate();
        }
        else if (date === -1) {
            newDate = moment(this.selectedDate).subtract(1, 'days').toDate();
        }
        else if (date === 1){
            newDate = moment(this.selectedDate).add(1, 'days').toDate();
        }
        else {
            newDate = date;
        }
        if (!moment(newDate).isSame(this.selectedDate, 'd')) {
            this.$store.dispatch(constants.SELECT_WORKOUT_DIARY_DATE, { date: newDate });
            this.fetchWorkouts();
        }
    },
    fetchWorkouts() {
      var self = this;
      var start = moment(self.date).startOf('day');
      var end = moment(self.date).endOf('day');
      self.$store.dispatch(constants.FETCH_WORKOUTS, {
        start,
        end,
        success: function () {
          self.$store.commit(constants.LOADING_DONE);
        },
        failure: function () { }
      });
    },
    date: formatters.formatDate,
    time: formatters.formatTime,
    unit: formatters.formatUnit,
    decimal (value, precision) {
        if (!value) {
            return value;
        }
        return value.toFixed(precision);
    }
  },
  created(){
    var self = this;
    self.fetchWorkouts();

    self.$store.dispatch(constants.FETCH_EXERCISES, {
        success: function () { },
        failure: function () { }
    });
  }
}
</script>

<style lang="stylus">
.row.nutrients{ padding-bottom: 20px;}
</style>
