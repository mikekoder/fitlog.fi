<template>
    <div>
        <div class='input-group date' v-if="icon">
            <input type='text' class="form-control" />
            <span class="input-group-addon">
                <span class="glyphicon glyphicon-calendar"></span>
            </span>
        </div>
        <input type='text' class="form-control" v-if="!icon" />
    </div>
</template>

<script>

$ = require('jquery')
require('eonasdan-bootstrap-datetimepicker')
require('eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css')
moment = require('moment');
module.exports = {
    data: function() {
        return { }
    },
    props:{
        value: {},
        format: {},
        icon: {}
    },
    methods: {
    },
    mounted: function () {
        var element = $(this.$el).children();
        var format = this.format || 'DD.MM.YYYY HH:mm';
        element.datetimepicker({ format: format });
        this.control = element.data("DateTimePicker");

        if (this.value) {
            var m = moment(this.value);
            this.control.date(m.format(format));
        }
        var self = this;
        element.on("dp.change", function () {
            var m = moment(self.control.date(), format);
            self.$emit('change', m.toDate())
            //self.model = m.toDate();
        });
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>