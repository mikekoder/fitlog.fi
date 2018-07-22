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
        var self = this;
        var defs = self.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: self.formatTime(d.start), end: self.formatTime(d.end) } });
        defs.push({ id: self.othersId, name: self.othersName });
        self.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
            definitions: defs,
            success() {
                self.notifySuccess(self.$t('saveSuccessful'));
            },
            failure() {
                self.notifyError(self.$t('saveFailed'));
            }
        });
    }
  },
  mounted(){
    var self = this;
    self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
        success(definitions) {
            self.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
            var other = definitions.find(d => !d.startHour);
            if (other) {
                self.othersId = other.id;
                self.othersName = other.name;
            }
            self.$store.commit(constants.LOADING_DONE);
        },
        failure() {
            self.notifyError(self.$t('fetchFailed'));
        }
    });
  },
  beforeRouteUpdate(to) {
    this.$store.commit(constants.LOADING_DONE);
  }
}
