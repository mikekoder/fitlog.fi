 import constants from '../../store/constants'

export default {
  data () {
    return {
      tab: 'tab-1',
    }
  },
  computed: {
      ownExercises() {
          return this.$store.state.training.exercises;
    }
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
    clickExercise(exercise){
      var self = this;
      this.$q.actionSheet({
        title: exercise.name,
        grid: true,
        actions: [
          {
            label: self.$t('edit'),
            icon: 'fa-edit',
            handler: () => {
              self.showExercise(exercise);
            }
          },
          {
            label: self.$t('progress'),
            icon: 'fa-chart-line',
            handler: () => {
              self.showProgress(exercise);
            }
          },
          {
            label: self.$t('delete'),
            icon: 'fa-trash',
            handler: () => {
              self.deleteExercise(exercise);
            }
          }
        ],
        dismiss: {
            label: self.$t('cancel'),
            handler: () => {
                
            }
        }
      });
    }
  },
  created () {
    var self = this;
    
    self.$store.dispatch(constants.FETCH_EXERCISES, {
      success(){
        self.$store.commit(constants.LOADING_DONE);
      },
      failure(){
        self.$store.commit(constants.LOADING_DONE);
      }
    });
    
  },
  beforeDestroy () {

  }
}
