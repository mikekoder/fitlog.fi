<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutrients") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <table class="nutrient-settings">
                        <thead>
                            <tr><th></th><th></th><th></th><th colspan="2">{{ $t("visibility") }}</th></tr>
                            <tr><th></th><th></th><th></th><th>{{ $t("summary") }}</th><th>{{ $t("details") }}</th></tr>
                        </thead>
                        <tbody v-for="group in groups">
                            <tr><th></th><th colspan="2">{{ $t(group.id) }}</th></tr>
                            <tr v-for="(nutrient, index) in nutrientSettings[group.id]">
                                <td>
                                    <button class="btn btn-sm" @click="moveNutrientUp(group.id, index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                    <button class="btn btn-sm" @click="moveNutrientDown(group.id, index)" :disabled="index === (nutrientSettings[group.id].length - 1)"><i class="fa fa-arrow-down"></i></button>
                                </td>
                                <td>{{ nutrient.name }}</td>
                                <td>{{ unit(nutrient.unit) }}</td>
                                <td>
                                    <select v-model="nutrient.userHideSummary">
                                        <option value="null">{{ $t("default") }} ({{ nutrient.defaultHideSummary ? $t("hide") : $t("show")}})</option>
                                        <option value="false">{{ $t("show") }}</option>
                                        <option value="true">{{ $t("hide") }}</option>
                                    </select>
                                </td>
                                <td>
                                    <select v-model="nutrient.userHideDetails">
                                        <option value="null">{{ $t("default") }} ({{ nutrient.defaultHideDetails ? $t("hide") : $t("show")}})</option>
                                        <option value="false">{{ $t("show") }}</option>
                                        <option value="true">{{ $t("hide") }}</option>
                                    </select>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <hr />
            <div class="row">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                </div>
            </div>

        </section>
        
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var utils = require('../../utils');
    var api = require('../../api');
    var formatters = require('../../formatters')
    var toaster = require('../../toaster');
module.exports = {
    data () {
        return {
            nutrientSettings: {},
        }
    },
    computed: {
        loading: function () {
            return this.$store.state.loading;
        },
        groups: function(){
            return this.$store.state.nutrition.nutrientGroups;
        }
    },
    components: {},
    methods: {
        moveNutrientUp: function (group, index) {
            var group = this.nutrientSettings[group];
            var nutrient = group[index];
            group.splice(index, 1);
            group.splice(index - 1, 0, nutrient);
        },
        moveNutrientDown: function (group, index) {
            var group = this.nutrientSettings[group];
            var nutrient = group[index];
            group.splice(index, 1);
            group.splice(index + 1, 0, nutrient);
        },
        save: function () {
            var self = this;
            var settings = [];
            for (var i in self.nutrientSettings) {
                for (var j in self.nutrientSettings[i]) {
                    var nutrient = self.nutrientSettings[i][j];
                    settings.push({ nutrientId: nutrient.id, userHideSummary: nutrient.userHideSummary, userHideDetails: nutrient.userHideDetails })
                }
            }
            self.$store.dispatch(constants.SAVE_NUTRIENT_SETTINGS, {
                settings,
                success: function () {
                    toaster.info(self.$t('nutrients.saved'));
                },
                failure: function () {
                    toaster.error(self.$t('nutrients.saveFailed'));
                }
            });
        },
        unit: formatters.formatUnit
    },
    created: function () {
        var self = this;
        this.$store.dispatch(constants.FETCH_NUTRIENTS, {
            success: function (nutrients) {
                var grouped = {};
                for (var i in nutrients) {
                    var nutrient = nutrients[i];
                    if (grouped[nutrient.fineliGroup]) {
                        grouped[nutrient.fineliGroup].push(nutrient);
                    }
                    else {
                        grouped[nutrient.fineliGroup] = [nutrient];
                    }

                }
                self.nutrientSettings = grouped;
                self.$store.commit(constants.LOADING_DONE);
            }, failure: function () { }
        });
    }
}
</script>

<style scoped>
    .nutrient-settings thead th { text-align: center;}
    .nutrient-settings td { padding: 1px 5px;}
</style>