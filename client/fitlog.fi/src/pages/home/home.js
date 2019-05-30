import moment from 'moment'
import constants from '../../store/constants'
import EnergyDistributionBar from '../../components/energy-distribution-bar'
import NutrientBar from '../../components/nutrient-bar'
import MealRowEditor from '../../components/meal-row-editor'
import MealSettings from './meal-settings'
import utils from '../../utils'
import nutrientsMixin from '../../mixins/nutrients'
import mealDefinitionsMixin from '../../mixins/meal-definitions'
import nutritionGoalMixin from '../../mixins/nutrition-goal'
import activityPresetsMixin from '../../mixins/activity-presets'
import NutrientKnob from '../../components/nutrient-knob'
import Help from './home-help.vue'
import PageMixin from '../../mixins/page'

export default {
  mixins: [nutrientsMixin, mealDefinitionsMixin, nutritionGoalMixin, activityPresetsMixin, PageMixin],
  components: {
    EnergyDistributionBar,
    NutrientBar,
    MealRowEditor,
    MealSettings,
    NutrientKnob,
    'home-help': Help
  },
  data () {
    return {
      proteinId: constants.PROTEIN_ID,
      carbId: constants.CARB_ID,
      fatId: constants.FAT_ID,
      energyId: constants.ENERGY_ID,
      energyDistributionId: constants.ENERGY_DISTRIBUTION_ID,
      energyDifferenceId: constants.ENERGY_DIFFERENCE_ID,
      selectedRow: undefined,
      datepickerVisible: false,
      eatenEnergy: undefined,
      rightMenuOpen: true,
      newMealTime: undefined
    }
  },
  computed: {
    selectedDate() {
      return this.$store.state.nutrition.diaryDate;
    },
    dateText() {
        if (moment().isSame(this.selectedDate, 'd')) {
          return this.$t('today');
        }
        else if (moment().subtract(1,'day').isSame(this.selectedDate, 'd')) {
          return this.$t('yesterday');
        }
        return this.formatDate(this.selectedDate);
    },
    visibleNutrients() {
      return this.$store.state.nutrition.nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1,n2) => n1.homeOrder - n2.homeOrder);
    },
    mealDay() {
      var date = moment(this.selectedDate).startOf('day');     
      return this.$store.state.nutrition.mealDays.find(d => moment(d.date).isSame(date, 'day'));
    },
    meals() {
      var start = moment(this.selectedDate).startOf('day');
      var end = moment(this.selectedDate).endOf('day');
      var defs = this.$store.state.nutrition.mealDefinitions;
      var meals = this.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(start, end));
      var result = defs.map(d => { return { definition: d, meal: meals.find(m => m.definitionId == d.id) } });
      meals.filter(m => !m.definitionId).forEach(m => {
        var index = result.findIndex(r => r.definition && r.definition.startHour && r.definition.startHour > m.time.getHours());
        if (index == -1) {
          result.push({ meal: m });
        }
        else
          result.splice(index, 0, { meal: m });
        }
      );
      return result;
    },
    workouts() {
      return this.$store.state.training.workouts.filter(w => moment(w.time).isSame(this.selectedDate, 'day'));
    },
    nutritionGoal() {
      return this.$store.state.nutrition.activeNutritionGoal;
    },
    dayNutrients() {
        var day = this.$store.state.nutrition.mealDays.find(d => moment(d.date).isSame(this.selectedDate, 'day'));
        if (day) {
            return day.nutrients;
        }
        return undefined;
    },
    nutrients() {
      var result = {};
      this.meals.filter(m => m.meal).forEach(m => {
          for (var i in m.meal.nutrients) {
              if (!result[i]) {
                  result[i] = 0;
              }
              result[i] += m.meal.nutrients[i];
          }
      });
      utils.calculateEnergyDistribution(result);
      return result;
    },
    nutrientGroups() {
        var nutrients = this.$store.state.nutrition.nutrients;
        return this.$store.state.nutrition.nutrientGroups.map(g => {
            return {
                name: g.name,
                nutrients: nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1)
            }
        });
        return 
    },
    mealCopy() {
        if (this.$store.state.clipboard.type == constants.MEAL) {
            return this.$store.state.clipboard.data;
        }
        return undefined;
    },
    rowCopy() {
        if (this.$store.state.clipboard.type == constants.MEAL_ROWS) {
            return this.$store.state.clipboard.data;
        }
        return undefined;
    },
    rmr() {
        if (this.$profile) {
            return this.$profile.rmr;
        }
        return null;
    },
    activityLevelEnergy() {
        if (this.activityPreset && this.rmr) {
            return (this.activityPreset.factor - 1) * this.rmr;
        }
        return null;
    },
    energyExpenditure() {
      var expenditures = this.$store.state.training.energyExpenditures.filter(e => moment(e.time).isSame(this.selectedDate, 'day'));
      var sum = 0;
      expenditures.forEach(e => {
          sum += e.energyKcal;
      });
      return sum;
    },
    totalEnergy() {
        return this.eatenEnergy - this.rmr - this.activityLevelEnergy - this.energyExpenditure;
    },
    energyGoal() {
        return this.nutrientGoal(this.energyDifferenceId);
    },
    totalClass() {
        if (this.totalEnergy && this.energyGoal) {
            if (this.energyGoal.min && this.totalEnergy < this.energyGoal.min) {
                return 'bg-danger';
            }
            if (this.energyGoal.max && this.totalEnergy > this.energyGoal.max) {
                return 'bg-danger';
            }
            if (this.energyGoal.min || this.energyGoal.min == 0 || this.energyGoal.max || this.energyGoal.max == 0) {
                return 'bg-success';
            }
        }
        return '';
    },
    totalTitle() {
        if (this.energyGoal) {
            if (this.energyGoal.min || this.energyGoal.min == 0) {
                if (this.energyGoal.max || this.energyGoal.max == 0) {
                    return this.energyGoal.min + ' - ' + this.energyGoal.max;
                }
                return '>' + this.energyGoal.min;
            }
            if (this.energyGoal.max || this.energyGoal.max == 0) {
                return '<' + this.energyGoal.max;
            }
        }
        return '';
    },
    activityPreset: {
        get() {
          var dayPreset = this.$store.state.activities.activityPresetDays.find(p => moment(p.date).isSame(this.selectedDate, 'day'));
          if (dayPreset) {
            return this.$activityPresets.find(p => p.id == dayPreset.activityPresetId);
          }
        },
        set(value) {
          this.changeActivityPreset(value);
        }
    }
  },
  methods: {
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
            this.$store.dispatch(constants.SELECT_MEAL_DIARY_DATE, { date: newDate });
            this.fetchData();
            var preset = this.getActivityPreset();
            this.changeActivityPreset(preset);
        }
    },
    changeActivityPreset(preset) {
        var date = this.selectedDate;
        if(preset && preset.id){
          this.$store.dispatch(constants.SAVE_ACTIVITY_PRESET_DAY, { date, preset });
        }
    },
    fetchData(refreshCallback) {
      var start = moment(this.selectedDate).startOf('day');
      var end = moment(this.selectedDate).endOf('day');
      var force = refreshCallback && true;
      this.$store.dispatch(constants.FETCH_MEALS, {
        start,
        end,
        force
      }).then(meals => {
        this.$store.commit(constants.LOADING_DONE);
        this.updateComputedValues();
        if(refreshCallback){
          refreshCallback();
        }
      });
      this.$store.dispatch(constants.FETCH_WORKOUTS, { start: start, end: end });
      this.$store.dispatch(constants.FETCH_ENERGY_EXPENDITURES, { start: start, end: end });
      this.$store.dispatch(constants.FETCH_ACTIVITY_PRESET_DAYS, {
        start: start,
        end: end
    }).then(presets => {
        var preset = presets.find(p => moment(p.date).isSame(this.selectedDate, 'day'));
        if (!preset) {
          preset = this.getActivityPreset();
          this.changeActivityPreset(preset);
        }
    });
    },
    refresh(done){
        this.init(done);
    },
    swipe(event){
        if(event.direction == "left"){
            this.changeDate(1);
        }
        else if(event.direction == "right"){
            this.changeDate(-1);
        }
        else{
            return false;
        }
    },
    mealName(defMeal) {
      if (defMeal.definition) {
          return defMeal.definition.name;
      }
      return this.formatTime(defMeal.meal.time);
    },
    addRow(mealdef){
      this.selectedRow = { 
        mealDefinitionId: mealdef.definition ? mealdef.definition.id : undefined,
        mealId: mealdef.meal ? mealdef.meal.id : undefined, 
        food: undefined, 
        quantity: undefined, 
        portion: undefined
      };
      this.$refs.editRow.show(this.selectedRow);
    },
    clickRow(mealDef, row){
      this.$q.actionSheet({
        title: `${row.foodName} ${ row.quantity } ${ row.portionName || 'g' }`,
        grid: true,
        actions: [
          {
            label: this.$t('edit'),
            icon: 'fas fa-edit',
            handler: () => {
              this.editRow(row);
            }
          },
          {
            label: this.$t('copy'),
            icon: 'fas fa-copy',
            handler: () => {
              this.copyRow(row);
            }
          },
          {
            label: this.$t('delete'),
            icon: 'fas fa-trash',
            handler: () => {
              this.deleteRow(mealDef, row);
            }
          }
        ],
        dismiss: {
          // label is used only for iOS theme
          label: 'Cancel',
      
          // tell what to do when Action Sheet
          // is dismised (doesn't trigger when
          // any of the actions above are clicked/tapped)
          handler: function() {
          }
        }
      }).catch(() => {
      });
    },
    clickMeal(mealDef){
        if(!mealDef.meal){
            return;
        }

        var mealName = this.mealName(mealDef);
        this.$q.actionSheet({
          title: mealName,
          grid: true,
          actions: [
            {
              label: this.$t('copy'),
              icon: 'fas fa-copy',
              handler: () => {
                this.copyMeal(mealDef.meal);
              }
            },
            {
              label: this.$t('delete'),
              icon: 'fas fa-trash',
              handler: () => {
                this.deleteMeal(mealDef.meal);
              }
            }
          ],
          dismiss: {
            label: this.$t('cancel'),
            handler: () => {
              
            }
          }
        }).catch(() => {
        });
      },
    editRow(row){
      this.selectedRow = row;
      this.$refs.editRow.show(this.selectedRow);
    },
    copyMeal(meal) {
        this.$store.dispatch(constants.CLIPBOARD_COPY, {
            type: constants.MEAL,
            data: meal
        });
    },
    deleteMeal(meal) {
      this.$store.dispatch(constants.DELETE_MEAL, {
        meal
      });
    },
    pasteMeal(mealDef) {
        var meal = this.$store.state.clipboard.data;
        this.appendRows(mealDef, meal.rows);
        this.$store.dispatch(constants.CLIPBOARD_CLEAR, { }).then(_ => {

        }).catch(_ => {

        });
    },
    copyRow(row) {
        this.$store.dispatch(constants.CLIPBOARD_COPY, {
          type: constants.MEAL_ROWS,
          data: [row]
        }).then(_ => {

        }).catch(_ => {

        });
    },
    pasteRows(mealDef) {
      var rows = this.$store.state.clipboard.data;
      this.appendRows(mealDef, rows);
      this.$store.dispatch(constants.CLIPBOARD_CLEAR, { });
    },
    appendRows(mealDef, rows) {
      var meal = {
        id: mealDef.meal ? mealDef.meal.id : undefined,
        date: this.selectedDate,
        definitionId: mealDef.definition.id,
        rows : mealDef.meal && mealDef.meal.rows ? mealDef.meal.rows.map(r => { return { foodId: r.foodId, quantity: r.quantity, portionId: r.portionId }}) : []
      }
      rows.forEach(r => {
        meal.rows.push({ foodId: r.foodId, quantity: utils.parseFloat(r.quantity), portionId: r.portionId });
      });
      this.$store.dispatch(constants.SAVE_MEAL, {
        meal
      });
    },
    deleteRow(mealdef, row){
      this.$store.dispatch(constants.DELETE_MEAL_ROW, {
        row
      }).then(_ => {
        if(mealdef.meal.rows.length == 0){
            mealdef.meal = undefined;
        }
        this.updateComputedValues();
      });
      return true;
    },
    saveRow(row){
      row.date = this.selectedDate;
      this.$store.dispatch(constants.SAVE_MEAL_ROW, {
        row
      }).then(_ => {
        this.updateComputedValues();
      });
      this.selectedRow = {};
      this.$refs.editRow.hide();
    },
    showMealSettings(){
      this.$refs.mealSettings.show();
    },
    nutrientGoal(nutrientId, meal) {
      return utils.nutrientGoal(this.$nutritionGoal, this.workouts, nutrientId, this.selectedDate, meal);
    },
    init(done){
        if(this.isLoggedIn){
          this.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, { }).then(_ => {
            this.fetchData(done);
            }).catch(reason => {
              this.$store.commit(constants.LOADING_DONE, { });
            });
            this.$store.dispatch(constants.FETCH_LATEST_FOODS, { });
            this.$store.dispatch(constants.FETCH_MOST_USED_FOODS, { });
            this.$store.dispatch(constants.FETCH_MY_FOODS, { });
        }
        else {
            setTimeout(() => {
              this.init();
            } , 100);
        }
    },
    updateComputedValues() {
      this.$nextTick(() => {
        this.eatenEnergy = this.dayNutrients ? this.dayNutrients[this.energyId] ? this.dayNutrients[this.energyId] : 0 : 0;
      });
    },
    getActivityPreset() {
      var preset;
      switch (this.selectedDate.getDay()) {
          case 0:
            preset = this.$activityPresets.find(p => p.sunday == true);
            break;
          case 1:
            preset = this.$activityPresets.find(p => p.monday == true);
            break;
          case 2:
            preset = this.$activityPresets.find(p => p.tuesday == true);
            break;
          case 3:
            preset = this.$activityPresets.find(p => p.wednesday == true);
            break;
          case 4:
            preset = this.$activityPresets.find(p => p.thursday == true);
            break;
          case 5:
            preset = this.$activityPresets.find(p => p.friday == true);
            break;
          case 6:
            preset = this.$activityPresets.find(p => p.saturday == true);
            break;
      }
      return preset;
    },
    showHelp(){
        this.$refs.help.open();
    },
    addMeal(){
      var date = this.selectedDate;
      var time = this.newMealTime;
      var meal = {
        date: date,
        time: time.getHours() + ':' + time.getMinutes(),
        rows: []
      };
      this.$store.dispatch(constants.SAVE_MEAL, { meal });
    }
  },
  created(){
    this.init();
    this.newMealTime = utils.previousQuarterHour();
  }
}
