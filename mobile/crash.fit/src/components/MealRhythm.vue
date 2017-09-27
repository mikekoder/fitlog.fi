<template>
<div :class="{desktop: isDesktop }">
        <q-card v-for="(def, index) in definitions" :key="index">
          <q-card-main>
              <div class="row">
                  <div class="col-10 col-md-6 col-lg-4 col-xl-4"><q-input v-model="def.name" type="text" :float-label="$t('name')" /></div>
                  <div class="col-2 col-lg-1"><q-btn round color="primary" icon="fa-trash" small /></div>
                  
                  
            </div>    
            <div class="row">
                <div class="col col-md-3 col-lg-2 col-xl-2">
                    <q-datetime v-model="def.start" type="time" :float-label="$t('start')" />
                </div>
                <div class="col col-md-3 col-lg-2 col-xl-2">   
                    <q-datetime v-model="def.end" type="time" :float-label="$t('end')" />
                </div>
            </div>
          </q-card-main>
        </q-card>
        <q-card>
        <q-card-main>
            <div class="row">
                <div class="col col-lg-4">
                    <q-input v-model="othersName" type="text" :float-label="$t('otherTimes')" />
                </div>
            </div>
        </q-card-main>
        </q-card>
    <div class="row pad buttons">
        <q-btn @click="cancel">{{ $t('cancel')}}</q-btn>
        <q-btn @click="addMeal" color="secondary">{{ $t('add')}}</q-btn>
        <q-btn @click="save" color="primary">{{ $t('save')}}</q-btn>
    </div>
    
</div>
</template>

<script>
    import constants from '../store/constants'
    import formatters from '../formatters'
import { QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator,QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime,QField, QInput,QLayout,QFixedPosition } from 'quasar'
export default {
  data () {
    return {
      othersId: '',
      othersName: '',
      definitions: []
    }
  },
  components:{
    QIcon,QCard,QCardTitle,QCardMain,QCardActions,QCardSeparator, QModal,QBtn,QTabs,QTab,QTabPane,QScrollArea,QFab,QFabAction,QContextMenu, QItem,QDatetime ,QField, QInput,QLayout,QFixedPosition
  },
  computed: {
  },
  methods: {
    addMeal() {
        this.definitions.push({start: '01.01.2000 10:00', end: '01.01.2000 13:00'});
    },
    removeMeal(index) {
        this.definitions.splice(index, 1);
    },
    save() {
        var self = this;
        var defs = self.definitions.filter(d => d.name && d.start && d.end).map(d => { return { id: d.id, name: d.name, start: self.formatTime(d.start), end: self.formatTime(d.end) } });
        defs.push({ id: self.othersId, name: self.othersName });
        self.$store.dispatch(constants.SAVE_MEAL_DEFINITIONS, {
            definitions: defs,
            success() {
                toaster.info(self.$t('saved'));
            },
            failure() {
                toaster.error(self.$t('saveFailed'));
            }
        });
    },
    cancel(){

    },
    formatTime: formatters.formatTime
  },
  created(){
    var self = this;
        self.$store.dispatch(constants.FETCH_MEAL_DEFINITIONS, {
            success(definitions) {
                self.definitions = definitions.filter(d => d.startHour).map(d => { return { id: d.id, name: d.name, start: '01.01.2000 ' + d.startHour + ':' + d.startMinute, end: '01.01.2000 ' + d.endHour + ':' + d.endMinute } });
                var other = definitions.find(d => !d.startHour);
                if (other) {
                    self.othersId = other.id;
                    self.othersName = other.name;
                }
                self.$store.commit(constants.LOADING_DONE);
            },
            failure() {
                toaster.error(self.$t('fetchFailed'));
            }
        });
  },
  mounted () {
  },
  beforeDestroy () {

  }
}
</script>

<style lang="stylus">
</style>
