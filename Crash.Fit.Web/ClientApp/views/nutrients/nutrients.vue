<template>
    <div>
        <section class="content-header"><h1>Ravintoaineet</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12">
                    <table class="nutrient-settings">
                        <thead>
                            <tr><th></th><th></th><th></th><th colspan="2">N&auml;kyvyys</th></tr>
                            <tr><th></th><th></th><th></th><th>Yhteenveto</th><th>Yksityiskohdat</th></tr>
                        </thead>
                        <tbody v-for="group in groups">
                            <tr><th></th><th colspan="2">{{ group.name }}</th></tr>
                            <tr v-for="(nutrient, index) in nutrientSettings[group.id]">
                                <td>
                                    <button class="btn btn-sm" @click="moveNutrientUp(group.id, index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                    <button class="btn btn-sm" @click="moveNutrientDown(group.id, index)" :disabled="index === (nutrientSettings[group.id].length - 1)"><i class="fa fa-arrow-down"></i></button>
                                </td>
                                <td>{{ nutrient.name }}</td>
                                <td>{{ unit(nutrient.unit) }}</td>
                                <td>
                                    <select v-model="nutrient.hideSummary">
                                        <option value="null">Oletus ({{ nutrient.defaultHideSummary ? 'piilota' : 'n&auml;yt&auml;'}})</option>
                                        <option value="false">N&auml;yt&auml;</option>
                                        <option value="true">Piilota</option>
                                    </select>
                                </td>
                                <td>
                                    <select v-model="nutrient.hideDetails">
                                        <option value="null">Oletus ({{ nutrient.defaultHideDetails ? 'piilota' : 'n&auml;yt&auml;'}})</option>
                                        <option value="false">N&auml;yt&auml;</option>
                                        <option value="true">Piilota</option>
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
                    <button class="btn btn-primary">Tallenna</button>
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

module.exports = {
    data () {
        return {
            nutrientSettings: {},
        }
    },
    computed:{
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
            }, failure: function () { }
        });
    }
}
</script>

<style scoped>
    .nutrient-settings thead th { text-align: center;}
    .nutrient-settings td { padding: 1px 5px;}
</style>