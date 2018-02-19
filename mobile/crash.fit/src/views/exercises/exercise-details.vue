<template>
  <div :class="{desktop: isDesktop }">
    <div class="row pad">
      <q-input type="text" v-model="name" :float-label="$t('name')" />
    </div>
    <div class="row pad" v-for="muscleGroup in muscleGroups">
      <q-toggle v-model="targets" :label="muscleGroup.name" :val="muscleGroup.id" />
    </div>
    <div class="row pad">
      <q-input type="number" v-model="percentageBW" suffix="%" :float-label="$t('bodyWeightPercentage')" />
    </div>
    <div class="row pad buttons">
      <q-btn @click="cancel">{{ $t('cancel') }}</q-btn>
      <q-btn color="primary" @click="save">{{ $t('save') }}</q-btn>
    </div>
  </div>
</template>

<script src="./exercise-details.js">
</script>

<style lang="stylus">
.pad{ padding-bottom: 10px;}
</style>
