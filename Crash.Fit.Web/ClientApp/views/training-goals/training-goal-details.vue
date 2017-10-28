<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("nutritionGoals") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-md-3 col-lg-2">
                    <div class="form-group">
                        <label>{{ $t("name") }}</label>
                        <input class="form-control" v-model="name" />
                    </div>

                </div>
            </div>
            <div class="exercise-separator row hidden-sm hidden-md hidden-lg">
                <div class="col-sm-12"><hr /></div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-2 col-text-40"><label>{{ $t("exercise") }}</label></div>
                        <div class="col-sm-2 col-text-10"><label :title="$t('frequencyInfo')" data-toggle="tooltip" data-placement="top">{{ $t("workoutsPerWeek") }} <i class="fa fa-question-circle-o"></i></label></div>
                        <div class="col-sm-2 col-number-5"><label>{{ $t("sets") }}</label></div>
                        <div class="col-sm-2 col-number-5"><label>{{ $t("reps") }}</label></div>    
                        <div class="col-sm-2 col-load"><label>{{ $t("load") }} (%)</label></div>
                        <div class="col-sm-2 col-actions-3">&nbsp;</div>
                    </div>
                    <template v-for="(exercise,index) in exercises">
                        <div class="row">
                            <div class="col-sm-2 col-text-40">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                <exercise-picker v-bind:exercises="exerciseOptions" v-bind:value="exercise.exercise" v-on:change="exercise.exercise=arguments[0]" v-on:nameChange="processNewExercise(exercise, arguments[0])" />
                            </div>
                            <div class="col-xs-4 col-text-10">
                                <label class="hidden-sm hidden-md hidden-lg" :title="$t('frequencyInfo')" data-toggle="tooltip" data-placement="top">{{ $t("workoutsPerWeek") }} <i class="fa fa-question-circle-o"></i></label>
                                <!--
                                <input type="number" min="0" step="0.166" class="form-control" v-model="exercise.frequency" />
                                -->
                                <div class="btn-group">
                                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ frequencyText(exercise.frequency) }} <span class="caret"></span></button>
                                    <ul class="dropdown-menu">
                                        <li v-for="frequency in frequencyOptions">
                                            <a @click="exercise.frequency = frequency.value">{{ frequency.text }}</a>
                                        </li>
                                        <li>
                                            <div class="input-group col-number-5">
                                              <input type="number" v-model="exercise.frequency" />
                                            </div>
                                        </li>
                                    </ul>
                                </div> 
                            </div>
                            <div class="col-xs-4 col-number-5">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("sets") }}</label>
                                <input type="number" min="0" step="1" class="form-control" v-model="exercise.sets" />
                            </div>
                            <div class="col-xs-4 col-number-5">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                <input type="number" min="0" step="1" class="form-control" v-model="exercise.reps" />
                            </div>  
                            <div class="col-xs-4 col-load">
                                <label class="hidden-sm hidden-md hidden-lg">{{ $t("load") }}</label>
                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadFrom" />
                                <i class="fa fa-minus"></i>
                                <input type="number" min="0" step="5" class="form-control" v-model="exercise.loadTo" />
                            </div>
                            <div class="col-xs-4 col-actions-3">
                                <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                <button class="btn btn-sm" @click="moveExerciseUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                <button class="btn btn-sm" @click="moveExerciseDown(index)" :disabled="index === (exercises.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                <button class="btn btn-danger btn-sm" @click="deleteExercise(index)">{{ $t("delete") }}</button>
                            </div>
                        </div>
                        <div class="exercise-separator row hidden-sm hidden-md hidden-lg">
                            <div class="col-sm-12"><hr /></div>
                        </div>
                    </template>
                </div>
            </div>
            <div class="row" v-if="exercises.length == 0">
              <div class="col-sm-12">
                <br />
                {{ $t("noExercises") }}
              </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12"><button class="btn" @click="addExercise">{{ $t("add") }}</button></div>
            </div>
            <hr />
            <div class="row" v-if="exercises.length > 0">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t('save') }}</button>
                </div>
            </div>
          
            <div class="row">
                <div class="col-sm-12">
                    <p>
                        {{ $t('trainingGoalsInfo') }}
                    </p>
                </div>
            </div>
        </section>
        
    </div>
</template>

<script src="./training-goal-details.js">
</script>

<style scoped>
    div.exercise-separator {
        padding: 0px;
    }

    div.exercise-separator hr {
        border: 1px solid #00c0ef;
    }
</style>