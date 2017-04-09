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
                <div class="row">
                    <div class="col-xs-8 col-sm-6 col-md-3"><label>Lihasryhmä</label></div>
                    <div class="col-xs-4 col-sm-2">&nbsp;</div>
                </div>
                <template v-for="(target,index) in targets">
                    <div class="exercise-target row">
                        <div class="col-xs-8 col-sm-6 col-md-3">
                            <select class="form-control" v-model="targets[index]">
                                <option v-for="musclegroup in muscleGroups" v-bind:value="musclegroup">
                                    {{ musclegroup.name }}
                                </option>
                            </select>
                        </div>
                        <div class="col-xs-4 col-sm-2">
                            <div>
                                <button class="btn btn-danger" @click="removeTarget(index)">Poista</button>
                            </div>
                        </div>
                    </div>
                    <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                        <div class="col-sm-12"><hr /></div>
                    </div>
                </template>
                <div class="row">
                    <div class="col-xs-12"><button class="btn" @click="addTarget"><i class="fa fa-plus"></i> Lisää</button></div>
                </div>
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
            name: null,
            targets: [],
            muscleGroups: []
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
        addTarget: function(){
            this.targets.push(undefined);
        },
        removeTarget: function(index){
            this.targets.splice(index, 1);
        },
        save: function () {
            var exercise = {
                id: this.id,
                name: this.name,
                targets: this.targets.map(t => t.id)
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
        this.id = this.exercise.id;
        this.name = this.exercise.name;
        api.listMuscleGroups().then(function (muscleGroups) {
            self.muscleGroups = muscleGroups;
            if (self.exercise.targets) {
                self.targets = self.exercise.targets.map(t => {
                    return self.muscleGroups.filter(g => g.id === t)[0];
                });
            }
        });
    }
}
</script>
<style scoped>
    div.exercise-target {
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