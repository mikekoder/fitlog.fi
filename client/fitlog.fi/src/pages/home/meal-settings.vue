<template>
  <q-modal ref="modal" maximized>
    <q-modal-layout view="LHh lpR fff" :container="isDesktop" :class="{'boxed': isDesktop }">      
      <q-toolbar color="tertiary" glossy>
        <q-toolbar-title>
          {{ $t('nutrientsToShow')}}
        </q-toolbar-title>
      </q-toolbar>

      <q-list>
        <q-item v-for="(id,index) in selectedNutrients">
          <q-item-main>
            <q-select v-model="selectedNutrients[index]" :options="nutrients" :float-label="$t('nutrient')"></q-select>
          </q-item-main>
        </q-item>
      </q-list>
      
      <div class="row q-ma-sm">
        <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
        <q-btn glossy color="primary" @click="save" :label="$t('save')"></q-btn>
      </div>
    </q-modal-layout>
  </q-modal>
</template>

<script>
  import constants from '../../store/constants'
  import api from '../../api'

  export default {
    name: 'meal-settings',
    data () {
      return {
        nutrients: [],
        selectedNutrients: []
      }
    },
    methods: {
      show(){  
        this.$store.dispatch(constants.FETCH_NUTRIENTS,{}).then(nutrients => {
          var nutrientOptions = [];
          this.$store.state.nutrition.nutrientGroups.forEach(g => {
            nutrientOptions.push({
              label: g.name,
              value: 'group-' + g.id,
              disable: true
            });
            nutrients.filter(n => n.fineliGroup == g.id).sort((n1, n2) => n1.name < n2.name ? -1 : 1).forEach(n => {
              nutrientOptions.push({
                label: `${n.name} (${this.formatUnit(n.unit)})`,
                value: n.id
              });
            });
          });
          this.nutrients = nutrientOptions;

          var visibleNutrients = nutrients.filter(n => n.homeOrder || n.homeOrder === 0).sort((n1,n2) => n1.homeOrder - n2.homeOrder);
          var selectedNutrients = [];
          visibleNutrients.forEach(n => { 
            selectedNutrients.push(n.id);
          });
          for (var i = selectedNutrients.length; i < 6; i++) {
              selectedNutrients.push(undefined);
          }
          this.selectedNutrients = selectedNutrients;
          this.$refs.modal.show();
        });
      },
      cancel () {
        this.$refs.modal.hide();
      },
      save () {
        var self = this;
        var settings = {
          nutrients: self.selectedNutrients
        };
        self.$store.dispatch(constants.SAVE_MEAL_DIARY_SETTINGS, {
          settings
        }).then(_ => {
          self.$refs.modal.hide();
        });
      }
    },
    created(){
      
    }
  }
</script>

<style scoped>

</style>
