<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t('routineDetails') }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-12 col-text-30">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input type="text" class="form-control" v-model="name" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <h4>{{ $t("workouts") }}</h4>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <template v-for="(workout,index) in workouts">
                        <div class="box box-solid">
                            <div class="box-header with-border">
                                <div class="row hidden-xs">
                                    <div class="col-xs-6 col-text-30">{{ $t('name') }}</div>
                                    <div class="col-xs-6 col-text-20">{{ $t('frequency') }}</div>
                                    <div class="col-xs-6 col-actions-3"></div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-6 col-text-30">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("name") }}</label>
                                        <input type="text" class="form-control" v-model="workout.name" />
                                    </div>
                                    <div class="col-xs-6 col-text-20">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("frequency") }}</label>
                                        <div class="btn-group">
                                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ frequencyText(workout.frequency) }} <span class="caret"></span></button>
                                            <ul class="dropdown-menu">
                                                <li v-for="frequency in frequencyPresets">
                                                    <a @click="workout.frequency = frequency.value">{{ frequency.text }}</a>
                                                </li>
                                                <li>
                                                    <div class="input-group">
                                                        <input type="number" class="form-control" v-model="workout.frequency" />
                                                        <span class="input-group-addon">
                                                            {{ $t('timesAbbr') }} / {{ $t('weekAbbr') }}
                                                        </span>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div> 
                                    </div>
                                    <div class="col-xs-6 col-actions-3">
                                        <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                        <button class="btn btn-sm" @click="moveWorkoutUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                        <button class="btn btn-sm" @click="moveWorkoutDown(index)" :disabled="index === (workouts.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                        <button class="btn btn-danger pull-right btn-sm" @click="deleteWorkout(index)">Poista</button>
                                    </div>
                                </div>
                            </div>
                            <div class="box-body">
                                <div class="workout-details">
                                    <div class="row hidden-xs">
                                        <div class="col-sm-4 col-text-30"><label>{{ $t("exercise") }}</label></div>
                                        <div class="col-sm-2 col-number-5"><label>{{ $t("sets") }}</label></div>
                                        <div class="col-sm-2 col-number-5"><label>{{ $t("reps") }}</label></div>
                                        <div class="col-sm-2 col-load"><label>{{ $t("load") }} (%)</label></div>
                                        <div class="col-sm-4 col-actions-3">&nbsp;</div>
                                    </div>
                                    <template v-for="(exercise,index) in workout.exercises">
                                        <div class="row">
                                            <div class="col-sm-4 col-text-30">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                                <exercise-picker :exercises="exercises" :value="exercise.exercise" @change="val => exercise.exercise=val" @nameChange="val => processNewExercise(exercise, val)" />
                                            </div>
                                            <div class="col-xs-4 col-number-5">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("sets") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.sets" />
                                            </div>
                                            <div class="col-xs-4 col-number-5">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                                <input type="number" min="1" step="1" class="form-control" v-model="exercise.reps" />
                                            </div>
                                            <div class="col-xs-4 col-load">
                                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("load") }}</label>
                                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadFrom" />
                                                <i class="fa fa-arrow-right"></i>
                                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadTo" />
                                            </div>
                                            <div class="actions col-sm-4 col-xs-4 col-actions-3">
                                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                                <button class="btn btn-sm" @click="moveExerciseUp(workout,index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                                <button class="btn btn-sm" @click="moveExerciseDown(workout,index)" :disabled="index === (workout.exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                                <button class="btn btn-danger btn-sm" @click="deleteExercise(workout,index)">{{ $t("delete") }}</button>
                                            </div>
                                        </div>
                                        <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                                            <div class="col-sm-12"><hr /></div>
                                        </div>
                                    </template>
                                    <div class="row">&nbsp;</div>
                                    <div class="row">
                                        <div class="col-sm-12"><button class="btn" @click="addExercise(workout)">{{ $t("add") }}</button></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </template>
                    <div class="row">
                        <div class="col-sm-12">
                            <button class="btn" @click="addWorkout">{{ $t("addWorkout") }}</button>
                        </div>
                    </div>
                </div>
            </div>

            <hr />
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger btn-sm" v-if="id" @click="deleteWorkout">{{ $t("delete") }}</button>
                </div>
            </div>
        </section>
    </div>
</template>

<script src="./routine-details.js">
</script>
<style scoped>
    div.workout-header
    {
        background-color: #ccc;
        padding: 10px;
    }
    div.workout-details 
    {
        padding-bottom: 20px;
    }
    .box 
    {
        max-width: 1000px;
    }

    label{ display:block;}
    input{ padding: 6px;}
</style>