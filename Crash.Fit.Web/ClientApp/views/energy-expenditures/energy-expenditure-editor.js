import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import DatetimePicker from '../../components/datetime-picker'

export default {
    name: 'energy-expenditure-editor',
    data () {
        return {
            manual: false,
            time: undefined,
            activity: undefined,
            activityName: '',
            //hours: 1,
            //minutes: 0,
            duration: undefined,
            energyKcal: undefined
        }
    },
    props: {
        show: false,
        energyExpenditure: undefined
    },
    computed: {
        canSave() {
            return (this.activity && (this.duration)) || (this.activityName && this.energyKcal);
        },
        activities() {
            return this.$store.state.training.activities;
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
    components: {
        DatetimePicker
    },
    methods: {
        cancel() {
            this.$emit('close');
        },
        save() {
            var self = this;
            var time = self.duration ? new Date(self.duration) : undefined;
            var expenditure = {
                id: self.energyExpenditure.id,
                time: self.time,
                activityId: self.activity ? self.activity.id : undefined,
                hours: time ? time.getHours() : undefined,
                minutes: time ? time.getMinutes() : undefined,
                activityName: self.activity ? self.activity.name : self.activityName,
                energyKcal: self.energyKcal
            };
            this.$emit('save', expenditure);
        }
    },
    created() {
        var self = this;
        self.$store.dispatch(constants.FETCH_ACTIVITIES, {
                success() {
                    if (self.energyExpenditure.activityId) {
                        self.activity = self.activities.find(a => a.id == self.energyExpenditure.activityId);
                    }
                },
                failure() {
                }
            });
        
        self.activityName = self.energyExpenditure.activityName;
        //self.hours = self.energyExpenditure.hours;
        //self.minutes = self.energyExpenditure.minutes;
        self.duration = '01.01.2000 ' + (self.energyExpenditure.hours || 0) + ':' + (self.energyExpenditure.minutes || 0);
        self.energyKcal = self.energyExpenditure.energyKcal;
        self.time = self.energyExpenditure.time;
    }
}