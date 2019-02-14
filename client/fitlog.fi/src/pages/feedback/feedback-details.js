import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
  mixins: [PageMixin],
    data () {
        return {
          id: undefined,
          type: undefined,
          title: '',
          description: ''
        }
    },
    computed:{
      canSave(){
        return this.title && this.description;
      }
    },
    methods: {
      save() {
        var feedback = {
          id: this.id,
          type: this.type,
          title: this.title,
          description: this.description
        };
        this.$store.dispatch(constants.SAVE_FEEDBACK, {
          feedback
        }).then(_ => {
          this.notifySuccess(this.$t('saveSuccessful'));
            if(feedback.type === 'Bug'){
              this.$router.replace({ name: 'bugs' });
            }
            else if(feedback.type === 'Improvement'){
              this.$router.replace({ name: 'improvements' });
            }
        }).catch(_ => {
          this.notifyError(this.$t('saveFailed'));
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