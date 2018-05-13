import constants from '../../store/constants'
import api from '../../api'
import activityPresetsMixin from '../../mixins/activity-presets'
import utils from '../../utils'
import Help from './activity-levels-help.vue'
import PageMixin from '../../mixins/page'

export default {
    components:{
        'activity-levels-help': Help
    },
    mixins:[activityPresetsMixin, PageMixin],
    data () {
        return {
            tab: 'tab-0',
            presets: []
        }
    },
    computed: {
        canSave(){
            return true;
        }
    },
    methods: {
        $activityPresetsLoaded(presets) {
            var self = this;
            self.presets = presets.map(p => {
                return {
                    id: p.id,
                    name: p.name,
                    sleep: p.sleep,
                    inactivity: p.inactivity,
                    lightActivity: p.lightActivity,
                    moderateActivity: p.moderateActivity,
                    heavyActivity: p.heavyActivity,
                    monday: p.monday,
                    tuesday: p.tuesday,
                    wednesday: p.wednesday,
                    thursday: p.thursday,
                    friday: p.friday,
                    saturday: p.saturday,
                    sunday: p.sunday
                }
            });
            self.$store.commit(constants.LOADING_DONE);
        },
        addPreset() {
            this.presets.push({ name: this.$t('new'), sleep: undefined, inactivity: undefined, lightActivity: undefined, moderateActivity: undefined, heavyActivity: undefined, monday: false, tuesday: false, wednesday: false, thursday: false, friday: false, saturday: false, sunday: false});
        },
        deletePreset(index) {
            var self = this;
            self.presets.splice(index, 1);
        },
        save() {
            var self = this;
            self.$store.dispatch(constants.SAVE_ACTIVITY_PRESETS, {
                presets: self.presets,
                success() {
                    self.notifySuccess(self.$t('saveSuccessful'));
                },
                failure() {
                    self.notifyError(self.$t('saveFailed'));
                }
            });
        },
        daysChanged(preset, day) {
            if (preset[day] == true) {
                this.presets.forEach(p => {
                    if (p != preset) {
                        p[day] = false;
                    }
                });
            }
        },
        calculateInactivity(preset) {
            var sleep = utils.parseFloat(preset.sleep ? preset.sleep : 0);
            var light = utils.parseFloat(preset.lightActivity ? preset.lightActivity : 0);
            var moderate = utils.parseFloat(preset.moderateActivity ? preset.moderateActivity : 0);
            var heavy = utils.parseFloat(preset.heavyActivity ? preset.heavyActivity : 0);

            return 24 - sleep - light - moderate - heavy;
        },
        calculateFactor(preset) {
            var inactivity = this.calculateInactivity(preset);
            var sleep = utils.parseFloat(preset.sleep ? preset.sleep : 0);
            var light = utils.parseFloat(preset.lightActivity ? preset.lightActivity : 0);
            var moderate = utils.parseFloat(preset.moderateActivity ? preset.moderateActivity : 0);
            var heavy = utils.parseFloat(preset.heavyActivity ? preset.heavyActivity : 0);
            return this.formatDecimal((constants.ACTIVITY_FACTOR_SLEEP * sleep + constants.ACTIVITY_FACTOR_INACTIVITY * inactivity + constants.ACTIVITY_FACTOR_LIGHT_ACTIVITY * light + constants.ACTIVITY_FACTOR_MODERATE_ACTIVITY * moderate + constants.ACTIVITY_FACTOR_HEAVY_ACTIVITY * heavy) / 24, 2);
        },
        showHelp(){
            this.$refs.help.open();
        }
    },
    created() {
    }
}