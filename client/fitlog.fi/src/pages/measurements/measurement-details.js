import constants from '../../store/constants'
import api from '../../api'
import utils from '../../utils'
import PageMixin from '../../mixins/page'

export default {
    mixins: [PageMixin],
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
          var measurements = {
            time: this.time,
            measurements: this.measurements.filter(m => m.value).map(m => { return { 
              measureId: m.id, 
              measureName: m.name, 
              value: utils.parseFloat(m.value), 
              unit: m.unit } })
          };
          api.saveMeasurements(measurements).then(response => {
            this.notifySuccess(this.$t('saveSuccessful'));
            this.$router.replace({ name: 'measurements' });
          });
        },
        cancel() {
            this.$router.go(-1);
        }
    },
    created() {
      this.units = [
        { label: '', value: ''},
        { label: this.formatUnit('KG'), value: 'KG' },
        { label: this.formatUnit('CM'), value: 'CM' },
        { label: this.formatUnit('MM'), value: 'MM' },
        { label: this.formatUnit('PERCENT'), value: 'PERCENT' }
      ];
      this.time = utils.previousHalfHour();
      api.listMeasures().then(response => {
        this.measurements = response.data.map(m => { return { id: m.id, name: m.name, value: undefined, unit: m.unit} });
        this.$store.commit(constants.LOADING_DONE);
      });
        
    }
}