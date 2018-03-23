import constants from '../../store/constants'
import api from '../../api'

export default {
    name: 'energy-expenditure-details',
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
            var self = this;
            self.id = energyExpenditure.id;
            self.time = energyExpenditure.time;
            self.activityName = energyExpenditure.activityName;
            self.energyKcal = energyExpenditure.energyKcal;
            self.duration = '01.01.2000 ' + (energyExpenditure.hours || 0) + ':' + (energyExpenditure.minutes || 0);
            self.$store.dispatch(constants.FETCH_ACTIVITIES, {
                success(activities) {
                    self.activities = activities.map(a => {return { ...a, label: a.name, value: a}});
                    if (energyExpenditure.activityId) {
                        self.activity = self.activities.find(a => a.id == energyExpenditure.activityId);
                    }
                },
                failure() {
                }
            });
            self.$refs.modal.show();
        },
        hide(){
            this.cancel();
        },
        cancel() {
            this.$refs.modal.hide();
        },
        save() {
            var self = this;
            var time = self.duration ? new Date(self.duration) : undefined;
            var expenditure = {
                id: self.id,
                time: self.time,
                activityId: self.activity ? self.activity.id : undefined,
                hours: time ? time.getHours() : undefined,
                minutes: time ? time.getMinutes() : undefined,
                activityName: self.activity ? self.activity.name : self.activityName,
                energyKcal: self.energyKcal
            };
            this.$emit('save', expenditure);
        }
    }
}