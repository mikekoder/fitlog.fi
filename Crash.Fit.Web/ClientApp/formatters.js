import moment from 'moment'

function formatDate(value) {
    if(!value){
        return '-';
    }
    var m = new moment(value);
    return m.format('DD.MM.YYYY');
}
function formatTime(value) {
    if(!value){
        return '-';
    }
    var m = new moment(value);
    return m.format('HH:mm');
}
function formatDateTime(value, format) {
    if(!value){
        return '-';
    }
    var m = new moment(value);
    format = format || 'DD.MM.YYYY HH:mm';
    return m.format(format);
}
function formatUnit(unit){
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
export default { formatDate, formatTime, formatDateTime, formatUnit }