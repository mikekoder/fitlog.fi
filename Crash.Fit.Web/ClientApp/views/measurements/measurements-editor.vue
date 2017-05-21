<template>
    <div id="measurements">
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Aika</label>
                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                </div>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                <div class="row">
                    <div class="col-xs-8 col-sm-4 col-md-2"><label>Mitta</label></div>
                    <div class="col-xs-4 col-sm-2">&nbsp;</div>
                    <div class="col-xs-4 col-sm-2">&nbsp;</div>
                </div>
                <template v-for="(measurement,index) in measurements">
                    <div class="measurement row">
                        <div class="col-xs-8 col-sm-4 col-md-2">
                            <span v-if="measurement.id">{{ measurement.name }}</span>
                            <input type="text" v-else v-model="measurement.name" />
                        </div>
                        <div class="col-xs-8 col-sm-6 col-md-2">
                            <input type="number" v-model="measurement.value" />
                        </div>
                        <div class="col-xs-4 col-sm-2">
                            <div>
                                <button class="btn btn-danger btn-sm" v-if="!measurement.id" @click="removeMeasurement(index)">Poista</button>
                            </div>
                        </div>
                    </div>
                    <div class="measure-separator row hidden-sm hidden-md hidden-lg">
                        <div class="col-sm-12"><hr /></div>
                    </div>
                </template>
                <div class="row">
                    <div class="col-xs-12"><button class="btn" @click="addMeasurement"><i class="fa fa-plus"></i> Lisää</button></div>
                </div>
            </div>
        </div>
        <hr />
        <div class="row main-actions">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody></tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters');
    var utils = require('../../utils');

module.exports = {
    data () {
        return {
            time: null,
            measurements: null,
        }
    },
    computed: {
    },
    props: {
        //exercise: null,
        measures: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
    },
    methods: {
        addMeasurement: function(){
            this.measurements.push({name: undefined, value:undefined});
        },
        removeMeasurement: function(index){
            this.measurements.splice(index, 1);
        },
        save: function () {
            var measurements = {
                time: this.time,
                measurements: this.measurements.filter(m => m.value).map(m => { return { measureId: m.id, measureName: m.name, value: utils.parseFloat(m.value) } })
            };

            this.saveCallback(measurements);
        },
        cancel: function () {
            this.cancelCallback();
        },
        deleteMeasurement: function () {
            this.deleteCallback(this.exercise);
        }
    },
    created: function () {
        var self = this;
        this.time = utils.previousHalfHour();
        this.measurements = this.measures.map(m => { return { id: m.id, name: m.name, value: undefined} });
    }
}
</script>
<style scoped>
    #measurements 
    {
        max-width:1200px;
    }
</style>