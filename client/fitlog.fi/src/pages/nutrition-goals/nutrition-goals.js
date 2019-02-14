import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
    data () {
        return {
            goals: []
        }
    },
    computed: {
  
    },
    components: {},
    methods: {
        showGoal(goal){
            this.$router.push({ name: 'nutrition-goal-details', params: { id: goal.id } });
        },
        createGoal(){
            this.$router.push({ name: 'nutrition-goal-details', params: { id: constants.NEW_ID } });
        },
        activate(goal){
            this.$store.dispatch(constants.ACTIVATE_NUTRITION_GOAL, {
                goal
            }).then(_ => {
              this.notifySuccess(this.$t('saveSuccessful'));
            }).catch(_ => {
              this.notifyError(this.$t('saveFailed'));
            });
        },
        deleteGoal(goal) {
          this.$store.dispatch(constants.DELETE_NUTRITION_GOAL, {
            goal
          }).catch(_ => {
            this.notifyError(this.$t('deleteFailed'));
          });
        },
        clickGoal(goal){
          this.$q.actionSheet({
            title: goal.name,
            grid: true,
            actions: [
              {
                label: this.$t('edit'),
                icon: 'fas fa-edit',
                handler: () => {
                  this.showGoal(goal);
                }
              },
              {
                label: this.$t('activate'),
                icon: 'fas fa-check',
                handler: () => {
                  this.activate(goal);
                }
              },
              {
                label: this.$t('delete'),
                icon: 'fas fa-trash',
                handler: () => {
                  this.deleteGoal(goal);
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
      this.$store.dispatch(constants.FETCH_NUTRITION_GOALS, { }).then(goals => {
        this.goals = goals;
        this.$store.commit(constants.LOADING_DONE, { });
        });
    }
}