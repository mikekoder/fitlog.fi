import constants from '../../store/constants'
import api from '../../api'

export default {
    data () {
        return {
            measures: []
        }
    },
    computed:{
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
            this.$router.push({ name: 'measurement-details' });
        },
        saveMeasurements(measurements) {
            var self = this;
            
        },
        cancelMeasurements() {
            this.create = false;
        }
    },
    created() {
        this.loadMeasures();
    },
}