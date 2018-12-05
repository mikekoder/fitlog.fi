import constants from '../../store/constants'
import api from '../../api'
import ExercisePicker from '../../components/exercise-picker.vue'
import PageMixin from '../../mixins/page'

export default {
    name: 'exercise-transfer',
    mixins: [PageMixin],
    components: {
      'exercise-picker': ExercisePicker
    },
    data () {
        return {
            fromExercise: undefined,
            toExercise: undefined,
            transferWorkouts: true,
            transferRoutines: true,
            transfer1rm: true
        }
    },
    computed: {
        canTransfer() {
            return (this.fromExercise && this.toExercise && this.fromExercise != this.toExercise);
        }
    },
    methods: {
        selectExercise(){
          this.$refs.exercisePicker.show(this.toExercise, true);
        },
        exerciseSelected(exercise){
          this.toExercise = exercise;
          this.$refs.exercisePicker.hide();
        },
        transfer() {
            api.transferExerciseData(this.fromExercise.id, this.toExercise.id, this.transferWorkouts, this.transferRoutines, this.transfer1rm).then(response => {
              this.notifySuccess(this.$t('transferSuccessful'));
            }).catch(reason => {
              this.notifyError(this.$t('saveFailed'));
            });
        }
    },
    created(){
      var id = this.$route.params.exerciseId;
      this.$store.dispatch(constants.FETCH_EXERCISE, { id }).then(exercise => {
          this.fromExercise = exercise;
          this.$store.commit(constants.LOADING_DONE);
      }).catch(_ => {
          this.$store.commit(constants.LOADING_DONE);
      });
    }
}