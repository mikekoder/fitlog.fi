import constants from '../../store/constants'
import ExercisesMixin from '../../mixins/exercises'
import PageMixin from '../../mixins/page'
import api from'../../api'

export default {
  mixins: [ExercisesMixin, PageMixin],
  data () {
    return {
      tab: 'tab-1',
      muscleGroup: undefined,
      equipment: undefined,
      searchText: '',
      searching: false,
      searchResults: [],
    }
  },
  computed: {
    ownExercises() {
      return this.$store.state.training.exercises;
    },
    muscleGroups(){
      return this.$store.state.training.muscleGroups.sort((a,b) => a.name < b.name ? -1 : 1).map(g => {return {...g, label: g.name, value: g }});
    },
    muscleGroupText(){
      return this.muscleGroup ? this.muscleGroup.name : this.$t('select');
    },
    equipments(){
      return this.$store.state.training.equipments.sort((a,b) => a.name < b.name ? -1 : 1).map(e => {return {...e, label: e.name, value: e }});
    },
    equipmentText(){
      return this.equipment ? this.equipment.name : this.$t('select');
    },
  },
  methods: {
    showExercise(exercise){
      this.$router.push({name: 'exercise-details',params:{id:exercise.id}});
    },
    createExercise(){
      this.$router.push({ name: 'exercise-details', params: { id: constants.NEW_ID } });
    },
    showProgress(exercise){
      this.$router.push({ name: 'exercise-progress', params: { exerciseId: exercise.id} });
    },
    deleteExercise(exercise) {
      this.$store.dispatch(constants.DELETE_EXERCISE, {
          exercise
      });
    },
    showTransfer(exercise){
      this.$router.push({name: 'exercise-transfer',params: { exerciseId: exercise.id }});
    },
    clickExercise(exercise){
      var actions = [
        {
          label: exercise.userId ? this.$t('edit') : this.$t('details'),
          icon: 'fas fa-edit',
          handler: () => {
            this.showExercise(exercise);
          }
        },
        {
          label: this.$t('progress'),
          icon: 'fas fa-chart-line',
          handler: () => {
            this.showProgress(exercise);
          }
        },
        {
          label: this.$t('transferData'),
          icon: 'fas fa-exchange-alt',
          handler: () => {
            this.showTransfer(exercise);
          }
        }
      ];
      if(exercise.userId){
        actions.push({
          label: this.$t('delete'),
          icon: 'fas fa-trash',
          handler: () => {
            this.deleteExercise(exercise);
          }
        });
      }
      this.$q.actionSheet({
        title: exercise.name,
        grid: true,
        actions,
        dismiss: {
            label: this.$t('cancel'),
            handler: () => {
                
            }
        }
      });
    },
    search(){
      if(this.searchText.length >= 2 || this.muscleGroup || this.equipment){
        this.searching = true;
        api.searchExercises(this.searchText, this.muscleGroup ? this.muscleGroup.id : undefined, this.equipment ? this.equipment.id : undefined).then(response => {
            this.searchResults = response.data;
            this.searching = false;
        });
      }
      else {
          this.searchResults = [];
      }
      if(this.exercise && this.searchText.length < this.exercise.name.length){
          this.exercise = undefined;
      }
    },
  },
  created () {
    this.$store.dispatch(constants.FETCH_EXERCISES, {
      forceRefresh: true
    }).then(_ => {
      this.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      this.$store.commit(constants.LOADING_DONE, { });
    });
    
  }
}
