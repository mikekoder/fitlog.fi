<template>
    <span>{{ str }}</span>
</template>

<script>
moment = require('moment');
module.exports = {
    data: function() {
        return {
            str: null
        }
    },
    props:{
        value: {},
        format: {}
    },
    methods: {
    },
    mounted: function () {
        var format = this.format || 'DD.MM.YYYY HH:mm';
        if (this.value) {
            var m = moment(this.value);
            this.str = m.format(format);
        }
    }
}
</script>