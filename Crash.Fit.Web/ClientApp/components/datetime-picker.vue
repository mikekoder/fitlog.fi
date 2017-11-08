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

    import $ from 'jquery'
    require('eonasdan-bootstrap-datetimepicker')
    require('eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css')
    import moment from 'moment'

    export default {
        name: 'datetime-picker',
        data() {
            return {}
        },
        props: {
            value: {
                type: [Date, String],
                required: false
            },
            format: {
                type: String,
                required: false,
                default: 'DD.MM.YYYY HH:mm'
            },
            icon: {
                type: String,
                required: false
            }
        },
        watch: {
            value() {
                if (this.value) {
                    var format = this.format || 'DD.MM.YYYY HH:mm';
                    var m = moment(this.value);
                    this.control.date(m.format(format));
                }
                else {
                    this.control.date('');
                }
            }
        },
        methods: {
        },
        mounted() {
            var element = $(this.$el).children();
            var format = this.format || 'DD.MM.YYYY HH:mm';
            element.datetimepicker({ format: format, locale: 'fi' });
            this.control = element.data("DateTimePicker");

            if (this.value) {
                var m = moment(this.value);
                this.control.date(m.format(format));
            }
            var self = this;
            element.on("dp.change", () => {
                var m = moment(self.control.date(), format);
                self.$emit('change', m.toDate());
                //self.model = m.toDate();
            });
            element.on("dp.show", () => {
                var m = moment(self.control.date(), format);
                self.$emit('click', m.toDate());
                //self.model = m.toDate();
            });
        }
    }
</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>