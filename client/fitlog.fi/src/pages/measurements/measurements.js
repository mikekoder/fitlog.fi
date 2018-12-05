import constants from '../../store/constants'
import api from '../../api'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
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
            api.listMeasures().then(response => {
                self.measures = response.data;
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
        },
        showProgress(measure){
            this.$router.push({ name: 'measurement-progress', params: { measureId: measure.id} });
        }
    },
    created() {
        this.loadMeasures();
    },
}