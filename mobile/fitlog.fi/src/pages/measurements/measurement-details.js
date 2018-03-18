import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'

export default {
    data () {
        return {
            time: null,
            measurements: null,
            units: []
        }
    },
    computed: {
        canSave(){
            return true;
        }
    },
    methods: {
        addMeasurement(){
            this.measurements.push({name: undefined, value:undefined});
        },
        deleteMeasurement(index){
            this.measurements.splice(index, 1);
        },
        save() {
            var self = this;
            var measurements = {
                time: self.time,
                measurements: self.measurements.filter(m => m.value).map(m => { return { measureId: m.id, measureName: m.name, value: utils.parseFloat(m.value) } })
            };
            api.saveMeasurements(measurements).then(() => {
                self.notifySuccess(self.$t('saveSuccessful'));
                self.$router.replace({ name: 'measurements' });
            });
        },
        cancel() {
            this.$router.go(-1);
        }
    },
    created() {
        var self = this;
        self.units = [
            { label: '', value: ''},
            { label: self.formatUnit('KG'), value: 'KG' },
            { label: self.formatUnit('CM'), value: 'CM' }
        ];
        this.time = utils.previousHalfHour();
        api.listMeasures().then((measures) => {
            self.measurements = measures.map(m => { return { id: m.id, name: m.name, value: undefined, unit: m.unit} });
            self.$store.commit(constants.LOADING_DONE);
        });
        
    }
}