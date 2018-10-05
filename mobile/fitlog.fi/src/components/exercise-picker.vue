<template>
    <q-modal ref="modal">
        <q-tabs v-model="tab" style="height: 82vh;" @select="changeTab" v-if="selectExercise">
            
            <q-tab slot="title" name="tab-1"  :label="$t('search')" />
            <q-tab slot="title" name="tab-2"  :label="$t('latest')" />
            <q-tab slot="title" name="tab-3" :label="$t('mostUsed')" />
            <q-tab slot="title" name="tab-4"  :label="$t('my')" />
           
            <q-scroll-area style="height: 75vh;">
            <q-tab-pane name="tab-1">
                <div class="row">
                    <div class="col">
                        <q-select v-model="muscleGroup" :float-label="$t('muscleGroup')" :options="muscleGroups" :display-value="muscleGroupText" @input="search" />
                    </div>
                     <div class="col">
                         <q-select v-model="equipment" :float-label="$t('equipment')" :options="equipments" :display-value="equipmentText" @input="search" />
                     </div>
                </div>
                
                
                <q-search v-model="searchText" :float-label="$t('search')" :placeholder="$t('exercise')" @input="search" :debounce="500" clearable></q-search>
                <q-list v-if="searchResults.length > 0">
                    <q-item v-for="(e, index) in searchResults" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        {{ e.text }}
                    </q-item>
                </q-list>
                <div v-else>
                    <span v-if="!searching && (searchText && searchText.length >= 2)">{{ $t('noExercises') }}</span>
                </div>
            </q-tab-pane>
            <q-tab-pane name="tab-2">
                <q-list>
                    <q-item v-for="(e, index) in latestExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        {{ e.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-3">
                <q-list>
                    <q-item v-for="(e, index) in mostUsedExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        {{ e.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            <q-tab-pane name="tab-4">
                <q-list>
                    <q-item v-for="(e, index) in ownExercises" @click.native="load(e.id)" :class="{selected: exercise && e.id == exercise.id }" :key="index" :separator="true">
                        {{ e.name }}
                    </q-item>
                </q-list>
            </q-tab-pane>
            </q-scroll-area>
        </q-tabs>
        <div class="row q-ma-sm" v-if="!selectExercise">     
            <q-btn @click="reselectExercise" :label="exercise ? exercise.name : ''"></q-btn>
        </div>
        <div class="row q-ma-sm" v-if="!selectExercise && exercise && exercise.images">     
            <div class="col" v-for="image in exercise.images">
                <img :src="image.url" class="exercise-image" />
            </div>
        </div>
        <div class="row q-ma-sm q-mt-lg">
            <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
            <q-btn glossy color="primary" @click="save" :label="$t('save')" :disabled="!canSave"></q-btn>
        </div>
    </q-modal>
</template>

<script>
    import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem } from 'quasar'
    import constants from '../store/constants'
    import api from'../api'

export default {
    name: 'exercise-picker',
    data () {
        return {
            tab: 'tab-1',
            selectExercise: false,
            exercise: undefined,
            searchText: '',
            searchResults: [],
            searching: false,
            muscleGroup: undefined,
            equipment: undefined
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
            return this.$store.state.training.muscleGroups.sort((a,b) => a.name < b.name ? -1 : 1).map(g => {return {...g, label: g.name, value: g }});;
        },
        muscleGroupText(){
            return this.muscleGroup ? this.muscleGroup.name : this.$t('select');
        },
        equipments(){
            return this.$store.state.training.equipment.sort((a,b) => a.name < b.name ? -1 : 1).map(e => {return {...e, label: e.name, value: e }});;
        },
        equipmentText(){
            return this.equipment ? this.equipment.name : this.$t('select');
        },
    },
    components: {
        QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem
    },
    methods: {
        show(group){
            var self = this;
            self.tab = 'tab-1';
            if(group.exercise){
                self.exercise = group.exercise;
                self.selectExercise = false;
            }
            else {
                self.selectExercise = true;
            }
            self.$refs.modal.show();
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
            api.searchExercises(self.searchText, self.muscleGroup ? self.muscleGroup.id : undefined, self.equipment ? self.equipment.id : undefined).then(results => {
                self.searchResults = results.map(e => { return { ...e, text: e.name, icon: e.userId ? 'fas fa-user' : '' }});
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
        exerciseSelected(exercise){
          this.load(exercise.id);
        },
        load(exerciseId){
            var self = this;
            self.$store.dispatch(constants.FETCH_EXERCISE, {
                id: exerciseId,
                success (exercise) {
                    //self.searchText = food.manufacturer ? `${food.name} (${food.manufacturer})` : food.name;
                    self.exercise = exercise;
                    self.selectExercise = false;
                },
                failure () {
                    self.notifyError(self.$t('fetchFailed'));
                }
            });
        },
        reselectExercise(){
            this.selectExercise = true;
        },
        tabChanged(tab){
            this.tab = tab;
        },
        cancel () {
            this.searchText = '';
            this.searchResults = [];
            this.exercise = undefined;
            this.$refs.modal.hide();
        },
        hide(){
            this.cancel();
        },
        save () {
            var self = this;
            this.$emit('selected', self.exercise);
        },
    },
    mounted () {

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
</style>
