import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import toaster from '../../toaster'
import mealDefinitionsMixin from '../../mixins/meal-definitions'

export default {
    mixins:[mealDefinitionsMixin],
    data () {
        return {
            othersId: '',
            othersName: '',
            definitions: []
        }
    },
    computed: {
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker')
    },
    methods: {
        $mealDefinitionsLoaded(definitions) {
            var self = this;
            self.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
                var other = definitions.find(d => !d.startHour);
                if (other) {
                    self.othersId = other.id;
                    self.othersName = other.name;
                }
                self.$store.commit(constants.LOADING_DONE);
        },
        addMeal() {
            this.definitions.push({start: '01.01.2000 10:00', end: '01.01.2000 13:00'});
        },
        deleteMeal(index) {
            var self = this;
            self.definitions.splice(index, 1);
            /*
            var definition = self.definitions[index];
            self.$store.dispatch(constants.DELETE_MEAL_DEFINITION, {
                definition,
                success() {
                    
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
            */
        },
        save() {
            var self = this;
            var defs = self.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: self.formatTime(d.start), end: self.formatTime(d.end) } });
            defs.push({ id: self.othersId, name: self.othersName });
            self.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
                definitions: defs,
                success() {
                    toaster.info(self.$t('saveSuccessful'));
                },
                failure() {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        formatTime: formatters.formatTime
    },
    created() {
    }
}