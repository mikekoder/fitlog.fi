import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
  data () {
    return {
        selectedRoutine: null
    }
},
computed: {
    routines(){
        return this.$store.state.training.routines;
    },
    exercises() {
        return this.$store.state.training.exercises;
    }
},
methods: {
    createRoutine(){
        this.$router.push({ name: 'routine-details', params: { id: constants.NEW_ID } });
    },
    showRoutine(routine){
        this.$router.push({name: 'routine-details',params:{id:routine.id}});
    },
    activate(routine){
      this.$store.dispatch(constants.ACTIVATE_ROUTINE, {
        routine
      }).catch(_ => {
        this.notifyError(this.$t('activationFailed'));
      });
    },
    deleteRoutine(routine) {
        this.$store.dispatch(constants.DELETE_ROUTINE, {
            routine
        }).catch(_ => {
          this.notifyError(this.$t('deleteFailed'));
        });
    },
    clickRoutine(routine){
        this.$q.actionSheet({
          title: routine.name,
          grid: true,
          actions: [
            {
              label: this.$t('edit'),
              icon: 'fas fa-edit',
              handler: () => {
                this.showRoutine(routine);
              }
            },
            {
              label: this.$t('activate'),
              icon: 'fas fa-check',
              handler: () => {
                this.activate(routine);
              }
            },
            {
              label: this.$t('delete'),
              icon: 'fas fa-trash',
              handler: () => {
                this.deleteRoutine(routine);
              }
            }
          ],
          dismiss: {
              label: this.$t('cancel'),
              handler: () => {
                  
              }
          }
        });
      }
},
created() {
  this.$store.dispatch(constants.FETCH_EXERCISES, { forceRefresh: true });
  this.$store.dispatch(constants.FETCH_ROUTINES, { }).then(_ => {
    this.$store.commit(constants.LOADING_DONE, { });
  });
}
}
