<template>
<layout >

  <span slot="title">{{ $t('nutritionGoals') }}</span>

  <div slot="toolbar">
    <q-btn flat icon="fas fa-plus" @click="createGoal" :label="$t('goal')"></q-btn>
  </div>
  <q-page class="q-pa-sm">
      <q-list v-if="goals.length > 0">
          <q-item v-for="(goal,index) in goals" @click.native="clickGoal(goal)" :key="index" :separator="true" :class="{selected: goal.active }">
            <q-item-main>
              <div class="row">
                <div class="col-11">{{ goal.name }}</div>
                <div class="col-1"><q-icon name="fas fa-check" v-if="goal.active"></q-icon></div>
              </div>
            </q-item-main>
            
          </q-item>
      </q-list>
      <div v-else>{{ $t('noGoals') }}</div>
  </q-page>
  </layout>
</template>

<script src="./nutrition-goals.js">
</script>

<style lang="stylus">
</style>
