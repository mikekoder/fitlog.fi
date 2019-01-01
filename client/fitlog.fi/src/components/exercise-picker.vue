<template>
    <q-modal ref="exercisePickerModal" maximized>
      <q-modal-layout view="LHh lpR fff" :container="isDesktop" :class="{'boxed': isDesktop }">  
        <q-tabs v-model="tab" style="height: 70vh;" @select="changeTab" v-if="selectExercise">
            
            <q-tab slot="title" name="tab-1" :label="$t('search')" />
            <q-tab slot="title" name="tab-2" :label="$t('latest')" />
            <q-tab slot="title" name="tab-3" :label="$t('mostUsed')" />
            <q-tab slot="title" name="tab-4" :label="$t('my')" />
           
            <q-scroll-area style="height: 65vh;">
            <q-tab-pane name="tab-1">
                <div class="row">
                    <div class="col">
                        <q-select v-model="muscleGroup" :float-label="$t('muscleGroup')" :options="muscleGroups" :display-value="muscleGroupText" @input="search" clearable />
                    </div>
                     <div class="col">
                         <q-select v-model="equipment" :float-label="$t('equipment')" :options="equipments" :display-value="equipmentText" @input="search" clearable />
                     </div>
                </div>
                
                
                <q-search v-model="searchText" :float-label="$t('search')" :placeholder="$t('exercise')" @input="search" :debounce="500" clearable></q-search>
                <q-list v-if="searchResults.length > 0">
                    <q-item v-for="(e, index) in searchResults" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        <q-item-main>{{ e.name }}</q-item-main>
                        <q-item-side right icon="fas fa-user" v-if="e.userId" />
                        <q-item-side right icon="fas fa-globe" v-else />
                    </q-item>
                </q-list>
                <div v-else>
                    <span v-if="!searching && (searchText && searchText.length >= 2)">{{ $t('noExercises') }}</span>
                </div>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-list>
                    <q-item v-for="(e, index) in latestExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        <q-item-main>{{ e.name }}</q-item-main>
                        <q-item-side right icon="fas fa-user" v-if="e.userId" />
                        <q-item-side right icon="fas fa-globe" v-else />
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-list>
                    <q-item v-for="(e, index) in mostUsedExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        <q-item-main>{{ e.name }}</q-item-main>
                        <q-item-side right icon="fas fa-user" v-if="e.userId" />
                        <q-item-side right icon="fas fa-globe" v-else />
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-4">
                <q-list>
                    <q-item v-for="(e, index) in ownExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        <q-item-main>{{ e.name }}</q-item-main>
                    </q-item>
                </q-list>
            </q-tab-pane>
            </q-scroll-area>
        </q-tabs>
        <template v-if="!selectExercise">
          <div class="row q-ma-sm">     
              <q-btn @click="reselectExercise" :label="exercise ? exercise.name : ''"></q-btn>
          </div>
          <div class="row q-ma-sm" v-if="exercise && exercise.images">     
              <div class="col" v-for="image in exercise.images">
                  <img :src="image.url" class="exercise-image" />
              </div>
          </div>
          <div class="row q-ma-sm" v-if="exercise">   
            <div class="col"><q-input readonly :value="exercise.oneRepMap || '-'" :float-label="$t('1rm')" /></div>  
            <div class="col"><q-input readonly :value="exercise.latestWeights || '-'" :float-label="$t('latestWeights')" /></div>  
          </div>
        </template>

        <div class="row q-ma-sm q-mt-lg">
            <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
            <q-btn glossy color="primary" @click="save" :label="$t('select')" :disabled="!canSave"></q-btn>
        </div>
      </q-modal-layout>
    </q-modal>
</template>

<script>
  import constants from '../store/constants'
  import api from'../api'
  import ExercisesMixin from '../mixins/exercises'
  export default {
    name: 'exercise-picker',
    mixins: [ExercisesMixin],
    data () {
        return {
            tab: 'tab-1',
            selectExercise: false,
            exercise: undefined,
            searchText: '',
            searchResults: [],
            searching: false,
            muscleGroup: undefined,
            equipment: undefined,
            skipConfirm: false
        }
    },
    computed: {
        canSave() {
            return this.exercise;
        },
        latestExercises(){
            return this.$store.state.training.latestExercises.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        mostUsedExercises(){
            return this.$store.state.training.mostUsedExercises.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        ownExercises(){
            return this.$store.state.training.exercises.sort((a,b) => a.name < b.name ? -1 : 1);
        },
        muscleGroups(){
            return this.$store.state.training.muscleGroups.sort((a,b) => a.name < b.name ? -1 : 1).map(g => {return {...g, label: g.name, value: g }});
        },
        muscleGroupText(){
            return this.muscleGroup ? this.muscleGroup.name : this.$t('select');
        },
        equipments(){
            return this.$store.state.training.equipments.sort((a,b) => a.name < b.name ? -1 : 1).map(e => {return {...e, label: e.name, value: e }});
        },
        equipmentText(){
            return this.equipment ? this.equipment.name : this.$t('select');
        },
    },
    methods: {
        show(exercise, skipConfirm){
            this.tab = 'tab-1';
            if(exercise){
                this.exercise = exercise;
                this.selectExercise = skipConfirm;
            }
            else {
                this.exercise = undefined;
                this.selectExercise = true;
            }
            this.skipConfirm = skipConfirm;
            this.$refs.exercisePickerModal.show();
        },
        changeTab(tab){
            /*
            if(tab == 'tab-1' && this.exercise){
                if(this.searchText != this.exercise.name){
                    this.searchText = this.exercise.name;
                    this.search();
                }
                
            }
            */
        },
        search(){
          var self = this;
          if(self.searchText.length >= 2 || self.muscleGroup || self.equipment){
            self.searching = true;
            api.searchExercises(self.searchText, self.muscleGroup ? self.muscleGroup.id : undefined, self.equipment ? self.equipment.id : undefined).then(response => {
                self.searchResults = response.data;
                self.searching = false;
            });
          }
          else {
              self.searchResults = [];
          }
          if(self.exercise && self.searchText.length < self.exercise.name.length){
              self.exercise = undefined;
          }
        },
        /*
        exerciseSelected(exercise){
          this.load(exercise.id);
        },
        */
        load(exerciseId){
            this.$store.dispatch(constants.FETCH_EXERCISE, { id: exerciseId}).then(exercise => {
                this.exercise = exercise;
                this.selectExercise = false;
                if(this.skipConfirm){
                  this.save();
                }
            });
        },
        reselectExercise(){
            this.selectExercise = true;
            this.search();
        },
        tabChanged(tab){
            this.tab = tab;
        },
        cancel () {
            this.searchText = '';
            this.searchResults = [];
            this.exercise = undefined;
            this.$refs.exercisePickerModal.hide();
        },
        hide(){
            this.cancel();
        },
        save () {
            this.muscleGroup = undefined;
            this.equipment = undefined;
            this.searchText = '';
            this.searchResults = [];
            this.$emit('selected', this.exercise);
        },
    },
    created () {
      //this.$store.dispatch(constants.FETCH_EXERCISES, { });
    }
}
</script>

<style scoped>
/*
.q-select { min-width: 50%;}
button{margin-bottom: 10px;}

.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
*/
.exercise-image{
    width:100%;
    height: 100%;
}
.boxed {
  width: 600px; 
  min-height: 500px; 
  margin: auto;
}
</style>
