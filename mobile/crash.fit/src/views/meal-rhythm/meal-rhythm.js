import constants from '../../store/constants'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime,QField, QInput,QLayout,QFixedPosition } from 'quasar'

export default {
  data () {
    return {
      othersId: '',
      othersName: '',
      definitions: []
    }
  },
  components:{
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime ,QField, QInput,QLayout,QFixedPosition
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
                toaster.info(self.$t('saved'));
            },
            failure() {
                toaster.error(self.$t('saveFailed'));
            }
        });
    }
  },
  created(){
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
                toaster.error(self.$t('fetchFailed'));
            }
        });
  },
  mounted () {
  },
  beforeDestroy () {

  }
}
