<template>
    <div v-if="!loading">
        <section class="content-header"><h1>{{ $t("workoutDetails") }}</h1></section>
        <section class="content">
            <div class="row">
                <div class="col-sm-5 col-datetime">
                    <div class="form-group">
                        <label>{{ $t("time") }}</label>
                        <datetime-picker :value="time" @change="val => time=val"></datetime-picker>
                    </div>
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="row hidden-xs">
                        <div class="col-sm-1 col-drag"></div>
                        <div class="col-sm-4 col-text-40"><label>{{ $t("exercise") }}</label></div>
                        <div class="col-sm-2 col-number-5"><label>{{ $t("reps") }}</label></div>
                        <div class="col-sm-3 col-number-5"><label>{{ $t("weights") }}</label></div>
                        <div class="col-sm-2 col-actions-4">&nbsp;</div>
                    </div>
                    <draggable :list="sets" :options="{handle:'.handle'}">
                        <div class="row" v-for="(set,index) in sets">
                            <div class="col-sm-12">
                                 <div class="row">
                                     <div class="col-sm-1 col-xs-1 col-drag">
                                         <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                         <i class="fa fa-bars handle" :title="$t('dragToReorder')"></i>
                                     </div>
                                    <div class="col-sm-4 col-xs-11 col-text-40">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("exercise") }}</label>
                                        <exercise-picker :exercises="exercises" :value="set.exercise" @change="val => setExercise(set,val)" @nameChange="val => processNewExercise(set, val)" />
                                    </div>
                                    <div class="col-xs-4 col-number-5">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("reps") }}</label>
                                        <input type="number" min="0" class="form-control" v-model="set.reps" />
                                    </div>
                                    <div class="col-xs-4 col-number-5">
                                        <label class="hidden-sm hidden-md hidden-lg">{{ $t("weights") }}</label>
                                        <input type="number" min="0" step="2.5" class="form-control" v-model="set.weights" />
                                    </div>
                                    <div class="col-xs-4 col-actions-4">
                                        <label class="hidden-sm hidden-md hidden-lg">&nbsp;</label>
                                        <!--
                                        <button class="btn btn-sm" @click="moveSetUp(index)" :disabled="index === 0"><i class="fa fa-arrow-up"></i></button>
                                        <button class="btn btn-sm" @click="moveSetDown(index)" :disabled="index === (sets.length - 1)"><i class="fa fa-arrow-down"></i></button>
                                        -->
                                        <button class="btn btn-primary btn-sm" @click="copySet(index)">{{ $t("copy") }}</button>
                                        <button class="btn btn-danger btn-sm" @click="deleteSet(index)">{{ $t("delete") }}</button>
                                    </div>
                            </div>
                            <div class="workout-set-separator row hidden-sm hidden-md hidden-lg">
                                <div class="col-sm-12"><hr /></div>
                            </div>
                            </div>
                        </div>
                    </draggable>
                    

                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                <div class="col-sm-12"><button class="btn" @click="addSet">{{ $t("add") }}</button></div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <hr />
                </div>
            </div>
            <div class="row">
                <div class="col-sm-5 col-datetime">
                    <div class="form-group">
                        <label>{{ $t("duration") }}</label>
                        <datetime-picker :value="duration" :format="'HH:mm'" @change="val => duration=val"></datetime-picker>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-5 col-datetime">
                    <div class="form-group">
                        <label>{{ $t("energyExpenditure") }}<span v-if="energyExpenditureEstimate && energySpecified" class="estimate"> {{ $t('estimate') }}: {{ formatDecimal(energyExpenditureEstimate) }} kcal</span></label>
                        <input type="number" min="0" v-model="energyExpenditure" @blur="energySpecified=true" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <hr />
                </div>
            </div>
            <div class="row main-actions">
                <div class="col-sm-12">
                    <button class="btn btn-primary" @click="save">{{ $t("save") }}</button>
                    <button class="btn" @click="cancel">{{ $t("cancel") }}</button>
                    <button class="btn btn-danger" v-if="id" @click="deleteWorkout">{{ $t("delete") }}</button>
                </div>
            </div>
            <hr />
            <div class="row">
                <table>
                    <tbody></tbody>
                </table>
            </div>
        </section>
    </div>
</template>

<script src="./workout-details.js">
</script>
<style scoped>
    div.workout-set
    {
        margin-bottom:5px;
    }
    div.workout-set-separator
    {
        padding: 0px;
    }
    div.workout-set-separator hr
    {
        border: 1px solid #00c0ef;
    }
    div.food, div.quantity, div.portion, div.weight, div.actions
    {
        padding-right:2px;
    }
    div.weight 
    {
        padding-top:5px;
    }
    span.estimate{
        font-weight: normal;
        font-size: smaller;
    }
    @media (max-width: 767px) {
        div.food, div.quantity, div.portion, div.weight, div.actions
        {
            padding-right:15px;
        }
        div.actions
        {
            text-align:right;
            padding-top:15px;
        }
        div.actions button
        {
            margin-top:10px;
            margin-right:0px;
        }
    }
</style>