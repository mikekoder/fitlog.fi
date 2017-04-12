const utils = {
    parseFloat(value) {
        if (typeof (value) === 'number') {
            return value;
        }
        return parseFloat((value || '0').replace(',', '.'));
    }
}
module.exports = utils