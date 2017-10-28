import constants from '../../store/constants'
import api from '../../api'
import formatters from '../../formatters'
import utils from '../../utils'

export default {
    data () {
        return {
            time: null,
            measurements: null,
        }
    },
    computed: {
    },
    props: {
        measures: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
    },
    methods: {
        addMeasurement(){
            this.measurements.push({name: undefined, value:undefined});
        },
        deleteMeasurement(index){
            this.measurements.splice(index, 1);
        },
        save() {
            var measurements = {
                time: this.time,
                measurements: this.measurements.filter(m => m.value).map(m => { return { measureId: m.id, measureName: m.name, value: utils.parseFloat(m.value) } })
            };

            this.saveCallback(measurements);
        },
        cancel() {
            this.cancelCallback();
        },
        deleteMeasurement() {
            this.deleteCallback(this.exercise);
        }
    },
    created() {
        var self = this;
        this.time = utils.previousHalfHour();
        this.measurements = this.measures.map(m => { return { id: m.id, name: m.name, value: undefined} });
    }
}