import Vue from 'vue'
import { TouchHold, TouchSwipe } from 'quasar'
import moment from 'moment'
import constants from '../store/constants'
export default ({ app, Vue }) => {
  Vue.mixin({
    directives: {
        TouchHold,
        TouchSwipe
    },
    data () {
        return {
            cardTitleBackground: 'bg-grey-3'
        }
    },
  computed:{
      isLoggedIn(){
          return this.$store.state.profile.profile && true;
      },
      isCordova(){
        return this.$q.platform.is.cordova;
      },
      isDesktop(){
        return this.$q.platform.is.desktop;
      },
      $profile() {
        return this.$store.state.profile.profile;
      },
      localMonthNames(){
        return [
          this.$t('january'),
          this.$t('february'),
          this.$t('march'),
          this.$t('april'),
          this.$t('may'),
          this.$t('june'),
          this.$t('july'),
          this.$t('august'),
          this.$t('september'),
          this.$t('october'),
          this.$t('november'),
          this.$t('december')]
      },
      localDayNames(){
        return [
          this.$t('sunday'),
          this.$t('monday'),
          this.$t('tuesday'),
          this.$t('wednesday'),
          this.$t('thursday'),
          this.$t('friday'),
          this.$t('saturday')
        ]
      },
      localDayNamesAbbr(){
        return [
          this.$t('sundayAbbr'),
          this.$t('mondayAbbr'),
          this.$t('tuesdayAbbr'),
          this.$t('wednesdayAbbr'),
          this.$t('thursdayAbbr'),
          this.$t('fridayAbbr'),
          this.$t('saturdayAbbr')
        ]
      }
  },
    methods: {
      formatDate(value) {
        if(!value){
            return '-';
        }
        var m = new moment(value);
        return m.format('DD.MM.YYYY');
    },
      formatTime(value) {
        if(!value){
            return '-';
        }
        var m = new moment(value);
        return m.format('HH:mm');
    },
    formatDateTime(value, format) {
        if(!value){
            return '-';
        }
        var m = new moment(value);
        format = format || 'DD.MM.YYYY HH:mm';
        return m.format(format);
    },
    formatDuration(hours, minutes) {
        var time = '01.01.2000 ' + (hours || 0) + ':' + (minutes || 0);
        return this.formatTime(time);
    },
    formatUnit(unit){
      switch(unit){
        case 'G':
          return 'g';
        case 'MG':
          return 'mg';
        case 'UG':
          return '\u03BCg';
        case'KCAL':
          return 'kcal';
        case 'KJ':
          return 'kJ';
        case 'CM':
          return 'cm';
        case 'KG':
          return 'kg';
        case 'KCAL_DAY':
          return 'kcal/' + this.$t('day');
        case 'MM':
          return 'mm';
        case 'PERCENT':
          return '%';
        default:
          return unit;
      }
    },
    formatDecimal (value, precision) {
        if (!value) {
            return value;
        }
        return value.toFixed(precision);
    },
    notifySuccess(message){
        this.$q.notify({ message, color: 'positive' });
    },
    notifyError(message){
        this.$q.notify({ message, color:'negative', icon: 'report_problem' });
    }
  },
  created(){
  },
  beforeRouteLeave(to, from, next) {
        //this.$store.commit(constants.LOADING);
        next();
    }   
  });
}