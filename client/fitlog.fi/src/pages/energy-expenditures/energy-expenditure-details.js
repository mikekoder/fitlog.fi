import constants from '../../store/constants'
import api from '../../api'
import PageMixin from '../../mixins/page'

export default {
    name: 'energy-expenditure-details',
    mixins: [PageMixin],
    data () {
        return {
            id: undefined,
            manual: false,
            time: undefined,
            activity: undefined,
            activityName: '',
            duration: undefined,
            energyKcal: undefined,
            activities: []
        }
    },
    computed: {
        canSave() {
            return (this.activity && (this.duration)) || (this.activityName && this.energyKcal);
        },
        totalMinutes() {
            var time = this.duration ? new Date(this.duration) : undefined;
            if (time) {
                return time.getHours() * 60 + time.getMinutes();
            }
            return null;
        },
        weight() {
            return this.$profile.weight;
        },
        estimate() {
            if (this.activity && this.totalMinutes && this.weight) {
                return this.activity.energyExpenditure * this.totalMinutes * this.weight;
            }
            return null;
        }
    },
    watch: {
        manual() {
            if (this.manual) {
                this.activity = undefined;
            }
            else {
                this.activityName = undefined;
                this.energyKcal = undefined;
            }
        }
    },
    methods: {
      show(energyExpenditure){
        this.id = energyExpenditure.id;
        this.time = energyExpenditure.time;
        this.activityName = energyExpenditure.activityName;
          this.energyKcal = energyExpenditure.energyKcal;
          this.duration = '01.01.2000 ' + (energyExpenditure.hours || 0) + ':' + (energyExpenditure.minutes || 0);
          this.$store.dispatch(constants.FETCH_ACTIVITIES, { }).then(activities => {
            this.activities = activities.map(a => {return { ...a, label: a.name, value: a}});
              if (energyExpenditure.activityId) {
                this.activity = this.activities.find(a => a.id == energyExpenditure.activityId);
              }
          });
          this.$refs.modal.show();
      },
      hide(){
          this.cancel();
      },
      cancel() {
          this.$refs.modal.hide();
      },
      save() {
        var time = this.duration ? new Date(this.duration) : undefined;
        var expenditure = {
          id: this.id,
          time: this.time,
          activityId: this.activity ? this.activity.id : undefined,
          hours: time ? time.getHours() : undefined,
          minutes: time ? time.getMinutes() : undefined,
          activityName: this.activity ? this.activity.name : this.activityName,
          energyKcal: this.energyKcal
        };
        this.$emit('save', expenditure);
      },
      filter(text, activity){
        return activity.name.toLowerCase().indexOf(text.toLowerCase()) >= 0;
      }
    }
}