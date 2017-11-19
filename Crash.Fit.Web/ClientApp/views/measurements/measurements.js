import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import formatters from '../../formatters'

export default {
    data () {
        return {
            measures: [],
            create: false
        }
    },
    computed:{
    },
    components: {
        'measurements-editor': require('./measurements-editor.vue')
    },
    methods: {
        loadMeasures() {
            var self = this;
            api.listMeasures().then((measures) => {
                self.measures = measures;
                self.$store.commit(constants.LOADING_DONE);
            });
        },
        createMeasurements() {
            this.create = true;
        },
        saveMeasurements(measurements) {
            var self = this;
            api.saveMeasurements(measurements).then(() => {
                toaster.info(self.$t('saveSuccessful'));
                self.loadMeasures();
                self.create = false;
            });
        },
        cancelMeasurements() {
            this.create = false;
        },
        datetime: formatters.formatDateTime
    },
    created() {
        this.loadMeasures();
    },
    beforeRouteUpdate (to, from, next) {
        /*
        if (to.params.id) {
            this.editExercise(to.params.id);
        }
        else {
            this.showSummary();
        }*/
        next();
    }
}