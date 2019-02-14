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
          api.listMeasures().then(response => {
            this.measures = response.data;
            this.$store.commit(constants.LOADING_DONE);
          });
        },
        createMeasurements() {
            this.$router.push({ name: 'measurement-details' });
        },
        saveMeasurements(measurements) {
            
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