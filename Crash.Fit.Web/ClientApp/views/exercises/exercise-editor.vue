<template>
    <div>
        <div class="row" v-if="!anon">
            <div class="col-sm-5 col-md-3 col-lg-2">
                <div class="form-group">
                    <label>Nimi</label>
                    <input class="form-control" v-model="name" />
                </div>

            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-12"></div>
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
                <tbody></tbody>
            </table>
        </div>
    </div>
</template>

<script>
    var api = require('../../api');
    var formatters = require('../../formatters');

module.exports = {
    data () {
        return {
            id: null,
            name: null
        }
    },
    computed: {
    },
    props: {
        exercise: null,
        saveCallback: null,
        cancelCallback: null,
        deleteCallback: null
    },
    components: {
    },
    methods: {
        save: function () {
            var exercise = {
                id: this.id,
                name: this.name
            };

            this.saveCallback(exercise);
        },
        cancel: function () {
            this.cancelCallback();
        },
        delete: function(){ }
    },
    created: function () {
        var self = this;
        this.name = this.exercise.name;
        this.portions = this.exercise.portions || [];
        api.listNutrients().then(function (allNutrients) {
            var nutrients = {};
            for (var i in allNutrients) {
                var nutrient = allNutrients[i];
                nutrient.amount = undefined;
                if (self.exercise.nutrients) {
                    var exerciseNutrient = self.exercise.nutrients.find(fn => fn.nutrientId == nutrient.id);
                    if (exerciseNutrient) {
                        nutrient.amount = exerciseNutrient.amount;
                    }
                }
                if (nutrients[nutrient.fineliGroup]) {
                    nutrients[nutrient.fineliGroup].push(nutrient);
                }
                else {
                    nutrients[nutrient.fineliGroup] = [nutrient];
                }

            }
            self.nutrients = nutrients;           
        });
    }
}
</script>
<style scoped>
    div.recipe-row {
        margin-bottom: 5px;
    }

    div.recipe-row-separator {
        padding: 0px;
    }

        div.recipe-row-separator hr {
            border: 1px solid #00c0ef;
        }

    div.exercise, div.quantity, div.portion, div.weight, div.actions {
        padding-right: 2px;
    }

    div.weight {
        padding-top: 5px;
    }

    @media (max-width: 767px) {
        div.exercise, div.quantity, div.portion, div.weight, div.actions {
            padding-right: 15px;
        }

        div.actions {
            text-align: right;
        }

            div.actions button {
                margin-top: 10px;
                margin-right: 0px;
            }
    }
</style>