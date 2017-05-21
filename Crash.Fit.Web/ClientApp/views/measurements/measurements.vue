<template>
    <div>
        <div v-if="!create">
            <section class="content-header"><h1>Mitat</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createMeasurements"><i class="glyphicon glyphicon-plus"></i> Lis&auml;&auml; mitat</button>
                    </div>
                </div>
                <div class="row" v-if="measures.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="measure-list">
                            <thead>
                                <tr>
                                    <th>Mitta</th>
                                    <th>Arvo</th>
                                    <th>Aika</th>
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
                        Ei mittoja
                    </div>
                </div>
            </section>
        </div>
        <div v-if="create">
            <section class="content-header"><h1>Mitat</h1></section>
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
    components: {
        'measurements-editor': require('./measurements-editor')
    },
    methods: {
        loadMeasures: function () {
            var self = this;
            api.listMeasures().then(function (measures) {
                self.measures = measures;
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
        /*
        var id = this.$route.params.id;
        if (id) {
            this.editExercise(id);
        }*/
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