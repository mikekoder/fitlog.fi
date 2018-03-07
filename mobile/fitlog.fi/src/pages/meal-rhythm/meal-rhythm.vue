<template>
<q-page class="q-pa-sm">
        <q-card v-for="(def, index) in definitions" :key="index" class="q-ma-sm">
          <q-card-main>
            <div class="row">
                  <div class="col-10"><q-input v-model="def.name" type="text" :float-label="$t('name')" /></div>
            </div>    
            <div class="row q-mt-sm">
                <div class="col-5">
                    <q-datetime v-model="def.start" format24h type="time" :float-label="$t('starts')" />
                </div>
                <div class="col-5">   
                    <q-datetime v-model="def.end" format24h type="time" :float-label="$t('ends')" />
                </div>
                <div class="col-2"><q-btn round glossy small color="primary" icon="fa-trash"/></div>
            </div>
          </q-card-main>
        </q-card>
        <q-card class="q-ma-sm">
        <q-card-main>
            <div class="row">
                <div class="col-10">
                    <q-input v-model="othersName" type="text" :float-label="$t('otherTimes')" />
                </div>
            </div>
        </q-card-main>
        </q-card>
    <div class="row q-ma-sm q-my-lg">
        <q-btn glossy @click="addMeal" color="secondary" icon="fa-plus" :label="$t('meal')" class="q-mr-sm"></q-btn>
        <q-btn glossy @click="save" color="primary" :label="$t('save')"></q-btn>
    </div>
    
</q-page>
</template>

<script src="./meal-rhythm.js">
</script>

<style lang="stylus">
</style>
