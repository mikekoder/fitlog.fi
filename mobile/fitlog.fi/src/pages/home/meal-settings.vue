<template>
    <q-modal ref="modal">
        <div class="row q-ma-sm">
            {{ $t('nutrientsToShow')}}
        </div>
        <div class="row">
<q-list>
            <q-item v-for="(id,index) in selectedNutrients">
                <q-item-main>
                    <select v-model="selectedNutrients[index]">
                        <option v-bind:value="undefined"></option>
                        <template v-for="(group,index_g) in nutrientGroups">
                            <option disabled>{{ group.name }}</option>
                            <option v-for="(nutrient,index_n) in group.nutrients" v-bind:value="nutrient.id">
                            {{ nutrient.name }} ({{ formatUnit(nutrient.unit) }})
                            </option>
                        </template>
                    </select>
                </q-item-main>
            </q-item>
        </q-list>
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
    name: 'meal-settings',
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
            var self = this;
            var settings = {
                nutrients: self.selectedNutrients
            };
            self.$store.dispatch(constants.SAVE_MEAL_DIARY_SETTINGS, {
                settings,
                success() {
                    self.$refs.modal.hide();
                },
                failure() { }
            });
            
        }
    }
}
</script>

<style scoped>
select { width: 99%;}
</style>
