<template>
    <q-modal ref="modal" :class="{desktop: isDesktop }">
      <h5>{{ $t('nutrientsToShow')}}</h5>
      <q-list>
          <q-item v-for="(id,index) in selectedNutrients" :key="index">
              <q-item-main>
                <select v-model="selectedNutrients[index]">
                    <option v-bind:value="undefined"></option>
                    <template v-for="(group,index_g) in nutrientGroups">
                        <option disabled :key="index_g">{{ group.name }}</option>
                        <option v-for="(nutrient,index_n) in group.nutrients" v-bind:value="nutrient.id" :key="index_n">
                        {{ nutrient.name }} ({{ unit(nutrient.unit) }})
                        </option>
                    </template>
                </select>
              </q-item-main>
          </q-item>
      </q-list>
      <div class="row pad buttons">
        <q-btn @click="close">{{ $t('cancel') }}</q-btn>
        <q-btn color="primary" @click="save">{{ $t('save') }}</q-btn>
      </div>
    </q-modal>
</template>

<script>
    import { QTabs,QTab,QTabPane,QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem,QItemMain } from 'quasar'
    import constants from '../store/constants'
    import api from'../api'
    import formatters from '../formatters'
export default {
    data () {
        return {
            selectedNutrients: []
        }
    },
    props: {
    },
    computed: {
        nutrientGroups() {
            var nutrients = this.$store.state.nutrition.nutrients;
            return this.$store.state.nutrition.nutrientGroups.map(g => {
                return {
                    name: g.name,
                    nutrients: nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1)
                }
            });
            return 
        },
        visibleNutrients() {
            return this.$store.state.nutrition.nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1,n2) => n1.homeOrder - n2.homeOrder);
        }
    },
    components: {
        QField,QInput,QScrollArea,QSearch,QAutocomplete,QSelect,QBtn,QModal,QList,QItem,QItemMain
    },
    methods: {
        open(){  
            var selectedNutrients = [];
            this.visibleNutrients.forEach(n => { selectedNutrients.push(n.id); });
            for (var i = selectedNutrients.length; i < 6; i++) {
                selectedNutrients.push(undefined);
            }
            this.selectedNutrients = selectedNutrients;
            this.$refs.modal.open();
        },
        close () {
            this.$refs.modal.close();
        },
        save () {
            var self = this;
            var settings = {
                nutrients: self.selectedNutrients
            };
            self.$store.dispatch(constants.SAVE_MEAL_DIARY_SETTINGS, {
                settings,
                success() {
                    self.close();
                },
                failure() { }
            });
            
        },
        unit: formatters.formatUnit
    }
}
</script>

<style scoped>
select { width: 99%;}
.q-select { min-width: 50%;}
.selected{background: hsla(0,0%,74%,.5);}
.desktop .q-tab-pane { height: 400px;}
.desktop .q-scrollarea { height: 100%;}
</style>