<template>
    <div v-if="!loading">
        <div v-if="!create">
            <section class="content-header"><h1>{{ $t("measurements.title") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createMeasurements">{{ $t("measurements.create") }}</button>
                    </div>
                </div>
                <div class="row" v-if="measures.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="measure-list">
                            <thead>
                                <tr>
                                    <th>{{ $t("measurements.columns.measure") }}</th>
                                    <th>{{ $t("measurements.columns.value") }}</th>
                                    <th>{{ $t("measurements.columns.time") }}</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="measure in measures">
                                    <td>{{ measure.name }}</td>
                                    <td>{{ measure.latestValue }}</td>
                                    <td>{{ datetime(measure.latestTime) }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="row" v-if="measures.length == 0">
                    <div class="col-sm-12">
                        <br />
                        {{ $t("measurements.noMeasurements") }}
                    </div>
                </div>
            </section>
        </div>
        <div v-if="create">
            <section class="content-header"><h1>{{ $t("measurements.title") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <measurements-editor v-bind:measures="measures" v-bind:saveCallback="saveMeasurements" v-bind:cancelCallback="cancelMeasurements" />
                    </div>
                </div>
            </section>
        </div>
    </div>
</template>

<script>
var constants = require('../../store/constants')
var api = require('../../api');
var toaster = require('../../toaster');
var formatters = require('../../formatters')

module.exports = {
    data () {
        return {
            measures: [],
            create: false
        }
    },
    computed:{
        loading: function() {
            return this.$store.state.loading;
        }
    },
    components: {
        'measurements-editor': require('./measurements-editor')
    },
    methods: {
        loadMeasures: function () {
            var self = this;
            api.listMeasures().then(function (measures) {
                self.measures = measures;
                self.$store.commit(constants.LOADING_DONE);
            });
        },
        createMeasurements: function () {
            this.create = true;
        },
        saveMeasurements: function (measurements) {
            var self = this;
            api.saveMeasurements(measurements).then(function () {
                self.loadMeasures();
                self.create = false;
            });
        },
        cancelMeasurements: function () {
            this.create = false;
        },
        datetime: formatters.formatDateTime
    },
    created: function () {
        this.loadMeasures();
    },
    beforeRouteUpdate (to, from, next) {
        /*
        if (to.params.id) {
            this.editExercise(to.params.id);
        }
        else {
            this.showSummary();
        }*/
        next();
    }
}
</script>

<style scoped>
      #measure-list{
        width: auto;
        table-layout: fixed; 
        /*width: 100%;*/
    }
    #measure-list td {
        padding-bottom: 0px;
    }
    #measure-list td span{
        margin: 5px;
    }
</style>