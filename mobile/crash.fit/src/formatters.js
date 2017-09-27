import moment from 'moment'

export default {
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
            default:
                return unit;
        }
    }
}