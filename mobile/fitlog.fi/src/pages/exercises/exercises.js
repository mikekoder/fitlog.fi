import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
  mixins: [PageMixin],
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
    deleteExercise(exercise) {
      this.$store.dispatch(constants.DELETE_EXERCISE, {
          exercise
      });
    },
    clickExercise(exercise){
      var self = this;
      this.$q.actionSheet({
        title: exercise.name,
        grid: true,
        actions: [
          {
            label: self.$t('edit'),
            icon: 'fas fa-edit',
            handler: () => {
              self.showExercise(exercise);
            }
          },
          {
            label: self.$t('progress'),
            icon: 'fas fa-chart-line',
            handler: () => {
              self.showProgress(exercise);
            }
          },
          {
            label: self.$t('delete'),
            icon: 'fas fa-trash',
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
      forceRefresh: true
    }).then(_ => {
      self.$store.commit(constants.LOADING_DONE, { });
    }).catch(_ => {
      self.$store.commit(constants.LOADING_DONE, { });
    });
    
  },
  beforeDestroy () {

  }
}
