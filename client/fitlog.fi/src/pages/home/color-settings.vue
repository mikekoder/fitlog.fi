<template>
    <q-modal ref="modal">
        <div class="row q-ma-sm">
            {{ $t('colors')}}
        </div>
        <div class="row">
        </div>
        
        <div class="row q-ma-sm">
            <q-btn glossy @click="cancel" :label="$t('cancel')" class="q-mr-sm"></q-btn>
            <q-btn glossy color="primary" @click="save" :label="$t('save')"></q-btn>
        </div>
    </q-modal>
</template>

<script>
    import constants from '../../store/constants'
    import api from '../../api'

export default {
    name: 'color-settings',
    data () {
        return {
            proteinColor: undefined,
            carbColor: undefined,
            fatColor: undefined
        }
    },
    methods: {
        show(){  
          var selectedNutrients = [];
          this.visibleNutrients.forEach(n => { selectedNutrients.push(n.id); });
          for (var i = selectedNutrients.length; i < 6; i++) {
            selectedNutrients.push(undefined);
          }
          this.selectedNutrients = selectedNutrients;
          this.$refs.modal.show();
        },
        cancel () {
            this.$refs.modal.hide();
        },
        save () {
          var settings = {
            proteinColor: this.proteinColor,
            carbColor: this.carbColor,
            fatColor: this.fatColor
          };
          this.$store.dispatch(constants.SAVE_NUTRIENT_COLOR_SETTINGS, {
            settings
          }).then(_ => {
            this.$refs.modal.hide();
          }).catch(_ => {

          });
            
        }
    }
}
</script>

<style scoped>
select { width: 99%;}
</style>
