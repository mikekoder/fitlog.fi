<template>
  <q-page class="q-pa-sm">
      <q-scroll-area>
        <q-list v-if="goals.length > 0">
            <q-item v-for="(goal,index) in goals" @click.native="clickGoal(goal)" :key="index" :separator="true">
              <div class="col-10">{{ goal.name }}</div>
              <div class="col-2"></div>
            </q-item>
        </q-list>
        <div v-else>{{ $t('noGoals') }}</div>
      </q-scroll-area>
    <div class="row q-my-lg">
      <q-btn color="primary" glossy icon="fa-plus" @click="createGoal" :label="$t('goal')"></q-btn>
    </div>
  </q-page>
</template>

<script src="./nutrition-goals.js">
</script>

<style lang="stylus">
</style>
