import constants from '../../store/constants'

export default {
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
            var self = this;
            this.$store.dispatch(constants.ACTIVATE_NUTRITION_GOAL, {
                goal,
                success() {
                    self.notifySuccess(self.$t('saveSuccessful'));
                 },
                failure(xhr) {
                    self.notifyError(self.$t('saveFailed'));
                }
            });
        },
        deleteGoal(goal) {
            var self = this;
            this.$store.dispatch(constants.DELETE_NUTRITION_GOAL, {
                goal,
                success() { },
                failure() {
                    self.notifyError(this.$t('deleteFailed'));
                }
            });
        },
        clickGoal(goal){
            var self = this;
            this.$q.actionSheet({
              title: goal.name,
              grid: true,
              actions: [
                {
                  label: self.$t('edit'),
                  icon: 'fa-edit',
                  handler: () => {
                    self.showGoal(goal);
                  }
                },
                {
                    label: self.$t('activate'),
                    icon: 'fa-check',
                    handler: () => {
                      self.activate(goal);
                    }
                },
                {
                  label: self.$t('delete'),
                  icon: 'fa-trash',
                  handler: () => {
                    self.deleteGoal(goal);
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
       
        self.$store.dispatch(constants.FETCH_NUTRITION_GOALS, {
            success(goals) {
                self.goals = goals;
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() { }
        });
    }
}