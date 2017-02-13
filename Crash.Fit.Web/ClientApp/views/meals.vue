<template>
  <div>
      <div class="row">
          <div class="col-sm-2">
              <datetime-picker class="vue-picker1" name="picker1" v-bind:value="start" v-on:change="start=arguments[0]"></datetime-picker>
          </div>
          <div class="col-sm-2">
              <datetime-picker class="vue-picker1" name="picker2" v-bind:value="end" v-on:change="end=arguments[0]"></datetime-picker>
          </div>
          <div class="col-sm-2">
              <button  v-on:click="fetchMeals">Fetch</button>
          </div>
      </div>
      <div class="row">
          <div class="col-sm-12">
              <table>
                  <thead>
                      <tr><th>Time</th><th>Protein</th><th>Carb</th><th>Fat</th><th>Energy</th></tr>
                  </thead>
                  <tbody></tbody>
              </table>
          </div>
      </div>
  </div>
</template>

<script>
api = require('../api')

module.exports = {
    data () {
        return {
            start: null,
            end: null
        }
    },
    components: {
        'datetime-picker': require('../components/datetime-picker')
    },
    methods: {
        fetchMeals: function () { 
            console.log(this.start);
            console.log(this.end);
            api.listMeals(this.start, this.end).then(function (meals) {

            });
        }
    },
    created: function () {
        this.end = new Date();
        this.start = new Date();
    },
    mounted: function () {
        this.fetchMeals();
    }
}
</script>

<style scoped>
</style>