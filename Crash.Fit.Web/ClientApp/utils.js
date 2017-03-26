﻿const utils = {
    parseFloat(value) {
        if (typeof (value) === 'number') {
            return value;
        }
        return parseFloat(value.replace(',', '.'));
    }
}
module.exports = utils