<template>
    <q-modal ref="modal" :class="{desktop: isDesktop }">
      <h5>Ruoka-aine</h5>
      
        <q-tabs v-model="tab" v-if="selectExercise">
            <!-- Tabs - notice slot="title" -->
            <q-tab slot="title" name="tab-1" icon="fa-search" />
            <q-tab slot="title" name="tab-2" icon="fa-clock-o" />
            <q-tab slot="title" name="tab-3" icon="fa-star" />
            <!--
            <q-tab slot="title" name="tab-4" icon="fa-user" />
            -->
            <!-- Targets -->
            <q-tab-pane name="tab-1">
                <q-search v-model="searchText" :placeholder="$t('exercise')" @change="exerciseChange">
                    <q-autocomplete @search="search" @selected="exerciseSelected" :min-characters="2" :max-results="20" />
                </q-search>
                <q-btn @click="selectExercise=false" v-if="exercise">{{ $t('ok') }}</q-btn>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(e, index) in latestExercises" @click="load(e.id)" v-bind:class="{selected: exercise && e.id == exercise.id }" :key="index">{{ e.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(f, index) in mostUsedExercises" @click="load(f.id)" v-bind:class="{selected: exercise && f.id == exercise.id }" :key="index">{{ f.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
            <!--
            <q-tab-pane name="tab-4">
                <q-scroll-area>
                    <q-list>
                        <q-item v-for="(f, index) in ownExercises" @click="load(f.id)" v-bind:class="{selected: exercise && f.id == exercise.id }" :key="index">{{ f.name }}</q-item>
                    </q-list>
                </q-scroll-area>
            </q-tab-pane>
            -->
        </q-tabs>
      
      <div class="row pad" v-if="!selectExercise">     
        <q-btn @click="reselectExercise">{{ exercise.name }}</q-btn>
      </div>
      <div class="row pad" v-if="!selectExercise">     
        <q-input v-model="sets" type="number" v-if="exercise" :float-label="$t('sets')" />
      </div>
      <div class="row pad" v-if="!selectExercise">     
        <q-input v-model="reps" type="number" v-if="exercise" :float-label="$t('reps')" />
      </div>
      <div class="row pad" v-if="!selectExercise">     
        <q-input v-model="weights" type="number" v-if="exercise" :float-label="$t('weights')" />
      </div>
      <div class="row pad buttons">
        <q-btn @click="close">{{ $t('cancel') }}</q-btn>
        <q-btn color="primary" @click="save" v-if="exercise && quantity">{{ $t('save') }}</q-btn>
      </div>
    </q-modal>
</template>

<script>
    import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
    import constants from '../../store/constants'
    import api from'../../api'

export default {
    data () {
        return {
            tab: 'tab-1',
            selectExercise: true,

            id: undefined,
            workoutId: undefined,
            exercise: undefined,
            sets: undefined,
            reps: undefined,
            weights: undefined,

            searchText: undefined
        }
    },
    props: {
        row: undefined
    },
    computed: {
        canSave() {
            return this.exercise && this.reps;
        },
        latestExercises(){
            return this.$store.state.training.latestExercises.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        mostUsedExercises(){
            return this.$store.state.training.mostUsedExercises.sort((a,b) => a.name < b.name ? -1 : 1);
        }/*,
        
        ownExercises(){
            return this.$store.state.training.ownExercises.sort((a,b) => a.name < b.name ? -1 : 1);
        }*/
    },
    components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
    methods: {
        open(row){
            var self = this;
            self.workoutId = row.workoutId;
            self.sets = row.sets;
            self.reps = row.reps;
            self.weights = row.weights;
            var exerciseId = row.exercise ? row.exercise.id : row.exerciseId;
        
            if(exerciseId){
                self.load(exerciseId);
            }
            
            self.$refs.modal.open();
        },
        search(text, done){
          var self = this;
          api.searchExercises(text).then(results => {
            done(results.map(f => { return { ...f, value: f.name, label: f.name, icon: f.userId ? 'fa-user' : '' }}));
          });
          
        },
        exerciseChange(text){
            if(this.exercise && text.length < this.exercise.name.length){
                this.exercise = undefined;
            }
        },
        exerciseSelected(exercise){
          this.load(exercise.id);
        },
        load(exerciseId){
            var self = this;
            self.$store.dispatch(constants.FETCH_EXERCISE, {
                id: exerciseId,
                success (exercise) {
                    self.searchText = exercise.name;
                    self.exercise = exercise;
                    self.selectExercise = false;
                },
                failure () {
                    //toaster.error(self.$t('fetchFailed'));
                }
            });
        },
        reselectExercise(){
            this.selectExercise = true;
        },
        tabChanged(tab){
            this.tab = tab;
        },
        close () {
            this.searchText = '';
            this.exercise = undefined;
            this.sets = undefined;
            this.reps = undefined;
            this.weights = undefined;
            this.selectExercise = true;
            this.$refs.modal.close();
        },
        save () {
            var self = this;
            var row = {
                id: self.id,
                mealDefinitionId: self.mealDefinitionId,
                mealId: self.mealId,
                exercise: self.exercise,
                exerciseId: self.exercise.id,
                exerciseName: self.exercise.name,
                quantity: self.quantity,
                portion: self.portion.value,
                portionId: self.portion ? self.portion.id : undefined,
                portionName: self.portion ? self.portion.name : undefined
            };
            this.$emit('save', row);
        }
    },
    mounted () {

    }
}
</script>

<style scoped>
.q-select { min-width: 50%;}
button{margin-bottom: 10px;}
.selected{background: hsla(0,0%,74%,.5);}
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
</style>
