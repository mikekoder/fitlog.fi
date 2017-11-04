<template>
    <div  v-if="!loading">
        <section class="content-header"><h1>{{ $t("exerciseDetails") }}</h1></section>
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
                                    <button class="btn btn-danger btn-sm" @click="deleteTarget(index)">{{ $t("delete") }}</button>
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
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("bodyWeightPercentage") }}</label>
                        <div class="input-group">
                            <input class="form-control" v-model="percentageBW" />
                            <span class="input-group-addon">%</span>
                        </div>
                        
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

<script src="./exercise-details.js">
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