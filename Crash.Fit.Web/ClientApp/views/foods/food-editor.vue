<template>
    <div>
        <div class="row">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Nimi</label>
                    <input class="form-control" v-model="name" />
                </div>
                
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12">
                <table>
                    <thead>
                        <tr>
                            <th class="food">Ruoka</th>
                            <th class="quantity">Määrä</th>
                            <th class="portion">Annos</th>
                            <th class="weight">Paino</th>
                            <th class="actions"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(row,index) in rows">
                            <td><div v-if="copyMode"><input type="checkbox" v-model="row.copy" /></div></td>
                            <td class="food"><food-picker v-bind:value="row.food" v-on:change="row.food=arguments[0]" /></td>
                            <td class="quantity"><input type="text" class="form-control" v-model="row.quantity" /></td>
                            <td class="portion">
                                <select class="form-control" v-if="row.food" v-model="row.portion">
                                    <option v-bind:value="null">g</option>
                                    <option v-for="portion in row.food.portions" v-bind:value="portion">
                                        {{ portion.name }}
                                    </option>
                                </select>
                            </td>
                            <td class="weight">
                                <div v-if="row.food">
                                    <span v-if="row.food">{{ weight(row.quantity, row.portion) }}</span>
                                </div>
                            </td>
                            <td class="actions"><button class="btn btn-link" @click="removeRow(index)">Poista</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-sm-12">
                <button class="btn btn-primary" @click="save">Tallenna</button>
                <button class="btn" @click="cancel">Peruuta</button>
                <button class="btn btn-link" v-if="id">Poista</button>
            </div>
        </div>
        <hr />
        <div class="row">
            <table>
                <tbody>

                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');

module.exports = {
    data () {
        return {
            id: null,
            name: null,
            nutrients: {},
            allNutrients: []
        }
    },
    computed: {
    },
    props: {
        food: null,
        user: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    methods: {
        save: function () {
            var food = {
                id: this.id,
                name: this.name,
                rows: this.rows
            };
            this.saveCallback(food);
        },
        cancel: function () {
            this.cancelCallback();
        },
        delete: function(){ },
        startCopy: function () {
            this.copyMode = true;
            this.copyAllRows = true;
        },
        confirmCopy: function () { },
        cancelCopy: function () {
            this.copyMode = false;
            this.copyAllRows = true;
        },
        unit: function (unit) {
            switch (unit) {
                case 'G':
                    return 'g';
                case 'MG':
                    return 'mg';
                case 'UG':
                    return '\u03BCg';
                default:
                    return unit;
            }
        }
    },
    watch: {
        copyAllRows: function (newVal) {
            for (var i in this.rows) {
                this.rows[i].copy = newVal;
            }
        }
    },
    created: function () {
        var self = this;
        this.name = this.food.name;
        var nutrients = {};
        for (var i in this.food.nutrients) {
            nutrients[i] = this.food.nutrients[i];
        }
        api.listNutrients().then(function (allNutrients) {
            self.allNutrients = allNutrients;
        });
        this.nutrients = nutrients;
    },
    mounted: function () {
    }
}
</script>
<style scoped>
    th.food { width: 200px; }
    th.quantity { width: 80px; }
    th.portion { width: 120px; }
    th.weight { width: 80px; }
</style>