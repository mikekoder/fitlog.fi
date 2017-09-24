<template>
    <div v-if="!loading">
        <div v-if="!create">
            <section class="content-header"><h1>{{ $t("measurements") }}</h1></section>
            <section class="content">
                <div class="row">
                    <div class="col-sm-12">
                        <button class="btn btn-primary" @click="createMeasurements">{{ $t("create") }}</button>
                    </div>
                </div>
                <div class="row" v-if="measures.length > 0">
                    <div class="col-sm-12">
                        <table class="table" id="measure-list">
                            <thead>
                                <tr>
                                    <th>{{ $t("measure") }}</th>
                                    <th>{{ $t("value") }}</th>
                                    <th>{{ $t("time") }}</th>
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
                        {{ $t("noMeasurements") }}
                    </div>
                </div>
            </section>
        </div>
        <div v-if="create">
            <section class="content-header"><h1>{{ $t("measurements") }}</h1></section>
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
import constants from '../../store/constants'
import api from '../../api'
import toaster from '../../toaster'
import formatters from '../../formatters'

export default {
    data () {
        return {
            measures: [],
            create: false
        }
    },
    computed:{
    },
    components: {
        'measurements-editor': require('./measurements-editor')
    },
    methods: {
        loadMeasures() {
            var self = this;
            api.listMeasures().then((measures) => {
                self.measures = measures;
                self.$store.commit(constants.LOADING_DONE);
            });
        },
        createMeasurements() {
            this.create = true;
        },
        saveMeasurements(measurements) {
            var self = this;
            api.saveMeasurements(measurements).then(() => {
                self.loadMeasures();
                self.create = false;
            });
        },
        cancelMeasurements() {
            this.create = false;
        },
        datetime: formatters.formatDateTime
    },
    created() {
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