<template>
  <layout >

    <span slot="title">{{ $t('recipes') }}</span>

    <div slot="toolbar">
      <q-btn flat icon="fas fa-plus" @click="createRecipe" :label="$t('recipe')"></q-btn>
    </div>
    <q-page class="q-pa-sm">
      <q-list v-if="recipes.length > 0">
          <q-item v-for="(recipe,index) in recipes" @click.native="clickRecipe(recipe)" :key="index" :separator="true">
            {{ recipe.name }}
          </q-item>
      </q-list>
      <div v-else>{{ $t('noRecipes') }}</div>
    </q-page>
  </layout>
</template>

<script src="./recipes.js">
</script>

<style lang="stylus">
</style>
