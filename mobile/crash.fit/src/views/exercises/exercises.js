 import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
 import constants from '../../store/constants'

export default {
  components: {
    QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
  },
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
