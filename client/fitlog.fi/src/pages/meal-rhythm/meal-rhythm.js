import constants from '../../store/constants'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
  data () {
    return {
      othersId: '',
      othersName: '',
      definitions: []
    }
  },
  computed: {
  },
  methods: {
    addMeal() {
        this.definitions.push({start: '01.01.2000 10:00', end: '01.01.2000 13:00'});
    },
    removeMeal(index) {
        this.definitions.splice(index, 1);
    },
    save() {
      var defs = this.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: this.formatTime(d.start), end: this.formatTime(d.end) } });
      defs.push({ id: this.othersId, name: this.othersName });
      this.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
        definitions: defs
      }).then(_ => {
        this.notifySuccess(this.$t('saveSuccessful'));
      }).catch(_ => {
        this.notifyError(this.$t('saveFailed'));
      });
  }
  },
  mounted(){
    this.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {}).then(definitions => {
      this.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
      var other = definitions.find(d => !d.startHour);
      if (other) {
        this.othersId = other.id;
        this.othersName = other.name;
      }
      this.$store.commit(constants.LOADING_DONE);
    });
  },
  beforeRouteUpdate(to) {
    this.$store.commit(constants.LOADING_DONE);
  }
}
