import constants from '../../store/constants'

export default {
    data () {
        return {
          id: undefined,
          type: undefined,
          title: '',
          description: ''
        }
    },
    components: {},
    methods: {
      save() {
        var self = this;
        var feedback = {
            id: self.id,
            type: self.type,
            title: self.title,
            description: self.description
        };
        self.$store.dispatch(constants.SAVE_FEEDBACK, {
            feedback,
            success() {
                toaster.info(self.$t('saveSuccessful'));
                if(feedback.type === 'Bug'){
                  self.$router.replace({ name: 'bugs' });
                }
                else if(feedback.type === 'Improvement'){
                  self.$router.replace({ name: 'improvements' });
                }
            },
            failure() {
                toaster.error(self.$t('saveFailed'));
            }
        });
      },
      cancel() {
        this.$router.go(-1);
      },
      deleteFeedback(){}
    },
    created () {
        this.type = this.$route.meta.type;
        this.$store.commit(constants.LOADING_DONE);
    },
    watch: {
      $route(){
        this.type = this.$route.meta.type;
        this.$store.commit(constants.LOADING_DONE);
      }
    }
}