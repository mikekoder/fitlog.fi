<template>
    <div  v-if="!loading">
        <section class="content-header"><h1>{{ $t("exerciseDetails.title") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="row">
                        <div class="col-xs-8 col-sm-6 col-md-3"><label>{{ $t("muscleGroups") }}</label></div>
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
                                    <button class="btn btn-danger btn-sm" @click="removeTarget(index)">{{ $t("delete") }}</button>
                                </div>
                            </div>
                        </div>
                        <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-xs-12"><button class="btn" @click="addTarget">{{ $t("add") }}</button></div>
                    </div>
                </div>
            </div>
            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" v-if="id" @click="deleteExercise">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script>
    var constants = require('../../store/constants')
    var api = require('../../api');
    var formatters = require('../../formatters');
    var toaster = require('../../toaster');
module.exports = {
    data () {
        return {
            id: null,
            name: null,
            targets: []
        }
    },
    computed: {
        loading: function () {
            return this.$store.state.loading;
        },
        muscleGroups: function () {
            return this.$store.state.training.muscleGroups;
        }
    },
    methods: {
        addTarget: function(){
            this.targets.push(undefined);
        },
        removeTarget: function(index){
            this.targets.splice(index, 1);
        },
        save: function () {
            var self = this;

            var exercise = {
                id: self.id,
                name: self.name,
                targets: self.targets.map(t => t.id)
            };

            self.$store.dispatch(constants.SAVE_EXERCISE, {
                exercise,
                success: function () {
                    self.$router.replace({ name: 'exercises' });
                },
                failure: function () {
                    toaster(self.$t('exerciseDetails.saveFailed'));
                }
            });
        },
        cancel: function () {
            this.$router.go(-1);
        },
        deleteExercise: function () {
            var self = this;
            self.$store.dispatch(constants.DELETE_EXERCISE, {
                exercise: { id: self.id },
                success: function () {
                    self.$router.push({ name: 'exercises' });
                },
                failure: function () {
                    toaster(self.$t('exerciseDetails.deleteFailed'));
                }
            });
        }
    },
    created: function () {
        var self = this;
        var id = this.$route.params.id;
        self.$store.dispatch(constants.FETCH_MUSCLEGROUPS, {
            success: function () {
                if (id == constants.NEW_ID) {
                    self.id = undefined;
                    self.name = undefined;
                    self.targets = [];
                    self.$store.commit(constants.LOADING_DONE);
                }
                else {
                    self.$store.dispatch(constants.FETCH_EXERCISE, {
                        id, 
                        success: function (exercise) {
                            self.id = exercise.id;
                            self.name = exercise.name;
                            self.targets = exercise.targets.map(t => { return self.muscleGroups.find(g => g.id === t); });
                            self.$store.commit(constants.LOADING_DONE);
                        },
                        failure: function () {
                            toaster(self.$t('exerciseDetails.fetchFailed'));
                        }
                    });
                }
            },
            failure: function () {
                toaster(self.$t('exerciseDetails.fetchFailed'));
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