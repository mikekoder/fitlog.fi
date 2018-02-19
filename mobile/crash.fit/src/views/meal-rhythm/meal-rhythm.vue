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
                    <q-datetime v-model="def.start" type="time" :float-label="$t('starts')" />
                </div>
                <div class="col col-md-3 col-lg-2 col-xl-2">   
                    <q-datetime v-model="def.end" type="time" :float-label="$t('ends')" />
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
      <!--
        <q-btn @click="cancel">{{ $t('cancel')}}</q-btn>
          -->
        <q-btn @click="addMeal" color="secondary">{{ $t('add')}}</q-btn>
        <q-btn @click="save" color="primary">{{ $t('save')}}</q-btn>
    </div>
    
</div>
</template>

<script src="./meal-rhythm.js">
</script>

<style lang="stylus">
</style>
