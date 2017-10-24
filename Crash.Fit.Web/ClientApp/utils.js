import moment from 'moment'
export default {
    parseFloat(value) {
        if (typeof (value) === 'number') {
            return value;
        }
        return parseFloat((value || '0').replace(',', '.'));
    },
    roundToNearest(value, step) {
        var inv = 1.0 / step;
        return Math.round(value * inv) / inv;
    },
    previousHalfHour(value){
        if (!value) {
            value = new Date();
        }
        var minutes = value.getMinutes();

        value = new Date(value.setMinutes(minutes - (minutes % 30)));
        value = new Date(value.setSeconds(0));
        value = new Date(value.setMilliseconds(0));
        return value;
    }
}
