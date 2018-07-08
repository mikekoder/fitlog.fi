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
        var self = this;
        this.$store.dispatch(constants.ACTIVATE_ROUTINE, {
            routine,
            success() { },
            failure() {
                self.notifyError(self.$t('activationFailed'));
            }
        });
    },
    deleteRoutine(routine) {
        var self = this;
        this.$store.dispatch(constants.DELETE_ROUTINE, {
            routine,
            success() { },
            failure() {
                self.notifyError(self.$t('deleteFailed'));
            }
        });
    },
    clickRoutine(routine){
        var self = this;
        this.$q.actionSheet({
          title: routine.name,
          grid: true,
          actions: [
            {
              label: self.$t('edit'),
              icon: 'fa-edit',
              handler: () => {
                self.showRoutine(routine);
              }
            },
            {
                label: self.$t('activate'),
                icon: 'fa-check',
                handler: () => {
                  self.activate(routine);
                }
            },
            {
              label: self.$t('delete'),
              icon: 'fa-trash',
              handler: () => {
                self.deleteRoutine(routine);
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
created() {

    var self = this;
    this.$store.dispatch(constants.FETCH_EXERCISES, {
        forceRefresh: true,
        success() { },
        failure() { }
    });
    this.$store.dispatch(constants.FETCH_ROUTINES, {
        success() {
            self.$store.commit(constants.LOADING_DONE);
        },
        failure() { }
    });
}
}
