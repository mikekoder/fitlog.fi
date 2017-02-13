<template>
    <div class='input-group date'>
        <input type='text' class="form-control" />
        <span class="input-group-addon">
            <span class="glyphicon glyphicon-calendar"></span>
        </span>
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
        format: {}
    },
    methods: {
    },
    mounted: function () {
        var format = this.format || 'DD.MM.YYYY HH:mm';
        $(this.$el).datetimepicker({ format: format });
        this.control = $(this.$el).data("DateTimePicker");

        if (this.value) {
            var m = moment(this.value);
            this.control.date(m.format(format));
        }
        var self = this;
        $(this.$el).on("dp.change", function () {
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