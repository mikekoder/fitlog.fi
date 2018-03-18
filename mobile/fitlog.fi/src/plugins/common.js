import Vue from 'vue'
import { TouchHold, TouchSwipe } from 'quasar'
import translations from '../i18n/fi/translations'
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
      isDesktop(){
        return !this.$q.platform.is.cordova;
      },
      $profile() {
        return this.$store.state.profile.profile;
      },
      localMonthNames(){
        var tran = translations;
        return [
          tran.january,
          tran.february,
          tran.march,
          tran.april,
          tran.may,
          tran.june,
          tran.july,
          tran.august,
          tran.september,
          tran.october,
          tran.november,
          tran.december]
      },
      localDayNames(){
        var tran = translations;
        return [
          tran.sunday,
          tran.monday,
          tran.tuesday,
          tran.wednesday,
          tran.thursday,
          tran.friday,
          tran.saturday
        ]
      },
      localDayNamesAbbr(){
        var tran = translations;
        return [
          tran.sundayAbbr,
          tran.mondayAbbr,
          tran.tuesdayAbbr,
          tran.wednesdayAbbr,
          tran.thursdayAbbr,
          tran.fridayAbbr,
          tran.saturdayAbbr
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
      console.log('leaving route');
        //this.$store.commit(constants.LOADING);
        next();
    }   
  });
}