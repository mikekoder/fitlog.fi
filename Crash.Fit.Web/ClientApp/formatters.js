var moment = require('moment');

function formatDate(value) {
    var m = new moment(value);
    return m.format('DD.MM.YYYY');
}
function formatTime(value) {
    var m = new moment(value);
    return m.format('HH:mm');
}
function formatDatetime(value, format) {
    var m = new moment(value);
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
export { formatDate, formatTime, formatDatetime, formatUnit }