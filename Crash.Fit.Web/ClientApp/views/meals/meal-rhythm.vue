<template>
    <div class="modal fade" id="modal-default" style="display: block;" v-bind:class="{ in: show }">
        <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
            <button type="button" class="close" @click="cancel">
                <span aria-hidden="true">×</span></button>
            <h4 class="modal-title">{{ $t('mealRhythm') }}</h4>
            </div>
            <div class="modal-body">
                <table class="table">
                    <thead>
                        <tr>
                            <th>{{ $t('mealName') }}</th><th>{{ $t('starts') }}</th><th>{{ $t('ends') }}</th><td></td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(def, index) in definitions">
                            <td><input class="form-control" v-model="def.name" /></td>
                            <td><datetime-picker v-bind:value="def.start" v-on:change="def.start=arguments[0]" v-bind:format="'HH:mm'"></datetime-picker></td>
                            <td><datetime-picker v-bind:value="def.end" v-on:change="def.end=arguments[0]" v-bind:format="'HH:mm'"></datetime-picker></td>
                            <td><button class="btn btn-sm btn-danger" @click="removeMeal(index)">{{ $t('delete') }}</button></td>
                        </tr>
                    </tbody>
                    <tbody>
                        <tr>
                            <td><input class="form-control" v-model="othersName" /></td>
                            <td colspan="2">Muut ajat</td>
                        </tr>
                    </tbody>
                </table>
                <button class="btn" @click="addMeal">{{ $t('add') }}</button>
            </div>
            <div class="modal-footer">
                <button class="btn btn-default pull-left" @click="cancel">{{ $t('cancel')}}</button>
                <button class="btn btn-primary" @click="save">{{ $t('save')}}</button>
            </div>
        </div>
        </div>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var api = require('../../api');
    var formatters = require('../../formatters')
    var toaster = require('../../toaster');

module.exports = {
    data () {
        return {
            othersId: '',
            othersName: '',
            definitions: []
        }
    },
    props: {
        show: false,
        value: []
    },
    computed: {
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker')
    },
    methods: {
        addMeal: function () {
            this.definitions.push({start: '01.01.2000 10:00', end: '01.01.2000 13:00'});
        },
        removeMeal: function (index) {
            this.definitions.splice(index, 1);
        },
        cancel: function () {
            this.$emit('close');
        },
        save: function () {
            var self = this;
            var defs = self.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: self.formatTime(d.start), end: self.formatTime(d.end) } });
            defs.push({ id: self.othersId, name: self.othersName });
            self.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
                definitions: defs,
                success: function () {
                    self.$emit('close');
                },
                failure: function () {
                    toaster.error(self.$t('saveFailed'));
                }
            });
        },
        formatTime: formatters.formatTime
    },
    mounted: function () {
        var definitions = this.$store.state.nutrition.mealDefinitions;
        this.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
        var other = definitions.find(d => !d.startHour);
        if (other) {
            this.othersId = other.id;
            this.othersName = other.name;
        }
    }
}
</script>

<style scoped>
</style>