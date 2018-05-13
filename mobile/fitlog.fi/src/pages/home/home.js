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
      rightMenuOpen: true
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
      var self = this;
      var start = moment(self.selectedDate).startOf('day');
      var end = moment(self.selectedDate).endOf('day');
      var defs = self.$store.state.nutrition.mealDefinitions;
      var meals = self.$store.state.nutrition.meals.filter(m => moment(m.time).isBetween(start, end));
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
        var self = this;
        return this.$store.state.training.workouts.filter(w => moment(w.time).isSame(self.selectedDate, 'day'));
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
        var self = this;
        var expenditures = this.$store.state.training.energyExpenditures.filter(e => moment(e.time).isSame(self.selectedDate, 'day'));
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
            var self = this;
            var dayPreset = self.$store.state.activities.activityPresetDays.find(p => moment(p.date).isSame(self.selectedDate, 'day'));
            if (dayPreset) {
                return self.$activityPresets.find(p => p.id == dayPreset.activityPresetId);
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
        this.$store.dispatch(constants.SAVE_ACTIVITY_PRESET_DAY, { date, preset });
    },
    fetchData(refreshCallback) {
      var self = this;
      var start = moment(self.selectedDate).startOf('day');
      var end = moment(self.selectedDate).endOf('day');
      var force = refreshCallback && true;
      self.$store.dispatch(constants.FETCH_MEALS, {
        start,
        end,
        force,
        success: (meals) => {
          self.$store.commit(constants.LOADING_DONE);
          self.updateComputedValues();
          if(refreshCallback){
            refreshCallback();
          }
        },
        failure: () => { }
      });
      self.$store.dispatch(constants.FETCH_WORKOUTS, { start: start, end: end });
      self.$store.dispatch(constants.FETCH_ENERGY_EXPENDITURES, { start: start, end: end });
      self.$store.dispatch(constants.FETCH_ACTIVITY_PRESET_DAYS, {
        start: start,
        end: end,
        success: (presets) =>
        {
            var preset = presets.find(p => moment(p.date).isSame(self.selectedDate, 'day'));
            if (!preset) {
                preset = self.getActivityPreset();
                self.changeActivityPreset(preset);
            }
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
      return this.time(defMeal.meal.time);
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
      var self = this;
      this.$q.actionSheet({
        title: `${row.foodName} ${ row.quantity } ${ row.portionName || 'g' }`,
        grid: false,
        actions: [
          {
            label: self.$t('edit'),
            icon: 'fa-edit',
            handler: () => {
              self.editRow(row);
            }
          },
          {
            label: self.$t('copy'),
            icon: 'fa-copy',
            handler: () => {
              self.copyRow(row);
            }
          },
          {
            label: self.$t('delete'),
            icon: 'fa-trash',
            handler: () => {
              self.deleteRow(mealDef, row);
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
              console.log('Cancelled...')
            }
          }
      }).catch(() => {
          this.$q.notify('cancelled');
      });
    },
    clickMeal(mealDef){
        if(!mealDef.meal){
            return;
        }

        var self = this;
        var mealName =this.mealName(mealDef);
        this.$q.actionSheet({
          title: mealName,
          grid: true,
          actions: [
            {
              label: self.$t('copy'),
              icon: 'fa-copy',
              handler: () => {
                self.copyMeal(mealDef.meal);
              }
            }
          ],
          dismiss: {
              label: self.$t('cancel'),
              handler: () => {
                  
              }
          }
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
    pasteMeal(mealDef) {
        var self = this;
        var meal = self.$store.state.clipboard.data;
        self.appendRows(mealDef, meal.rows);
        self.$store.dispatch(constants.CLIPBOARD_CLEAR, {});
    },
    copyRow(row) {
        this.$store.dispatch(constants.CLIPBOARD_COPY, {
            type: constants.MEAL_ROWS,
            data: [row]
        });
    },
    pasteRows(mealDef) {
        var self = this;
        var rows = self.$store.state.clipboard.data;
        self.appendRows(mealDef, rows);
        self.$store.dispatch(constants.CLIPBOARD_CLEAR, {});
    },
    appendRows(mealDef, rows) {
        var self = this;
        var meal = {
            id: mealDef.meal ? mealDef.meal.id : undefined,
            date: self.selectedDate,
            definitionId: mealDef.definition.id,
            rows : mealDef.meal && mealDef.meal.rows ? mealDef.meal.rows.map(r => { return { foodId: r.foodId, quantity: r.quantity, portionId: r.portionId }}) : []
        }
        rows.forEach(r => {
            meal.rows.push({ foodId: r.foodId, quantity: utils.parseFloat(r.quantity), portionId: r.portionId });
        });
        self.$store.dispatch(constants.SAVE_MEAL, {
            meal,
            success() { },
            failure() { }
        });
    },
    deleteRow(mealdef, row){
        var self = this;
      this.$store.dispatch(constants.DELETE_MEAL_ROW, {
          row,
          success() { 
            if(mealdef.meal.rows.length == 0){
              mealdef.meal = undefined;
            }
            self.updateComputedValues();
          },
          failure() { }
      });
      return true;
    },
    saveRow(row){
        var self = this;
      row.date = this.selectedDate;
      this.$store.dispatch(constants.SAVE_MEAL_ROW, {
          row,
          success() {
            self.updateComputedValues();
          },
          failure() { }
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
        var self = this;
        if(self.isLoggedIn){
            self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
                success() {
                    self.fetchData(done);
                },
                failure() {
                    self.notifyError(self.$t('fetchFailed'));
                    self.$store.commit(constants.LOADING_DONE);
                 }
            });
            /*
            self.$store.dispatch(constants.FETCH_NUTRIENTS, {
                force: true,
                success() { },
                failure() { }
            });
            */
            self.$store.dispatch(constants.FETCH_LATEST_FOODS, {
                success() { },
                failure() { }
            });
            self.$store.dispatch(constants.FETCH_MOST_USED_FOODS, {
                success() { },
                failure() { }
            });
            self.$store.dispatch(constants.FETCH_MY_FOODS, {
                success() { },
                failure() { }
            });
        }
        else {
            setTimeout(() => {
                self.init();
            } , 100);
        }
        
    
        
    },
    updateComputedValues() {
        this.eatenEnergy = this.dayNutrients ? this.dayNutrients[this.energyId] ? this.dayNutrients[this.energyId] : 0 : 0;
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
    }
  },
  created(){
    this.init();
  }
}
