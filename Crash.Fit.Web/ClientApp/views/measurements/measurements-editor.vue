<template>
    <div id="measurements">
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>{{ $t("time") }}</label>
                    <datetime-picker class="vue-picker1" name="picker1" v-bind:value="time" v-on:change="time=arguments[0]"></datetime-picker>
                </div>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                <div class="row">
                    <div class="hidden-xs col-sm-4 col-md-2"><label>{{ $t("measure") }}</label></div>
                    <div class="hidden-xs col-xs-4 col-sm-2"><label>{{ $t("value") }}</label></div>
                    <div class="hidden-xs col-xs-4 col-sm-2">&nbsp;</div>
                </div>
                <template v-for="(measurement,index) in measurements">
                    <div class="measurement row">
                        <div class="col-sm-4 col-md-2">
                            <label class="hidden-sm hidden-md hidden-lg">{{ $t("measure") }}</label>
                            <span v-if="measurement.id">{{ measurement.name }}</span>
                            <input type="text" class="form-control" v-else v-model="measurement.name" />
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <label class="hidden-sm hidden-md hidden-lg">{{ $t("value") }}</label>
                            <input type="number" class="form-control" v-model="measurement.value" />
                        </div>
                        <div class="col-xs-4 col-sm-2">
                            <div>
                                <button class="btn btn-danger btn-sm" v-if="!measurement.id" @click="deleteMeasurement(index)">{{ $t("delete") }}</button>
                            </div>
                        </div>
                    </div>
                    <div class="measure-separator row hidden-sm hidden-md hidden-lg">
                        <div class="col-sm-12"><hr /></div>
                    </div>
                </template>
                <div class="row">
                    <div class="col-sm-12">&nbsp;</div>
                </div>
                <div class="row">
                    <div class="col-xs-12"><button class="btn" @click="addMeasurement">{{ $t("add") }}</button></div>
                </div>
            </div>
        </div>
        <hr />
        <div class="row main-actions">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
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
    import constants from '../../store/constants'
    import api from '../../api'
    import formatters from '../../formatters'
    import utils from '../../utils'

export default {
    data () {
        return {
            time: null,
            measurements: null,
        }
    },
    computed: {
    },
    props: {
        measures: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
        'datetime-picker': require('../../components/datetime-picker'),
    },
    methods: {
        addMeasurement(){
            this.measurements.push({name: undefined, value:undefined});
        },
        deleteMeasurement(index){
            this.measurements.splice(index, 1);
        },
        save() {
            var measurements = {
                time: this.time,
                measurements: this.measurements.filter(m => m.value).map(m => { return { measureId: m.id, measureName: m.name, value: utils.parseFloat(m.value) } })
            };

            this.saveCallback(measurements);
        },
        cancel() {
            this.cancelCallback();
        },
        deleteMeasurement() {
            this.deleteCallback(this.exercise);
        }
    },
    created() {
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